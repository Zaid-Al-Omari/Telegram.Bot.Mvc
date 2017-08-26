﻿using System;
using System.Collections.Generic;
using Telegram.Bot.Mvc.Framework;
using System.Configuration;
namespace Telegram.Bot.Mvc.StandAlone
{
    class Program
    {
        // For running bots that are multi-tenants uncomment ...
        //private static Dictionary<string, BotListener> Listeners = new Dictionary<string, BotListener>();
        private static Logger Logger = new Logger();
        private static void Main(string[] args)
        {
            while (true)
            {

                var token = "356143438:AAG4zU9PGgB1_9Az_gWMnsPvSoL5QW2f3Fo";
                // Starting ...
                var listener = new BotListener(token, Logger);
                listener.Start();
                //Listeners.Add(token, listener);

                Console.WriteLine("BOT STARTED: " + listener.BotInfo.Username);
                Console.ReadLine();
                // Stopping ...  
                listener.Stop();
                //Listeners.Clear();
            }
        }
    }
}