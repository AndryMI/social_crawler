using Newtonsoft.Json;
using OpenQA.Selenium.Chrome;
using System.IO;

namespace RD_Team_TweetMonitor
{
    public class ProfileInfo
    {
        public string Link;

        public string Id;
        public string Name;
        public string Description;
        public string Location;
        public string Url;
        public string JoinDate;

        public string HeaderImg;
        public string PhotoImg;

        public string Following;
        public string Followers;

        public static ProfileInfo Collect(ChromeDriver driver)
        {
            var script = File.ReadAllText("Scripts/ProfileInfo.js");
            var json = driver.ExecuteScript(script) as string;
            return JsonConvert.DeserializeObject<ProfileInfo>(json);
        }
    }
}
