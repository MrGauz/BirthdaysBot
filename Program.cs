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
#if DEBUG
            var stream = File.OpenRead(".env");
#else
            var stream = File.OpenRead(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "BirthdaysBot", ".env"));
#endif
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
                    Bot.SendMessage(birthday.Key, birthday.Value);
                }
            }).ToRunEvery(1).Days().At(10, 0);
            JobManager.Initialize(registry);

            // Manual break
            Console.WriteLine("Press any key to stop the bot");
            Console.ReadKey();
        }
    }
}
