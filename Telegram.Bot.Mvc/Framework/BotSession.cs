using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Telegram.BotMVC.Framework
{
    public class BotSession {

        public string Username => BotInfo.Username;
        public User BotInfo { get; protected set; }


        public BotRouter Router { get; protected set; }
        public ITelegramBotClient Bot { get; protected set; }


        public IDictionary<ChatId, ChatSession> ChatSessions { get; protected set; } = new Dictionary<ChatId, ChatSession>();

        public dynamic BotBag { get; protected set; } = new ExpandoObject();

        public BotSession(ITelegramBotClient client, BotRouter router) {
            Bot = client;
            BotInfo = client.GetMeAsync().Result;
            Router = router;
        }
    }
}
