using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Mvc.Core;
using Telegram.Bot.Types;

namespace Telegram.Bot.Mvc.Framework
{
    public class BotSession {

        public string Username => BotInfo.Username;
        public User BotInfo { get; protected set; }

        public ILogger Logger { get; protected set; }
        public IBotRouter Router { get; protected set; }
        public ITelegramBotClient Bot { get; protected set; }

        public string Token { get; protected set; }

        public IDictionary<ChatId, ChatSession> ChatSessions { get; protected set; } = new Dictionary<ChatId, ChatSession>();

        public BotSession(ITelegramBotClient client, IBotRouter router, ILogger logger, string token) {
            Bot = client;
            Logger = logger;
            Router = router;
            Token = token;
            Clear();
            BotInfo = client.GetMeAsync().Result;
        }

        public async Task RegisterCertificate(string certificatePath, string webHookPath)
        {
            using (var stream = new System.IO.FileStream(certificatePath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                await Bot.SetWebhookAsync(webHookPath, new FileToSend("cer.pem", stream));
            }
        }


        public void Clear()
        {
            _bag = new Dictionary<string, object>();
        }

        private IDictionary<string, object> _bag;

        public object this[string key]
        {
            get
            {
                _bag.TryGetValue(key, out object result);
                return result;
            }
            set
            {
                _bag[key] = value;
            }
        }
    }
}
