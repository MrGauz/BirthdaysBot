using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BirthdaysBot
{
    public class Bot
    {
        private static ITelegramBotClient BotClient;

        public static void Initialize(string token)
        {
            BotClient = new TelegramBotClient(token);
            BotClient.StartReceiving(new DefaultUpdateHandler(HandleUpdateAsync, HandleErrorAsync));
        }

        private static async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
        {
            try
            {
                if (update.Type == UpdateType.Message)
                {
                    var message = update.Message;

                    // Handle sent files
                    if (message.Document != null || (message.ReplyToMessage != null && message.ReplyToMessage.Document != null))
                    {
                        var document = message.Document ?? message.ReplyToMessage.Document;

                        // Handle iCal files
                        if (document.MimeType.ToLower() == "text/calendar")
                        {
                            // Delete old birthday entries
                            Database.DeleteUserBirthdays(message.Chat.Id);

                            // Download sent file
                            var randomChars = new string(Enumerable.Repeat("69420", 5).Select(s => s[new Random().Next(s.Length)]).ToArray());
                            var tmpFilename = Path.Combine(Path.GetTempPath(), randomChars + document.FileName);

                            using FileStream calendarStream = System.IO.File.Open(tmpFilename, FileMode.Create);
                            await bot.GetInfoAndDownloadFileAsync(document.FileId, calendarStream);
                            calendarStream.Close();

                            foreach (KeyValuePair<string, DateTime> birthday in IcalParser.Parse(tmpFilename))
                            {
                                Database.NewBirthday(message.Chat.Id, birthday.Key, birthday.Value.Month, birthday.Value.Day);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                await HandleErrorAsync(BotClient, e, cancellationToken);
            }
        }

        private static Task HandleErrorAsync(ITelegramBotClient bot, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine($"{DateTime.Now:HH:mm:ss} - {ErrorMessage}");
            return Task.CompletedTask;
        }

        public static Task SendMessage(Int64 chatId, string message)
        {
            return BotClient.SendTextMessageAsync(chatId, message);
        }
    }
}
