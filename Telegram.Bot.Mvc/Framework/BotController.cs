using System.Collections.Generic;
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
                    switch (Update.Type)
                    {
                        case Types.Enums.UpdateType.MessageUpdate:
                            _chat = Context.Update.Message.Chat;
                            break;
                        case Types.Enums.UpdateType.CallbackQueryUpdate:
                            _chat = Context.Update.CallbackQuery.Message.Chat;
                            break;
                        case Types.Enums.UpdateType.EditedMessage:
                            _chat = Context.Update.EditedMessage.Chat;
                            break;
                        case Types.Enums.UpdateType.ChannelPost:
                            _chat = Context.Update.ChannelPost.Chat;
                            break;
                        case Types.Enums.UpdateType.EditedChannelPost:
                            _chat = Context.Update.EditedChannelPost.Chat;
                            break;
                        default:
                            break;
                    }
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
                    switch (Update.Type)
                    {
                        case Types.Enums.UpdateType.MessageUpdate:
                            _user = Context.Update.Message.From;
                            break;
                        case Types.Enums.UpdateType.CallbackQueryUpdate:
                            _user = Context.Update.CallbackQuery.From;
                            break;
                        case Types.Enums.UpdateType.EditedMessage:
                            _user = Context.Update.EditedMessage.From;
                            break;
                        case Types.Enums.UpdateType.ChannelPost:
                            _user = Context.Update.ChannelPost.From;
                            break;
                        case Types.Enums.UpdateType.EditedChannelPost:
                            _user = Context.Update.EditedChannelPost.From;
                            break;
                        case Types.Enums.UpdateType.InlineQueryUpdate:
                            _user = Context.Update.InlineQuery.From;
                            break;
                        case Types.Enums.UpdateType.ChosenInlineResultUpdate:
                            _user = Context.Update.ChosenInlineResult.From;
                            break;
                        case Types.Enums.UpdateType.PreCheckoutQueryUpdate:
                            _user = Context.Update.PreCheckoutQuery.From;
                            break;
                        case Types.Enums.UpdateType.ShippingQueryUpdate:
                            _user = Context.Update.ShippingQuery.From;
                            break;
                        default:
                            break;
                    }
                }
                return _user;
            }
        }
    }
}