using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Mvc.Core;
using Telegram.Bot.Mvc.Framework;
using Telegram.Bot.Mvc.Scheduler;

namespace Telegram.Bot.Mvc.WebHook.Configurations
{
    public static class BotMvcExtensions
    {
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
            var logger = new Logger();
            var scheduler = new PerSecondScheduler(logger, tasksCount: 30, inSeconds: 1);
            var router = new BotRouter(new BotControllerFactory(scheduler));
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
