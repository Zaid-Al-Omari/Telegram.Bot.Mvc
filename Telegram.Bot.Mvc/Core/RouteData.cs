using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Telegram.Bot.Mvc.Core
{
    public class BotRouteData {

        public BotRouteData(string controller, string method, IEnumerable<string> parameters) {
            Controller = controller;
            Method = method;
            Parameters = parameters;
        }
        public string Controller { get; protected set; }

        public string Method { get; protected set; }

        public IEnumerable<string> Parameters { get; protected set; }
    }
}
