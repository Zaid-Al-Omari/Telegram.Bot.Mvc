using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Mvc.Core;

namespace Telegram.Bot.Mvc.WebHook.Configurations
{
    public class Logger : ILogger
    {
        public void Log(Exception ex, BotRouteData routeData = null)
        {
            var error = "";
            if(routeData != null) {
                error += string.Format("ERROR:\t /{0}/{1}/{2}: \n", 
                    routeData.Controller, 
                    routeData.Method, 
                    string.Join(",", routeData.Parameters));
            }
            error += "\t" + ex.Message;
            Console.WriteLine(error);
        }
    }
}
