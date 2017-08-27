using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using Telegram.Bot.Types;

namespace Telegram.Bot.Mvc.Framework {
    public class ChatSession {

        public ChatSession(ChatId chatId) {
            ChatId = chatId;
            Clear();
        }
        public ChatId ChatId { get; set; }
        

        public void Clear()
        {
            _bag = new Dictionary<string, object>();
        }

        public override bool Equals(object obj) {
            if (obj as ChatSession == null) return false;
            return (obj as ChatSession).ChatId == ChatId;
        }

        public override int GetHashCode() {
            return ChatId.GetHashCode();
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