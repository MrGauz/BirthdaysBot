using DotNetEnv;
using FluentScheduler;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;

namespace BirthdaysBot
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Load config
            var stream = File.OpenRead(".env");
            Env.Load(stream);
            var token = Env.GetString("TELEGRAM_BOT_TOKEN");
            stream.Close();

            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("Add Telegram bot's token to .env (refer to .env.example)");
                Environment.Exit(1);
            }

            // Fire up
            Bot.Initialize(token);
            Database.Initialize("BirthdaysBot");

            // Schedule daily messages
            Thread.CurrentThread.CurrentCulture = new CultureInfo("de-DE");
            var registry = new Registry();
            registry.Schedule(() =>
            {
                foreach (KeyValuePair<Int64, string> birthday in Database.GetTodaysBirthdays())
                {
                    var birthdayNotification = $"\ud83e\udd73 {birthday.Value} празднует сегодня День рождения. Не порти себе карму, поздравь человека.";
                    Bot.SendMessage(birthday.Key, birthdayNotification);
                }
            }).ToRunEvery(1).Days().At(10, 0);
            JobManager.Initialize(registry);

            // Manual break
            Console.WriteLine("Press any key to stop the bot");
            Console.ReadKey();
        }
    }
}
