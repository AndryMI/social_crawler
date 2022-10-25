using Core.Browsers.Specific;
using OpenQA.Selenium.Chrome;
using Serilog;
using Serilog.Events;
using IDisposable = System.IDisposable;

namespace Core.Crawling
{
    public class BrowserConsole : IDisposable
    {
        private readonly ILogger log;
        private readonly Console console;

        public BrowserConsole(ChromeDriver driver)
        {
            log = Log.Logger.ForContext<BrowserConsole>().ForContext("SessionId", driver.SessionId);
            console = Console.Create(driver);
            console.OnMessageAdded = OnMessageAdded;
        }

        public void Dispose()
        {
            console.Dispose();
        }

        private void OnMessageAdded(LogEventLevel level, Console.Message message)
        {
            log.Write(level, "{@ConsoleMessage}", message);
        }
    }
}
