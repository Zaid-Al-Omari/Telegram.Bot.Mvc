using System;
using System.Collections.Generic;
using Telegram.Bot.Mvc.Framework;

namespace Telegram.Bot.Mvc.Core
{
    public interface IBotControllerFactory
    {
        BotController Create(Type type, BotContext context);
        BotController Create<TController>(BotContext context) where TController : BotController, new();
        IEnumerable<Type> GetControllers();
    }
}