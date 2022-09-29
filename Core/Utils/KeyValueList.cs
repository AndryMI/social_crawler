using System.Collections.Generic;

namespace Core.Data
{
    public class KeyValueList<TKey, TValue> : List<KeyValuePair<TKey, TValue>>
    {
        public TValue TryGet(params TKey[] keys)
        {
            foreach (var key in keys)
            {
                foreach (var p in this)
                {
                    if (Equals(p.Key, key))
                    {
                        return p.Value;
                    }
                }
            }
            return default;
        }
    }
}
