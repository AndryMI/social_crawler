using System;

namespace Core
{
    public class TryLaterException : Exception
    {
        public readonly DateTimeOffset Time;

        public TryLaterException(string reason, DateTimeOffset time) : base(reason)
        {
            Time = time;
        }

        public TryLaterException(string reason, TimeSpan time) : base(reason)
        {
            Time = DateTimeOffset.Now + time;
        }
    }
}
