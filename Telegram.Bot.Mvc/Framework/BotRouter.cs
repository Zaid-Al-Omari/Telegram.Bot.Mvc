using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Mvc.Core;

namespace Telegram.Bot.Mvc.Framework {
    public class BotRouter : IBotRouter
    { 


        public Task Route(BotContext context, IBotControllerFactory factory) {
            // Data Parsing ...
            string body = context.Update.Message?.Text;
            if (context.Update.Type == UpdateType.CallbackQueryUpdate) body = context.Update.CallbackQuery?.Data;
            if (context.Update.Type == UpdateType.InlineQueryUpdate) body = context.Update.InlineQuery?.Query;

            if (string.IsNullOrEmpty(body)) body = "";
            string[] pathFragments;
            object[] parameters;
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
            var resolutionResult = factory.GetControllers()
                .Select(x => new {
                    ControllerType = x,
                    Method = GetMethod(context, x, command, parametersCount)
                })
                .OrderBy(x=> x.Method.GetCustomAttributes(typeof(AnyPathAttribute), false).Count())
                .FirstOrDefault(x => x.Method != null);
            if (resolutionResult == null)
                throw new Exception("Can't Find Method For Path: " + command + ", " + context.Update.Type.ToString());

            // Parameters Optimization ...
            var methodParametersCount = resolutionResult.Method.GetParameters().Count();
            var optimizedParameters = OptimizedParameters(methodParametersCount, body, parameters);

            // Controller Init ...
            using (var controller = factory.Create(resolutionResult.ControllerType, context))
            {
                context.RouteData = new BotRouteData(
                                  controller.GetType().Name,
                                  resolutionResult.Method.Name,
                                  optimizedParameters.Select(x => x == null ? "null" : x.ToString())
                              );

                // Method Invocation ...
                return resolutionResult.Method.Invoke(controller, optimizedParameters.Length == 0 ? null : optimizedParameters) as Task;
            }
        }

        protected object[] OptimizedParameters(int wantedCount, string body, object[] parameters)
        {
            var optimizedParameters = new object[wantedCount];
            if (parameters != null && wantedCount != 0 && wantedCount <= parameters.Length)
            {
                Array.Copy(parameters, 0, optimizedParameters, 0, wantedCount);
                optimizedParameters[optimizedParameters.Length - 1] = string.Join(" ", parameters.Where((x, i) => i >= wantedCount - 1));
            }
            else if (wantedCount == 1 && parameters == null)
            {
                optimizedParameters[0] = body;
            }
            return optimizedParameters;
        }

        protected MethodInfo GetMethod(BotContext context, Type controllerType, string path, int paramaetersCount) {
            var candidates = controllerType.GetTypeInfo().GetMethods()
                   .Where(x =>
                        x.GetCustomAttributes(typeof(BotPathAttribute), false).Any(z =>
                           (z as BotPathAttribute).Path.ToLowerInvariant() == path &&
                           (z as BotPathAttribute).UpdateType == context.Update.Type)
                    ).OrderByDescending(x=> x.GetParameters().Length);

            var method = candidates.FirstOrDefault(x => x.GetParameters().Length <= paramaetersCount);
            if (method == null) {
                method = controllerType.GetTypeInfo().GetMethods()
                   .FirstOrDefault(x => x.GetCustomAttributes(typeof(AnyPathAttribute), false).Any(z =>
                        (z as AnyPathAttribute).UpdateType == context.Update.Type));
            }
            if (method == null) return null; // throw new Exception("Can't Find Method For Path " + path);
            return method;
        }

    }
}