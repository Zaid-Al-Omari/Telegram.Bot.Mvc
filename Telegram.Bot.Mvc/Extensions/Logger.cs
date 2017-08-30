using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Mvc.Core;
using Telegram.Bot.Mvc.Framework;

namespace Telegram.Bot.Mvc.Extensions
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

        public void LogStats(BotContext context, string type)
        {
            var message = "";
            message += string.Format("INFO:\t /{0}/{1}/{2}: \n",
                 context.RouteData.Controller,
                 context.RouteData.Method,
                 string.Join(",", context.RouteData.Parameters));
            message += "\t" + context.User?.Id + "\t";
            message += type ;
            
          Console.WriteLine(message);
        }
    }
}
