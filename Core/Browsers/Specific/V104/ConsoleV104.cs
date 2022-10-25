﻿using OpenQA.Selenium.DevTools.V104;
using OpenQA.Selenium.DevTools.V104.Console;
using Serilog.Events;

namespace Core.Browsers.Specific.V104
{
    public class ConsoleV104 : Console
    {
        private readonly DevToolsSessionDomains domains;

        public ConsoleV104(V104Domains domain)
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
