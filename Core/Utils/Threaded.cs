using System;
using System.Collections.Generic;
using System.Threading;

namespace Core
{
    public abstract class Threaded
    {
        private static readonly List<Threaded> threads = new List<Threaded>();

        private readonly Thread thread;
        private volatile bool working = true;

        protected bool IsWorking => working;
        protected abstract void Run();

        protected Threaded()
        {
            thread = new Thread(Run) { Name = GetType().ToString() };
            thread.Start();
            threads.Add(this);
        }

        protected void LongSleep(TimeSpan time)
        {
            for (var ms = (int)time.TotalMilliseconds; working && ms > 0; ms -= 1000)
            {
                Thread.Sleep(Math.Min(ms, 1000));
            }
        }

        public void Stop(int timeout = -1)
        {
            working = false;
            thread.Join(timeout);
        }

        public static void StopAll()
        {
            StopAll<Threaded>();
        }

        public static void StopAll<T>() where T : Threaded
        {
            foreach (var thread in threads)
            {
                if (thread is T)
                {
                    thread.Stop(0);
                }
            }
            foreach (var thread in threads)
            {
                if (thread is T)
                {
                    thread.Stop();
                }
            }
            threads.RemoveAll(thread => thread is T);
        }
    }
}
