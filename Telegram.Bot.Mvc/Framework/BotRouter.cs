using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Mvc.Core;

namespace Telegram.Bot.Mvc.Framework {
    public class BotRouter {

        protected IEnumerable<Type> Controllers { get; }
        protected BotControllerFactory ControllerFactory { get; set; }

        public BotRouter(BotControllerFactory factory) {
            ControllerFactory = factory;
            Controllers = Assembly.GetEntryAssembly().GetTypes()
                .Where(x => x.GetTypeInfo().IsSubclassOf(typeof(BotController)))
                .Select(x => x).ToList();
        }

        public Task Route(BotContext context) {
            // Data Parsing ...
            string body = context.Update.Message?.Text;
            if (context.Update.Type == UpdateType.CallbackQueryUpdate) body = context.Update.CallbackQuery?.Data;
            if (string.IsNullOrEmpty(body)) body = "";
            string[] pathFragments; object[] parameters;
            if (body.StartsWith("/")) {
                pathFragments = body.Split(' ');
                parameters = new object[pathFragments.Length - 1];
                Array.Copy(pathFragments, 1, parameters, 0, parameters.Length);
            }
            else {
                pathFragments = new string[] { body };
                parameters = null;
            }

            int parametersCount = parameters == null ? 0 : parameters.Length;
            string command = pathFragments[0].ToLowerInvariant();

            // Controller & Method Resolution...
            var resolutionResult = Controllers.Select(x =>
                new {
                    ControllerType = x,
                    Method = GetMethod(context, x, command, parametersCount)
                }).FirstOrDefault(x => x.Method != null);
            if (resolutionResult == null)
                throw new Exception("Can't Find Method For Path: " + command + ", " + context.Update.Type.ToString());

            // Controller Init ...
            BotController controller = ControllerFactory.Create(resolutionResult.ControllerType, context);
            context.RouteData = new BotRouteData(
                controller.GetType().Name,
                resolutionResult.Method.Name,
                parameters == null ? new string[] { body }.AsEnumerable() : parameters.Select(x => x == null ? "null" : x.ToString())
                );
            // Method Invocation ...
            return resolutionResult.Method.Invoke(controller, parameters) as Task;
        }

        protected MethodInfo GetMethod(BotContext context, Type controllerType, string path, int paramaetersCount) {
            var candidates = controllerType.GetTypeInfo().GetMethods()
                   .Where(x =>
                        x.GetParameters().Length == paramaetersCount &&
                        x.GetCustomAttributes(typeof(BotPathAttribute), false).Any(z =>
                           (z as BotPathAttribute).Path.ToLowerInvariant() == path &&
                           (z as BotPathAttribute).UpdateType == context.Update.Type)
                    );

            if (candidates.Count() > 1) return null; //throw new Exception("Multiable Methods With The Same Path!");
            var method = candidates.FirstOrDefault();
            if (method == null) {
                method = controllerType.GetTypeInfo().GetMethod(path.TrimStart('/'), BindingFlags.IgnoreCase);
            }
            if (method == null) {
                method = controllerType.GetTypeInfo().GetMethods()
                   .FirstOrDefault(x => x.GetCustomAttributes(typeof(AnyPathAttribute), false).Any(z =>
                        (z as AnyPathAttribute).UpdateType == context.Update.Type));
            }
            if (method == null || method.ReturnType != typeof(Task)) return null; // throw new Exception("Can't Find Method For Path " + path);
            return method;
        }

    }
}