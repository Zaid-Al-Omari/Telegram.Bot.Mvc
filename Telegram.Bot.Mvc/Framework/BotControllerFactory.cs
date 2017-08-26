using System;

namespace Telegram.Bot.Mvc.Framework {
    public class BotControllerFactory {

        public BotController Create<TController>(BotContext context) where TController : BotController, new() {
            return new TController() { Context = context };
        }

        public BotController Create(Type type, BotContext context) {
            if (type == null) throw new Exception("Controller Type Not Found!");
            var controller = Activator.CreateInstance(type) as BotController;
            if (controller == null) throw new Exception("Could Not Create Controller From Type!");
            controller.Context = context;
            return controller;
        }
    }
}