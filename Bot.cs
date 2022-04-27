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
                    var chatId = message.Chat.Id;

                    // Welcome new users
                    if (message.Text.StartsWith("/start"))
                    {
                        _ = bot.SendChatActionAsync(chatId, ChatAction.Typing, CancellationToken.None);
                        var welcomeMessage = "ALLO YOBA\n" +
                                             "ETO TI???";
                        var sentMessage = await bot.SendTextMessageAsync(chatId, welcomeMessage, ParseMode.Html, cancellationToken: CancellationToken.None);
                        _ = bot.PinChatMessageAsync(chatId, sentMessage.MessageId, true, CancellationToken.None);
                    }

                    // Handle sent files
                    if (message.Document != null || (message.ReplyToMessage != null && message.ReplyToMessage.Document != null))
                    {
                        var document = message.Document ?? message.ReplyToMessage.Document;

                        // Handle iCal files
                        if (document.MimeType.ToLower() == "text/calendar")
                        {
                            _ = bot.SendChatActionAsync(chatId, ChatAction.Typing, CancellationToken.None);

                            // Download sent file
                            var randomChars = new string(Enumerable.Repeat("69420", 5).Select(s => s[new Random().Next(s.Length)]).ToArray());
                            var tmpFilename = Path.Combine(Path.GetTempPath(), randomChars + document.FileName);

                            using FileStream calendarStream = System.IO.File.Open(tmpFilename, FileMode.Create);
                            await bot.GetInfoAndDownloadFileAsync(document.FileId, calendarStream, CancellationToken.None);
                            calendarStream.Close();

                            // Delete old birthday entries
                            Database.DeleteUserBirthdays(chatId);

                            // Add birthdays to database
                            var birthdays = IcalParser.Parse(tmpFilename);
                            foreach (KeyValuePair<string, DateTime> birthday in birthdays)
                            {
                                Database.NewBirthday(chatId, birthday.Key, birthday.Value.Month, birthday.Value.Day);
                            }

                            // Send confirmation
                            var properDeclension = birthdays.Count switch
                            {
                                1 => "День рождения",
                                > 1 and <= 4 => "Дня рождения",
                                _ => "Дней рождений"
                            };
                            _ = bot.SendTextMessageAsync(chatId, $"Я нашёл, импортировал и сохранил {birthdays.Count} {properDeclension}.",
                                cancellationToken: CancellationToken.None);

                            // Delete temp file
                            System.IO.File.Delete(tmpFilename);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _ = HandleErrorAsync(BotClient, e, cancellationToken);
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
