using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace RD_Team_TweetMonitor
{
    public class AuthorizedDriver : IDriver
    {
        private ChromeDriver driver;

        public TaskType Type => TaskType.Authorized;

        public ChromeDriver Instance()
        {
            if (driver == null)
            {
                var account = Config.Instance.RandomAccount();

                driver = new ChromeDriver();
                driver.Url = "https://twitter.com/login";
                driver.InitTimeouts();
                driver.WaitForLoading();

                var user = driver.FindElement(By.CssSelector("[autocomplete=username]"));
                user.Click();
                user.SendKeys(account.Mail + "\n");
                driver.WaitForLoading();

                var name = driver.TryFindElement(By.CssSelector("[data-testid=ocfEnterTextTextInput]"));
                if (name != null)
                {
                    name.Click();
                    name.SendKeys(account.Name + "\n");
                    driver.WaitForLoading();
                }

                var pass = driver.FindElement(By.CssSelector("[type=password]"));
                pass.Click();
                pass.SendKeys(account.Password + "\n");
                driver.WaitForLoading();
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
            if (driver != null && driver.Url != "data:,")
            {
                driver.Url = "data:,";
            }
        }
    }
}
