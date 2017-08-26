using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;

namespace Telegram.Bot.Mvc.Framework
{
    public class BotPathAttribute : Attribute {

        public string Path { get; protected set; }

        public UpdateType UpdateType { get; set; }

        public BotPathAttribute(string path, UpdateType updateType) {
            Path = path;
            UpdateType = updateType;
        }
    }

    public class AnyPathAttribute : Attribute {
        public UpdateType UpdateType { get; set; }
        public AnyPathAttribute(UpdateType updateType) {
            UpdateType = updateType;
        }
    }
}
