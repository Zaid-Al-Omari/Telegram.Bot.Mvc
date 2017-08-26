using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Mvc.Framework;

namespace Telegram.Bot.Mvc.WebHook
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();
            

            var router = new BotRouter(new BotControllerFactory());
            services.AddSingleton(new Logger());
            services.AddSingleton(router);

            // Key-value pair of botUsername : BotSession
            // ToDo: Get the tokens from the data store.
            var tokens = new string[] { "356143438:AAG4zU9PGgB1_9Az_gWMnsPvSoL5QW2f3Fo" };
            var sessions = new Dictionary<string, BotSession>();
            foreach(var token in tokens)
            {
                var session = new BotSession(new TelegramBotClient(token), router);
                sessions.Add(session.Username, session);
            }
            services.AddSingleton(sessions);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            // Setup End-point for all bots.
            app.UseMvc((config)=> {
                config.MapRoute("WebhookApi", "api/{controller}/{botUsername}");
            });
        }
    }
}
