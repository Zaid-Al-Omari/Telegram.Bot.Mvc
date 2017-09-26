using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Mvc.Core;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Mvc.Scheduler;

namespace Telegram.Bot.Mvc.Framework
{
    public class BotListener  : IDisposable{

        private readonly ILogger _logger;
        private readonly Func<IBotControllerFactory> _factoryCreator;
        private readonly IBotRouter _router;
        private BotSession _session;

        public User BotInfo { get; protected set; }
        public ITelegramBotClient Bot { get; protected set; }
        public string Token { get; protected set; }


        public BotListener(string token, ILogger logger, IBotRouter router, Func<IBotControllerFactory> factoryCreator) {
            _logger = logger;
            _router = router;
            _factoryCreator = factoryCreator;

            Token = token;
            Bot = new TelegramBotClient(token);
            BotInfo = Bot.GetMeAsync().Result;
            Bot.OnReceiveError += Bot_OnReceiveError;
            Bot.OnReceiveGeneralError += Bot_OnReceiveGeneralError;
            Bot.OnUpdate += _bot_OnUpdate;
        }

        private async void _bot_OnUpdate(object sender, Bot.Args.UpdateEventArgs e) {
            var context = new BotContext(null, _session, e.Update);
            try {
                await _router.Route(context, _factoryCreator.Invoke());
            }
            catch (Exception ex) {
                _logger.Log(ex, context.RouteData);
            }
        }

        private void Bot_OnReceiveGeneralError(object sender, Args.ReceiveGeneralErrorEventArgs e) {
            _logger.Log(e.Exception);
        }

        private void Bot_OnReceiveError(object sender, Args.ReceiveErrorEventArgs e) {
            _logger.Log(e.ApiRequestException);
        }

        public BotSession Start(params UpdateType[] updateTypes)   {
            if (_session == null) _session = new BotSession(Bot, _router, _logger, Token);
            Bot.SetWebhookAsync().Wait();
            Bot.StartReceiving(updateTypes);
            return _session;
        }

        public void Stop() {
            Bot.StopReceiving();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }
                Bot = null;
                _session = null;
                disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
