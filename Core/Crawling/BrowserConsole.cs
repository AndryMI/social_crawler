using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools.V106.Console;
using OpenQA.Selenium.DevTools.V106;
using System;
using Serilog;
using Serilog.Events;

namespace Core.Crawling
{
    public class BrowserConsole : IDisposable
    {
        private readonly ILogger log;
        private readonly DevToolsSessionDomains domains;

        public BrowserConsole(ChromeDriver driver)
        {
            log = Log.Logger.ForContext<BrowserConsole>().ForContext("SessionId", driver.SessionId);
            domains = driver.GetDevToolsSession().GetVersionSpecificDomains<DevToolsSessionDomains>();
            domains.Console.Enable(new EnableCommandSettings()).Wait();
            domains.Console.MessageAdded += OnMessageAdded;
        }

        public void Dispose()
        {
            domains.Console.MessageAdded -= OnMessageAdded;
        }

        private void OnMessageAdded(object sender, MessageAddedEventArgs e)
        {
            log.Write(GetLevel(e.Message.Level), "{@ConsoleMessage}", e.Message);
        }

        private static LogEventLevel GetLevel(ConsoleMessageLevelValues level)
        {
            switch (level)
            {
                default:
                case ConsoleMessageLevelValues.Log: return LogEventLevel.Verbose;
                case ConsoleMessageLevelValues.Warning: return LogEventLevel.Warning;
                case ConsoleMessageLevelValues.Error: return LogEventLevel.Error;
                case ConsoleMessageLevelValues.Debug: return LogEventLevel.Debug;
                case ConsoleMessageLevelValues.Info: return LogEventLevel.Information;
            }
        }
    }
}
