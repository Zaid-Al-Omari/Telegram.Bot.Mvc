using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Telegram.Bot.Mvc.Core;
using Telegram.Bot.Mvc.Framework;
using Telegram.Bot.Mvc.Scheduler;

namespace Telegram.Bot.Mvc.WebHook.Configurations
{
    public static class BotMvcExtensions
    {

        // ToDo: Configure these statics ...
        private static string certificateFilePath = Path.Combine(Environment.CurrentDirectory, "Certificate", "cer.pem");
        private static string publicBaseUrl = "https://example.com/api/Webhooks/"; // should be mapped in UseMvc
        private static bool registerCertificate = true;

        private static IEnumerable<string> GetTokens()
        {
            // ToDo: Get the tokens from the data store.
            return new string[] { "<tokens here>" };
        }

        public static IServiceCollection AddBotMvc(this IServiceCollection services)
        {
            // use your own implementation of ILogger ...
            var logger = new Logger();
            
            // use PerSecondScheduler to throttle the outgoing messages to 30 messages per second using a multi-level priority queue...
            var scheduler = new PerSecondScheduler(logger, tasksCount: 30, inSeconds: 1);

            // if your controllers are in a differant assembly change it here ...

            var controllersAssembly = Assembly.GetEntryAssembly();
            // var controllersAssembly = typeof(Controllers.HelloController).Assembly

            var router = new BotRouter(
                factory: new BotControllerFactory(scheduler), 
                controllersAssembly: controllersAssembly);

            services.AddSingleton<ILogger>(logger);
            services.AddSingleton(router);
            services.AddSingleton(scheduler);

            // Key-value pair of botUsername : BotSession
            var tokens = GetTokens();
            var sessions = new Dictionary<string, BotSession>();
            foreach (var token in tokens)
            {
                var session = new BotSession(new TelegramBotClient(token), router, logger, token);
                if (registerCertificate) session.RegisterCertificate(
                    certificateFilePath,
                    Path.Combine(publicBaseUrl, session.Username)).Wait(); 
                sessions.Add(session.Username, session);
            }
            services.AddSingleton<IDictionary<string, BotSession>>(sessions);
            return services;
        }
    }
}
