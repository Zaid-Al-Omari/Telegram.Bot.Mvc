using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Telegram.Bot.Mvc.Core.Messages
{
    [JsonObject(MemberSerialization.OptIn)]
    public class TextMessage : IMessage
    {

        [JsonProperty(PropertyName = "Token")]
        public string Token { get; protected set; }

        public TextMessage(string token)
        {
            Token = token;
        }

        [JsonProperty(PropertyName = "ChatId")]
        public string ChatId { get; set; }

        [JsonProperty(PropertyName = "Text")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "ReplyMarkup")]
        public IReplyMarkup ReplyMarkup { get; set; }

        [JsonProperty(PropertyName = "ParseMode")]
        public ParseMode ParseMode { get; set; }

        [JsonProperty(PropertyName = "DisableWebPagePreview")]
        public bool DisableWebPagePreview { get; set; }

        [JsonProperty(PropertyName = "ReplyToMessageId")]
        public int ReplyToMessageId { get; set; }

        [JsonProperty(PropertyName = "DisableNotification")]
        public bool DisableNotification { get; set; }
    }
}
