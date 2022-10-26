using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Core.Crawling
{
    [JsonConverter(typeof(Json))]
    [DebuggerDisplay("{Count} / {Duration}")]
    public class RequestLimits : IRequestCounter
    {
        private readonly Queue<DateTimeOffset> requests = new Queue<DateTimeOffset>();

        public readonly int Count;
        public readonly TimeSpan Duration;
        public readonly Predicate<string> Matcher;

        public RequestLimits(int count, TimeSpan duration, Predicate<string> matcher)
        {
            Count = count;
            Duration = duration;
            Matcher = matcher;
        }

        public int GetAvailableCount()
        {
            TrimRequests();
            return Count - requests.Count;
        }

        public DateTimeOffset GetAvailableTime()
        {
            var first = TrimRequests();
            return requests.Count < Count ? DateTimeOffset.Now : first + Duration;
        }

        public void OnRequest(string url)
        {
            if (Matcher(url))
            {
                lock (requests)
                {
                    requests.Enqueue(DateTimeOffset.Now);
                }
            }
        }

        private DateTimeOffset TrimRequests(List<DateTimeOffset> output = null)
        {
            lock (requests)
            {
                var since = DateTimeOffset.Now - Duration;
                while (requests.Count > 0 && requests.Peek() < since)
                {
                    requests.Dequeue();
                }
                if (output != null)
                {
                    output.AddRange(requests);
                }
                return requests.Count > 0 ? requests.Peek() : default;
            }
        }

        private class Json : JsonConverter<RequestLimits>
        {
            public override RequestLimits ReadJson(JsonReader reader, Type objectType, RequestLimits existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                if (existingValue != null)
                {
                    var since = (DateTimeOffset.Now - existingValue.Duration).ToUnixTimeSeconds();
                    var requests = serializer.Deserialize<long[]>(reader);
                    foreach (var request in requests)
                    {
                        if (request > since)
                        {
                            existingValue.requests.Enqueue(DateTimeOffset.FromUnixTimeSeconds(request));
                        }
                    }
                }
                return existingValue;
            }

            public override void WriteJson(JsonWriter writer, RequestLimits value, JsonSerializer serializer)
            {
                var requests = new List<DateTimeOffset>();
                value.TrimRequests(requests);
                serializer.Serialize(writer, requests.Select(x => x.ToUnixTimeSeconds()));
            }
        }
    }
}
