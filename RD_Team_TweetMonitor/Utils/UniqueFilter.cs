using System;
using System.Collections.Generic;

namespace RD_Team_TweetMonitor
{
    public class UniqueFilter<T>
    {
        private int maxSize;
        private Func<T, string> getId;

        private List<string> order = new List<string>();
        private HashSet<string> hash = new HashSet<string>();

        public UniqueFilter(int maxSize, Func<T, string> getId)
        {
            this.maxSize = maxSize;
            this.getId = getId;
        }

        public T[] Filter(T[] items)
        {
            var result = new List<T>();
            foreach (var item in items)
            {
                var id = getId(item);
                if (hash.Add(id))
                {
                    order.Add(id);
                    result.Add(item);
                }
            }

            var remove = order.Count - maxSize;
            if (remove > 0)
            {
                for (var i = 0; i < remove; i++)
                {
                    hash.Remove(order[i]);
                }
                order.RemoveRange(0, remove);
            }
            return result.ToArray();
        }
    }
}
