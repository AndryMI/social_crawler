using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Core.Crawling
{
    public static class NumberUtils
    {
        //TODO add more localizations to K or M
        private static readonly string[] LocalizeK = new[] { "тыс" };
        private static readonly string[] LocalizeM = new[] { "млн" };

        /// <summary>Converts count string to int (eg "4.5K likes" to 4500)</summary>
        public static int ParseCount(string line)
        {
            if (string.IsNullOrEmpty(line))
            {
                return 0;
            }
            line = line.ToLower();
            foreach (var word in LocalizeK)
            {
                line = line.Replace(word, "k");
            }
            foreach (var word in LocalizeM)
            {
                line = line.Replace(word, "m");
            }
            foreach (Match match in Regex.Matches(line, @"[\d\s,.]*[\dmk]"))
            {
                var number = TryParseCount(Normalize(match.Value));
                if (number.HasValue)
                {
                    return number.Value;
                }
            }
            return 0;
        }

        private static string Normalize(string line)
        {
            var match = Regex.Match(line, @"(.*)[,.]([\d\s][\d\s]?[mk])$");
            if (match.Success)
            {
                var a = Regex.Replace(match.Groups[1].Value, @"[^\d]+", "");
                var b = Regex.Replace(match.Groups[2].Value, @"\s+", "");
                return a + "." + b;
            }
            match = Regex.Match(line, @"\d+[mk]$");
            if (match.Success)
            {
                return match.Value;
            }
            return Regex.Replace(line, @"[^\d]+", "");
        }

        /// <summary>Converts count string to int (eg 12.3K to 12300)</summary>
        private static int? TryParseCount(string line)
        {
            if (string.IsNullOrEmpty(line))
            {
                return null;
            }
            var number = Regex.Replace(line, @"[^0-9.]+", "");
            if (!double.TryParse(number, NumberStyles.Float, CultureInfo.InvariantCulture, out var count))
            {
                return null;
            }
            if (line.EndsWith("k", StringComparison.InvariantCultureIgnoreCase))
            {
                return (int)(count * 1000);
            }
            if (line.EndsWith("m", StringComparison.InvariantCultureIgnoreCase))
            {
                return (int)(count * 1000000);
            }
            return (int)count;
        }
    }
}
