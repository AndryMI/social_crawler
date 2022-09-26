using System.Collections.Generic;

namespace Facebook.Crawling
{
    public static class FacebookUtils
    {
        public static string TryGetInfo(this List<KeyValuePair<string, string>> info, params string[] keys)
        {
            foreach (var key in keys)
            {
                foreach (var p in info)
                {
                    if (p.Key == key)
                    {
                        return p.Value;
                    }
                }
            }
            return null;
        }

        /// <summary>Converts count string to int (eg 12.3K to 12300)</summary>
        public static int ParseCount(string line)
        {
            if (int.TryParse(line, out var count))
            {
                return count;
            }
            //TODO other options
            return 0;
        }
    }
}
