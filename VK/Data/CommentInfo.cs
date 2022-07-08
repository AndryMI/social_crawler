using Core.Utils;
using OpenQA.Selenium.Chrome;
using System;

namespace VK.Data
{
    public class CommentInfo
    {
        public string Link;

        public DateTimeOffset CreatedAt = DateTimeOffset.UtcNow;

        public static CommentInfo[] Collect(ChromeDriver driver)
        {
            return driver.RunCollector<CommentInfo[]>("Scripts/VK/CommentInfo.js");
        }
    }
}
