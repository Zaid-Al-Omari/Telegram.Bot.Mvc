using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Mvc.Framework;
using Telegram.Bot.Types.Enums;

namespace Telegram.Bot.Mvc.WebHook.Controllers
{
    [Route("api/[controller]")]
    public class WebhooksController : Controller
    {
        // POST api/Webhooks/[botUsername]
        [HttpPost]
        public async Task<IActionResult> Post(
            [FromRoute] string botUsername, 
            [FromBody] Update update, 
            [FromServices] IDictionary<string, BotSession> sessions,
            [FromServices] Logger logger)
        {
            BotContext context = null;
            BotSession session = null;
            try
            {
                if (update == null) throw new ArgumentException("update is null!");
                sessions.TryGetValue(botUsername, out session);
                if (session == null) throw new ArgumentException("session is null, bot token is not registered!");
                await session.Router.Route(context);
            }
            catch (Exception ex)
            {
                logger.Log(ex, context.RouteData);
            }
            return Ok(); // Suppress Errors ...
        }
        
    }
}
