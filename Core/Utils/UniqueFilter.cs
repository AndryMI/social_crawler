using System;
using System.Collections.Generic;

namespace Core
{
    public class UniqueFilter<T>
    {
        private readonly int maxSize;
        private readonly Func<T, string> getId;

        private readonly List<string> order = new List<string>();
        private readonly HashSet<string> hash = new HashSet<string>();

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
