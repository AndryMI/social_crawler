using OpenQA.Selenium.DevTools.V85;
using OpenQA.Selenium.DevTools.V85.Console;
using Serilog.Events;

namespace Core.Browsers.DevTools.V85
{
    public class ConsoleV85 : Console
    {
        private readonly DevToolsSessionDomains domains;

        public ConsoleV85(V85Domains domain)
        {
            domains = (DevToolsSessionDomains)domain.VersionSpecificDomains;
            domains.Console.Enable(new EnableCommandSettings()).Wait();
            domains.Console.MessageAdded += OnMessageAddedImpl;
        }

        public override void Dispose()
        {
            domains.Console.MessageAdded -= OnMessageAddedImpl;
        }

        private void OnMessageAddedImpl(object sender, MessageAddedEventArgs e)
        {
            OnMessageAdded?.Invoke(GetLevel(e.Message.Level), new Message
            {
                Source = e.Message.Source.ToString(),
                Level = e.Message.Level.ToString(),
                Text = e.Message.Text,
                Url = e.Message.Url,
                Line = e.Message.Line,
                Column = e.Message.Column,
            });
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
