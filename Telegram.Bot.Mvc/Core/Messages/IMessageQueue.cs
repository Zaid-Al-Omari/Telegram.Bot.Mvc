using System;
using System.Collections.Generic;
using System.Text;

namespace Telegram.Bot.Mvc.Core.Messages
{
    public interface IMessageQueue
    {
        void EnqueueMessage(IMessage message);
    }
}
