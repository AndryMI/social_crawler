using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Instagram.Crawling
{
    public static class InstagramUtils
    {
        /// <summary>Converts count string to int (eg 12.3K to 12300)</summary>
        public static int ParseCount(string line)
        {
            if (string.IsNullOrEmpty(line))
            {
                return 0;
            }
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
    }
}
