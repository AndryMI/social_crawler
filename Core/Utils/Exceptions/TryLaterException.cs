using System;

namespace Core
{
    public class TryLaterException : Exception
    {
        public readonly DateTimeOffset Time;

        public TryLaterException(DateTimeOffset time)
        {
            Time = time;
        }
    }
}
