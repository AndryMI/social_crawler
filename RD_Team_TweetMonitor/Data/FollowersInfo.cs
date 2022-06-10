using Newtonsoft.Json;
using OpenQA.Selenium.Chrome;
using System.IO;

namespace RD_Team_TweetMonitor
{
    public class FollowersInfo
    {
        public string Link;

        public static FollowersInfo[] Collect(ChromeDriver driver)
        {
            var script = File.ReadAllText("Scripts/FollowersInfo.js");
            var json = driver.ExecuteScript(script) as string;
            return JsonConvert.DeserializeObject<FollowersInfo[]>(json);
        }
    }
}
