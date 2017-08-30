using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Mvc.Core;
using Telegram.Bot.Mvc.Framework;
using Telegram.Bot.Types.Enums;

namespace Telegram.Bot.Mvc.StandAlone.Controllers
{
    public class HelloController : BotController
    {
        [BotPath("/start", UpdateType.MessageUpdate)]
        public async Task Start()
        {
            Console.WriteLine(User.Username + " Joined The Channel!");
            await Bot.SendTextMessageAsync(Chat.Id, "Welcome!");
        }

        [AnyPath(UpdateType.MessageUpdate)]
        public async Task Echo()
        {
            Console.WriteLine("Message: (" + Message.Text +  ")\nReceived From (" + User.Username + ")");
            await Bot.SendTextMessageAsync(Chat.Id, Message.Text);
        }

        [BotPath("/test", UpdateType.MessageUpdate)]
        public Task TestScheduler()
        {
            Console.WriteLine(User.Username + " TestScheduler!");
            var actions = new List<Action>();
            for(int i = 0; i < 5; i++)
            {
                int localIndex = i;
                var chatId = Chat.Id;
                actions.Add(async () =>
                {
                    await Bot.SendTextMessageAsync(chatId, "Welcome " + localIndex + "!");
                });
            }
            Scheduler.Enqueue(delay: 1000, priority: 0, actions: actions.ToArray());
            return Task.FromResult(0);
        }
    }
}
