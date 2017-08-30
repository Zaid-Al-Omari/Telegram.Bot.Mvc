using System;
using Telegram.Bot.Mvc.Core;
using Telegram.Bot.Mvc.Scheduler;

namespace Telegram.Bot.Mvc.Framework {
    public class BotControllerFactory : IBotControllerFactory
    {
        private ISchedualer _scheduler;
        public BotControllerFactory(ISchedualer scheduler)
        {
            _scheduler = scheduler;
        }
        public BotController Create<TController>(BotContext context) where TController : BotController, new() {
            return new TController() {
                Context = context,
                Scheduler = _scheduler
            };
        }

        public BotController Create(Type type, BotContext context) {
            if (type == null) throw new Exception("Controller Type Not Found!");
            var controller = Activator.CreateInstance(type) as BotController;
            if (controller == null) throw new Exception("Could Not Create Controller From Type!");
            controller.Context = context;
            controller.Scheduler = _scheduler;
            return controller;
        }
    }
}