using Core.Data;
using Core.Utils;
using OpenQA.Selenium.Chrome;
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Twitter.Data
{
    public class ProfileInfo : IProfileInfo
    {
        public string Social => "twitter";
        public string Link { get; set; }

        public string Id;
        public string Name;
        public string Description;
        public string Location;
        public string Url;
        public string JoinDate;

        public string HeaderImg;
        public string PhotoImg;

        public string RawFollowing;
        public string RawFollowers;

        public int Following => ParseFollow(RawFollowing);
        public int Followers => ParseFollow(RawFollowers);

        public DateTimeOffset CreatedAt = DateTimeOffset.UtcNow;

        public static ProfileInfo Collect(ChromeDriver driver)
        {
            return driver.RunCollector<ProfileInfo>("Scripts/Twitter/ProfileInfo.js");
        }

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
    }
}
