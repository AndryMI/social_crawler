using Newtonsoft.Json;
using OpenQA.Selenium.Chrome;
using System.IO;

namespace Twitter.Data
{
    public class FollowersInfo
    {
        public string Link;

        public static FollowersInfo[] Collect(ChromeDriver driver)
        {
            var script = File.ReadAllText("Scripts/Twitter/FollowersInfo.js");
            var json = driver.ExecuteScript(script) as string;
            return JsonConvert.DeserializeObject<FollowersInfo[]>(json);
        }
    }
}
