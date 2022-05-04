# BirthdaysBot

This [Telegram bot](https://t.me/iCalBirthdaysBot) is a brilliant little example of how a simple iCal parser will make you a better human being. Seriously.

Ok, if we're being *serious* serious, this bot will actually just send you notifications about your friends' birthdays. Which, in part, will make you a better human being, *if* you decide to act on the notification.

## Why, even?

Primarily, because [Google Calendar users can't enable notifications for Birthdays calendar](https://support.google.com/calendar/thread/1699815?hl=en&msgid=1714805). Don't ask yourself why a multi-trillion dollar corporation doesn't give this option to its users. You will not know.

## How do I use the bot?

Quite easily. Just send an `.ics` file to the bot.

Keep in mind: the bot itself provides no interface for interacting with Birthday information. If you want to change something, be it someone's name, date of birth *(who knows, you do you)*, or you want to add new Birhdays, do the changes in the calendar.

First, change or add info in the calendar. Then export the calendar. Then re-upload the `.ics` file to the bot.

## How do I get the ICS file?

You have a couple of options.

### [Google Calendar](https://support.google.com/calendar/answer/37111?hl=en)
1. Go to [Google Calendar's website](https://calendar.google.com/calendar) and log in.
    - Use the desktop version; mobile app does not support exporting calendars
2. Open Settings in the top right corner.
3. Navigate to "Import & export" tab and click "Export".
4. You will get a ZIP archive containing separate `.ics` files for each of the calendars

### macOS native Calendar.App
1. In the Calendar List, pick a calendar you'd like to export.
2. File --> Export --> Export...
3. Export. Done.

## Wanna host this bot yourself?

> BirthdaysBot imports birthdays into a local SQLite file to schedule notifications
>
> This data will be kept **private and secure**, never given to third parties or used for any other purposes than notificating you
>
> However, if you don't feel comfortable entrusting me with this data — host your own bot

1. Clone this repository
    - ``git clone https://github.com/MrGauz/BirthdaysBot.git``
2. Create a Telegram bot using [@BotFather](https://t.me/BotFather)
    - After bot is registered you'll get an API token, use it in the next step
    - **Keep API token a secret!**
3. Make a copy of ``.env.example`` and name it ``.env``
    - Put the API token into the ``.env``
4. [Install .NET Runtime 5.0](https://dotnet.microsoft.com/en-us/download/dotnet/5.0)
    - Alternatively, you can use the [.NET Runtime 6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
5. Build the project, compiled code can be found in ``bin/Release/net5.0/``
    - ``dotnet build -c Release``
    - Make sure ``.env`` was copied to the folder with compiled code
6. Run your bot
    - ``dotnet bin/Release/net5.0/BirthdaysBot.dll``
7. ???
8. PROFIT

## (Possible) future plans

Let's be frank: it's open source Telegram bot. Most likely it'll be surving it's purpose for as long as it should, but it won't become a multi-million dollar startup.

However, there are a couple of ideas that might be implemented in the future. Or not. See the paragraph above.

- [ ] Support adding birthdays from contacts' list
- [ ] Instructions on how to export calendars from more platforms
- [ ] Configure notifications time
- [ ] Let users add/remove Birthdays right from the Telegram bot UI without importing `.ics`
- [ ] Turn this into a bank

## MIT License

**TL;DR**: do whatever you want, I'm not liable for anything

```
MIT License

Copyright (c) 2022 Kirill Gringauz

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```
---
This `README.md` has been written together with [Daniel Bilyk](https://github.com/danielbilyk). Go say hello to him if you want your project's story to be told in a way that wants to be read.