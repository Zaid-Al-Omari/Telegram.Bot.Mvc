using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.BotMVC.Core;

namespace Telegram.BotMVC.Framework
{
    public class BotListener {

        private ILogger _logger;
        private ITelegramBotClient _bot;
        private BotRouter _router;
        private BotSession _session;
        public User BotInfo { get; protected set; }
        public BotListener(string token, BotRouter router, ILogger logger) {
            _router = router;
            _logger = logger;
            _bot = new TelegramBotClient(token);
            BotInfo = _bot.GetMeAsync().Result;
            _bot.OnReceiveError += Bot_OnReceiveError;
            _bot.OnReceiveGeneralError += Bot_OnReceiveGeneralError;
            _bot.OnUpdate += _bot_OnUpdate;
        }

        private async void _bot_OnUpdate(object sender, Bot.Args.UpdateEventArgs e) {
            var context = new BotContext(null, _session, e.Update);
            try {
                await _router.Route(context);
            }
            catch (Exception ex) {
                _logger.Log(ex, context.RouteData);
            }
        }

        private void Bot_OnReceiveGeneralError(object sender, Telegram.Bot.Args.ReceiveGeneralErrorEventArgs e) {
            _logger.Log(e.Exception);
        }

        private void Bot_OnReceiveError(object sender, Telegram.Bot.Args.ReceiveErrorEventArgs e) {
            _logger.Log(e.ApiRequestException);
        }

        public void SetSession(BotSession session) {
            _session = session;
        }

        public void Start() {
            _bot.StartReceiving();
        }

        public void Stop() {
            _bot.StopReceiving();
        }
    }
}
