# BirthdaysBot

This [Telegram bot](https://t.me/iCalBirthdaysBot) will send you daily notifications about your friends' birthdays.
Birthdays import is quite easy and convenient: just send a ICS file to the bot.

This bot is primarily useful for Google Calendar users — sadly it
has [no option to enable notifications for birthdays calendar](https://support.google.com/calendar/thread/1699815?hl=en&msgid=1714805).

## Where do I get the ICS file?

### [Google Calendar](https://support.google.com/calendar/answer/37111?hl=en)

1. Go to [Google Calendar's website](https://calendar.google.com/calendar) and log in
    - You have to use Desktop, mobile app does not support exporting calendars
2. Open Settings in the top right corner
3. Navigate to "Import & export" tab and click "Export"
4. You will get a ZIP archive containing separate ICS files for each of the calendars

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
5. Build the project, compiled code can be found in ``bin/Release/net5.0/``
    - ``dotnet build -c Release``
    - Make sure ``.env`` was copied to the folder with compiled code
6. Run your bot
    - ``dotnet bin/Release/net5.0/BirthdaysBot.dll``
7. ???
8. PROFIT

## (Possible) future plans

- [ ] Support adding birthdays from contacts' list
- [ ] Instructions on how to export calendars from more platforms
- [ ] Configure notifications time

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