using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

        public int Count { get; private set; }
        public TimeSpan Duration { get; private set; }
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
            TrimRequests();

            var delta = requests.Count - Count;
            if (delta < 0)
            {
                return DateTimeOffset.Now;
            }
            lock (requests)
            {
                return requests.Skip(delta).FirstOrDefault() + Duration;
            }
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

        private void TrimRequests(List<DateTimeOffset> output = null)
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
            }
        }

        private class Json : JsonConverter<RequestLimits>
        {
            public override RequestLimits ReadJson(JsonReader reader, Type objectType, RequestLimits existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                if (existingValue != null)
                {
                    var since = (DateTimeOffset.Now - existingValue.Duration).ToUnixTimeSeconds();
                    var json = serializer.Deserialize<JObject>(reader);

                    existingValue.Count = (int)json["count"];
                    existingValue.Duration = TimeSpan.FromSeconds((int)json["duration"]);

                    foreach (var request in json["last_requests"].Values<long>())
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
                var json = new JObject();
                json["count"] = value.Count;
                json["duration"] = value.Duration.TotalSeconds;
                json["last_requests"] = new JArray(requests.Select(x => x.ToUnixTimeSeconds()));
                serializer.Serialize(writer, json);
            }
        }
    }
}
