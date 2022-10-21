using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;

namespace Core
{
    public class UrlEncodedData : List<KeyValuePair<string, object>>, IRequestData
    {
        public string ContentType => "application/x-www-form-urlencoded";

        public void Serialize(StreamWriter writer)
        {
            writer.Write(ToString());
        }

        public void Add(string key, object value)
        {
            Add(new KeyValuePair<string, object>(key, value));
        }

        public override string ToString()
        {
            return string.Join("&", Encode(this));
        }

        public static string operator +(string url, UrlEncodedData data)
        {
            var query = data.ToString();

            if (string.IsNullOrEmpty(query))
            {
                return url;
            }
            return url.IndexOf('?') < 0 ? url + '?' + query : url + '&' + query;
        }

        private static IEnumerable<string> Encode(UrlEncodedData data)
        {
            return data.Where(p => p.Value != null).SelectMany(p => Encode(p.Key, JToken.FromObject(p.Value)));
        }

        private static string Encode(JValue value)
        {
            switch (value.Type)
            {
                case JTokenType.Float:
                case JTokenType.Integer:
                case JTokenType.Boolean:
                    return ((double)value).ToString(CultureInfo.InvariantCulture);
                default:
                    return HttpUtility.UrlEncode(value.ToString());
            }
        }

        private static IEnumerable<string> Encode(string key, JToken token)
        {
            if (token is JValue value)
            {
                if (value.Type != JTokenType.Null)
                {
                    yield return key + "=" + Encode(value);
                }
                yield break;
            }
            if (token is JArray array)
            {
                foreach (var item in array)
                {
                    foreach (var line in Encode(key + "[]", item))
                    {
                        yield return line;
                    }
                }
                yield break;
            }
            if (token is JObject obj)
            {
                foreach (var p in obj)
                {
                    foreach (var line in Encode(key + "[" + p.Key + "]", p.Value))
                    {
                        yield return line;
                    }
                }
                yield break;
            }
        }
    }
}