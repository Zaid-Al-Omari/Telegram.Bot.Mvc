using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Mvc.Framework;

namespace Telegram.Bot.Mvc.Core
{
    public interface ILogger
    {
        void Log(Exception ex, BotRouteData routeData = null);
        void LogStats(BotContext context, string type);
    }
}
