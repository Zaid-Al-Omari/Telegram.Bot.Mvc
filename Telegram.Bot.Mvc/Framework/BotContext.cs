using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Mvc.Core;

namespace Telegram.Bot.Mvc.Framework {
    public class BotContext {

        public BotContext(
            BotRouteData routeData,
            BotSession session, Update update) {
            BotSession = session;
            Update = update;
            RouteData = routeData;
        }

        public ITelegramBotClient Bot => BotSession.Bot;

        public Update Update { get; protected set; }

        public BotSession BotSession { get; protected set; }

        public BotRouteData RouteData { get; internal set; }




        private Chat _chat;
        public Chat Chat
        {
            get
            {
                if (_chat == null)
                {
                    switch (Update.Type)
                    {
                        case Types.Enums.UpdateType.MessageUpdate:
                            _chat = Update.Message.Chat;
                            break;
                        case Types.Enums.UpdateType.CallbackQueryUpdate:
                            _chat = Update.CallbackQuery.Message.Chat;
                            break;
                        case Types.Enums.UpdateType.EditedMessage:
                            _chat = Update.EditedMessage.Chat;
                            break;
                        case Types.Enums.UpdateType.ChannelPost:
                            _chat = Update.ChannelPost.Chat;
                            break;
                        case Types.Enums.UpdateType.EditedChannelPost:
                            _chat = Update.EditedChannelPost.Chat;
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
                if (_user == null)
                {
                    switch (Update.Type)
                    {
                        case Types.Enums.UpdateType.MessageUpdate:
                            _user = Update.Message.From;
                            break;
                        case Types.Enums.UpdateType.CallbackQueryUpdate:
                            _user = Update.CallbackQuery.From;
                            break;
                        case Types.Enums.UpdateType.EditedMessage:
                            _user = Update.EditedMessage.From;
                            break;
                        case Types.Enums.UpdateType.ChannelPost:
                            _user = Update.ChannelPost.From;
                            break;
                        case Types.Enums.UpdateType.EditedChannelPost:
                            _user = Update.EditedChannelPost.From;
                            break;
                        case Types.Enums.UpdateType.InlineQueryUpdate:
                            _user = Update.InlineQuery.From;
                            break;
                        case Types.Enums.UpdateType.ChosenInlineResultUpdate:
                            _user = Update.ChosenInlineResult.From;
                            break;
                        case Types.Enums.UpdateType.PreCheckoutQueryUpdate:
                            _user = Update.PreCheckoutQuery.From;
                            break;
                        case Types.Enums.UpdateType.ShippingQueryUpdate:
                            _user = Update.ShippingQuery.From;
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