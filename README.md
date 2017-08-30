# Telegram.Bot.Mvc

[![Build status](https://ci.appveyor.com/api/projects/status/xntyrljf5rdjsmf4?svg=true)](https://ci.appveyor.com/project/Zaid-Al-Omari/telegram-bot-mvc)

* Just like Asp.net MVC.
* Establish command routes. (`/start`,  `/help`,  etc.)
* Create bot controllers.
* Automatic parameters binding.
* Works both for **webhooks** and **standalone**.
* Handle user data in sessions.
* Supports multi-tenancy.
* Throttle outgoing requests using the scheduler. No more *429: Too Many Requests* :)

## Getting Started

Install the [package](https://www.nuget.org/packages/Telegram.Bot.Mvc) into your project using Nuget Package Manager.
```
Install-Package Telegram.Bot.Mvc
```
Prerequisites

```
.netcore2.0
```

Working As Standalone Console Application (Pull)
------------------------------------------------

```
private static void Main(string[] args)
{
    var listener = new BotListener("<token here>", new Logger());
    listener.Start();
    Console.WriteLine("BOT STARTED: " + listener.BotInfo.Username);
    Console.ReadLine();
}
```

And then create a controller that inherits `BotController`...

```
public class HelloController : BotController
{
    [BotPath("/start", UpdateType.MessageUpdate)]
    public async Task Start()
    {
        await Bot.SendTextMessageAsync(Chat.Id, "Welcome!");
    }

    [AnyPath(UpdateType.MessageUpdate)]
    public async Task Echo()
    {
        await Bot.SendTextMessageAsync(Chat.Id, Message.Text);
    }
}
```
