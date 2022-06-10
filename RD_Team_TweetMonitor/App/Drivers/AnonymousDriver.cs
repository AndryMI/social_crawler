using OpenQA.Selenium.Chrome;

namespace RD_Team_TweetMonitor
{
    public class AnonymousDriver : IDriver
    {
        private ChromeDriver driver;

        public TaskType Type => TaskType.Anonymous;

        public ChromeDriver Instance()
        {
            if (driver == null)
            {
                driver = new ChromeDriver();
            }
            return driver;
        }

        public void Destroy()
        {
            if (driver != null)
            {
                driver.Quit();
                driver = null;
            }
        }

        public void Suspend()
        {
            if (driver != null)
            {
                driver.Quit();
                driver = null;
            }
        }
    }
}
