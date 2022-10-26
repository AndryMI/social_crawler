using Serilog.Events;
using System;

namespace Core.Browsers.DevTools
{
    public abstract class Console : DevTools<Console>, IDisposable
    {
        public delegate void MessageAdded(LogEventLevel level, Message message);

        public MessageAdded OnMessageAdded;

        public abstract void Dispose();

        public class Message
        {
            public string Source { get; set; }
            public string Level { get; set; }
            public string Text { get; set; }
            public string Url { get; set; }
            public long? Line { get; set; }
            public long? Column { get; set; }
        }
    }
}
