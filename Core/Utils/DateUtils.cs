using System;
using System.Text.RegularExpressions;

namespace Core.Crawling
{
    public static class DateUtils
    {
        public static string TryParseDate(string line)
        {
            if (string.IsNullOrEmpty(line))
            {
                return null;
            }
            var time = default(TimeSpan);
            var date = default(DateTimeOffset);

            var tm = Regex.Match(line, @"\d?\d:\d\d(:\d\d)?");
            if (tm.Success)
            {
                TimeSpan.TryParse(tm.Value, out time);
                line = line.Remove(tm.Index, tm.Length);
            }

            var numeric = Regex.Replace(line, "[^0-9./-]+", "");
            if (DateTimeOffset.TryParse(numeric, out date))
            {
                return (date + time).ToString("yyyy-MM-ddTHH:mm:ss.000Z");
            }

            line = ' ' + line + ' ';
            var ym = Regex.Match(line, @"\D(\d\d\d\d)\D");
            var mm = Regex.Match(line, @"\W(jan|feb|mar|apr|may|jun|jul|aug|sep|oct|nov|dec)", RegexOptions.IgnoreCase);
            var dm = Regex.Match(line, @"\D(\d\d?)\D");

            if (ym.Success && mm.Success)
            {
                line = (dm.Success ? dm.Groups[1].Value : null) + " " + mm.Groups[1].Value + " " + ym.Groups[1].Value;
                DateTimeOffset.TryParse(line, out date);
                return (date + time).ToString("yyyy-MM-ddTHH:mm:ss.000Z");
            }
            return null;
        }
    }
}
