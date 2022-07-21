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
            thread = new Thread(Run);
            thread.Start();
            threads.Add(this);
        }

        public void Stop(int timeout = -1)
        {
            working = false;
            thread.Join(timeout);
        }

        public static void StopAll()
        {
            foreach (var thread in threads)
            {
                thread.Stop(0);
            }
            foreach (var thread in threads)
            {
                thread.Stop();
            }
            threads.Clear();
        }
    }
}
