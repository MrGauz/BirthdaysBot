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
                        var welcomeMessage = "\ud83d\udc4b\ud83c\udffc\n\n" + 
                                             "Привет и добро пожаловать!\n\n" + 
                                             "Для начала: я нужен далеко не каждому. Больше всего я буду полезен пользователям Google Calendar: Google <a href=\"https://support.google.com/calendar/thread/1699815?hl=en&msgid=1714805\">не даёт возможности включить уведомления для календаря Дней рождений</a>.\n\n" + 
                                             "<b>Как мною пользоваться?</b>\n" + 
                                             "Пришли мне <code class='code-inline'>.ics</code>-файл и чуточку подожди. Я его обработаю и дам тебе знать, когда загружу все Дни рождения.\n\n" + 
                                             "Если ты не знаешь, откуда взять <code class='code-inline'>.ics</code>-файл — посмотри чуть ниже в раздел “Откуда взять <code class='code-inline'>.ics</code>-файл?”\n\n" + 
                                             "Если ты захочешь добавить новые Дни рождения или у кого-то поменялась дата рождения на свет (¯\\_(ツ)_/¯), просто скорми мне ещё один файл. Я его обработаю и перепишу все Дни рождения у себя в памяти.\n\n" + 
                                             "По хорошему, это всё, что тебе нужно знать о том, что со мной делать. Если у тебя есть вопросы и/или пожелания — напиши @mrGauz или @danielbilyk. Если ты хакерман — можешь посмотреть на <a href=\"https://github.com/MrGauz/BirthdaysBot\">Github</a>.\n\n" + 
                                             "Покамест я попробую ответить на короткий FAQ.\n\n" + 
                                             "<b>Откуда взять .ics-файл?</b>\n" + 
                                             "Для этого тебе нужен компьютер.\n\n" + 
                                             "· Пойди в свой <a href=\"https://calendar.google.com/\">Google-календарь</a>;\n" + 
                                             "· Найди “My Calendars” слева;\n" + 
                                             "· Найди календарь, который хочешь экспортировать. Нажми на три вертикальных точки —> <b>Settings and sharing</b>;\n" + 
                                             "· Нажми <b>Export calendar</b>;\n" + 
                                             "· Вуаля. Этот файл потом нужно скинуть мне и будет класс.\n\n" + 
                                             "<b>А если я маковод?</b>\n" + 
                                             "Для маководов, пользующихся родным приложением календаря, есть ещё один способ:\n\n" + 
                                             "· Выбери нужный календарь в списке календарей слева;\n" + 
                                             "· File —> Export —> Export…\n" + 
                                             "· Вуаля. Этот файл потом нужно скинуть мне и будет класс.\n\n" + 
                                             "<b>Как мне добавить Дни рождения?</b>\n" + 
                                             "Чтобы добавить или удалить День рождения или поменять имя, тебе нужно поправить данные в самом календаре, а мне скормить новый <code class='code-inline'>.ics</code>-файл. Сделай нужные изменения, загрузи сюда календарный файл и всё будет.";
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
                            _ = bot.SendTextMessageAsync(chatId, $"Готово! Я нашёл, импортировал и сохранил {birthdays.Count} {properDeclension}.\n\nЯ напомню тебе обо всём в нужный момент. Однако, пеняй на себя, если у тебя выключены от меня уведомления.",
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
