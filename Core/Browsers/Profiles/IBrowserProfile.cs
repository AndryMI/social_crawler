using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;

namespace Core.Browsers.Profiles
{
    public interface IBrowserProfile
    {
        [JsonProperty(Required = Required.Always)]
        string Type { get; }

        [JsonProperty]
        string Id { get; }

        ChromeDriver Start();
    }

    public class BrowserProfileJson : JsonConverter<IBrowserProfile>
    {
        private static readonly Dictionary<string, Type> types = new Dictionary<string, Type>
        {
            { "Anonymous", typeof(AnonymousProfile) },
            { "Anty", typeof(AntyProfile) },
            { "Chrome", typeof(ChromeProfile) },
        };

        public override IBrowserProfile ReadJson(JsonReader reader, Type objectType, IBrowserProfile existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var json = serializer.Deserialize<JObject>(reader);
            var type = types[json["Type"].ToString()];
            return (IBrowserProfile)json.ToObject(type);
        }

        public override void WriteJson(JsonWriter writer, IBrowserProfile value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}
