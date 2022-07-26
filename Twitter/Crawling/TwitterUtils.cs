using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Twitter
{
    public static class TwitterUtils
    {
        /// <summary>Converts follow-count string to int (eg 12.3K to 12300)</summary>
        public static int ParseFollow(string line)
        {
            var number = Regex.Replace(line, @"[^0-9.]+", "");
            if (!double.TryParse(number, NumberStyles.Float, CultureInfo.InvariantCulture, out var count))
            {
                return -1;
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

        /// <summary>Extracts profile part from tweet link (eg twitter.com/some_profile from twitter.com/some_profile/status/...)</summary>
        public static string ExtractProfileLink(string link)
        {
            return Regex.Replace(link, "/status/.*", "");
        }
    }
}
