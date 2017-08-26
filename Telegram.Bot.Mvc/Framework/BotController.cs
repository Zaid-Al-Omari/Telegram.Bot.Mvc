﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Mvc.Core;
using Telegram.Bot.Mvc.Scheduler;

namespace Telegram.Bot.Mvc.Framework {
    public class BotController {


        public BotContext Context { get; set; }
        public PerSecondScheduler Scheduler { get; set; }

        private ChatSession _chatSession;
        public ChatSession ChatSession
        {
            get
            {
                if (_chatSession == null)
                {
                    if (Context.BotSession.ChatSessions.ContainsKey(Chat.Id))
                    {
                        _chatSession = Context.BotSession.ChatSessions[Chat.Id];
                    }
                    else
                    {
                        _chatSession = new ChatSession(Chat.Id);
                        Context.BotSession.ChatSessions[Chat.Id] = _chatSession;
                    }
                }
                return _chatSession;
            }
        }

        public Update Update => Context.Update;
        public Message Message => Context.Update.Message;
        public CallbackQuery Query => Context.Update.CallbackQuery;
        public ILogger Logger => Context.BotSession.Logger;
        public ITelegramBotClient Bot => Context.Bot;


        private Chat _chat;
        public Chat Chat
        {
            get
            {
                if(_chat == null) {
                    if (Message != null) _chat = Message.Chat;
                    if (Query != null) _chat = Query.Message.Chat;
                }
                return _chat;
            }
        }

        private User _user;
        public User User
        {
            get
            {
                if (_user == null) {
                    if (Message != null) _user = Message.From;
                    if (Query != null) _user = Query.From;
                }
                return _user;
            }
        }
    }
}