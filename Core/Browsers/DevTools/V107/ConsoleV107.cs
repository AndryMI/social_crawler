using OpenQA.Selenium.DevTools.V107;
using OpenQA.Selenium.DevTools.V107.Console;
using Serilog.Events;

namespace Core.Browsers.DevTools.V107
{
    public class ConsoleV107 : Console
    {
        private readonly DevToolsSessionDomains domains;

        public ConsoleV107(V107Domains domain)
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
