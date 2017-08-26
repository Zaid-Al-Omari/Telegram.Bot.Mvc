using System.Dynamic;
using Telegram.Bot.Types;

namespace Telegram.BotMVC.Framework {
    public class ChatSession {

        public ChatSession(ChatId chatId) {
            ChatId = chatId;
        }
        public ChatId ChatId { get; set; }
        public dynamic Bag { get; protected set; } = new ExpandoObject();

        public override bool Equals(object obj) {
            if (obj as ChatSession == null) return false;
            return (obj as ChatSession).ChatId == ChatId;
        }

        public override int GetHashCode() {
            return ChatId.GetHashCode();
        }
    }
}