using System;

namespace Core
{
    public class TryLaterException : Exception
    {
        public readonly DateTimeOffset Time;

        public TryLaterException(string reason, Exception exception, DateTimeOffset time) : base(reason, exception)
        {
            Time = time;
        }

        public TryLaterException(string reason, Exception exception, TimeSpan time) : this(reason, exception, DateTimeOffset.Now + time)
        {
        }

        public TryLaterException(string reason, DateTimeOffset time) : this(reason, null, time)
        {
        }

        public TryLaterException(string reason, TimeSpan time) : this(reason, null, DateTimeOffset.Now + time)
        {
        }

        public TryLaterException(string reason, Exception exception) : this(reason, exception, DateTimeOffset.Now)
        {
        }

        public TryLaterException(string reason) : this(reason, DateTimeOffset.Now)
        {
        }
    }
}
