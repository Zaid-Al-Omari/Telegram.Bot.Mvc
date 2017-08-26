using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Mvc.Framework;
using Telegram.Bot.Mvc.WebHook.Configurations;
using Telegram.Bot.Types;

namespace Telegram.Bot.Mvc.WebHook.Controllers
{
    [Route("api/[controller]")]
    public class WebhooksController : Controller
    {
        // POST api/Webhooks/[botUsername]
        [HttpPost("{botUsername}")]
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
                context = new BotContext(null, session, update);
                await session.Router.Route(context);
            }
            catch (Exception ex)
            {
                logger.Log(ex, context?.RouteData);
            }
            return Ok(); // Suppress Errors ...
        }
        
    }
}
