using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace BirthdaysBot
{
    public class IcalParser
    {
        public static Dictionary<string, DateTime> Parse(string filename)
        {
            // Parse iCalendar
            var iCalRaw = System.IO.File.ReadAllText(filename);
            var processedSummaries = new Dictionary<string, DateTime>();

            while (iCalRaw.IndexOf("BEGIN:VEVENT") != -1)
            {
                // Get individual event
                var startIndex = iCalRaw.IndexOf("BEGIN:VEVENT") + 12;
                var length = iCalRaw.IndexOf("END:VEVENT") - startIndex;
                var bdayEvent = iCalRaw.Substring(startIndex, length).Trim();

                // Parse summary and date
                var text = string.Empty;
                var birthdayDate = DateTime.Now;
                foreach (string line in bdayEvent.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries & StringSplitOptions.TrimEntries))
                {
                    if (line.StartsWith("DTSTART;VALUE=DATE:"))
                    {
                        birthdayDate = DateTime.ParseExact(line[19..], "yyyyMMdd", CultureInfo.InvariantCulture);
                    }
                    else if (line.StartsWith("SUMMARY:"))
                    {
                        text = line[8..];

                        // Google Calendar saves SUMMARY as "<contact_name>'s birthday"
                        // We only want the contact's name
                        if (text.Contains("'s birthday"))
                        {
                            text = text.Replace("'s birthday", "");
                        }

                    }
                }

                // Save birthday
                if (!processedSummaries.Keys.Contains(text))
                {
                    processedSummaries[text] = birthdayDate;
                }

                iCalRaw = iCalRaw[(startIndex + 10 + length)..];
            }

            return processedSummaries;
        }
    }
}
