using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Telegram.BotMVC.Core
{
    public interface ILogger
    {
        void Log(Exception ex, BotRouteData routeData = null);
    }
}
