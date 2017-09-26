using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Mvc.Core;
using Telegram.Bot.Mvc.Scheduler;
using System;

namespace Telegram.Bot.Mvc.Framework {
    public abstract class BotController : IDisposable {


        public BotContext Context { get; set; }
        public ISchedualer Scheduler { get;  set; }

        private ChatSession _chatSession;
        public ChatSession ChatSession
        {
            get
            {
                if (_chatSession == null)
                {
                    if (Context.BotSession.ChatSessions.ContainsKey(Context.Chat.Id))
                    {
                        _chatSession = Context.BotSession.ChatSessions[Context.Chat.Id];
                    }
                    else
                    {
                        _chatSession = new ChatSession(Context.Chat.Id);
                        Context.BotSession.ChatSessions[Context.Chat.Id] = _chatSession;
                    }
                }
                return _chatSession;
            }
        }

        public User User => Context.User;
        public Chat Chat => Context.Chat;

        public Update Update => Context.Update;
        public Message Message => Context.Update.Message;
        public CallbackQuery Query => Context.Update.CallbackQuery;
        public ILogger Logger => Context.BotSession.Logger;
        public ITelegramBotClient Bot => Context.Bot;

        public abstract void Dispose();
    }
}