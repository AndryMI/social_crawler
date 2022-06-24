using System;
using System.Collections.Generic;

namespace Core.Crawling
{
    public delegate CrawlerTask TaskCtor(string url, string priority);

    public class TaskFactory : Dictionary<string, TaskCtor>
    {
        public CrawlerTask CreateTask(string url, string priority)
        {
            foreach (var p in this)
            {
                if (url.StartsWith(p.Key))
                {
                    return p.Value(url, priority);
                }
            }
            throw new NotImplementedException();
        }
    }
}
