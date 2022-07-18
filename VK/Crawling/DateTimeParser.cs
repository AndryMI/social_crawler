using System;
using System.Text.RegularExpressions;

namespace VK.Crawling
{
    public static class DateTimeParser
    {
        private static readonly string[] Numbers = new[] {
            "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve"
        };

        public static DateTimeOffset? Parse(string time, DateTimeOffset createdAt)
        {
            var result = createdAt.ToUniversalTime();

            time = time.ToLower();
            if (time.Contains("now"))
            {
                return result;
            }
            if (time.EndsWith("ago"))
            {
                for (var i = 0; i < Numbers.Length; i++)
                {
                    time = time.Replace(Numbers[i], i.ToString());
                }
                var matches = Regex.Matches(time, @"(\d+)\s+(hour|minute)s?");
                foreach (Match match in matches)
                {
                    if (match.Groups[2].Value == "hour")
                    {
                        result = result.AddHours(-int.Parse(match.Groups[1].Value));
                    }
                    if (match.Groups[2].Value == "minute")
                    {
                        result = result.AddMinutes(-int.Parse(match.Groups[1].Value));
                    }
                }
                return result;
            }
            time = time.Replace("at", result.Year.ToString());
            time = time.Replace("today", result.ToString("dd MMM"));
            time = time.Replace("yesterday", result.AddDays(-1).ToString("dd MMM"));

            if (DateTimeOffset.TryParse(time, out result))
            {
                return result.ToUniversalTime();
            }
            return null;
        }

        public static string Convert(string time, DateTimeOffset createdAt)
        {
            return Parse(time, createdAt)?.ToString("yyyy-MM-ddTHH:mm:ss.000Z");
        }
    }
}
