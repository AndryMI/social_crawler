using System;
using System.Threading;

namespace Core.Crawling
{
    public static class Crawler
    {
        private static readonly Random random = new Random();

        public static void Sleep(object context, string action)
        {
            Thread.Sleep(random.Next(1000, 3000));
        }
    }
}
