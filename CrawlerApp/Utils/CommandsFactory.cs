using Core.Crawling;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;

namespace CrawlerApp
{
    public class CommandsFactory : JsonConverter<ICommand>
    {
        private readonly Dictionary<string, Type> types = new Dictionary<string, Type>();

        public void Register<T>(string type) where T : ICommand
        {
            types.Add(type, typeof(T));
        }

        public IEnumerable<string> Types => types.Keys;

        public override ICommand ReadJson(JsonReader reader, Type objectType, ICommand existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            try
            {
                var json = serializer.Deserialize<JObject>(reader);
                var type = types[json["type"].ToString()];
                if (json["_id"] is JObject)
                {
                    json["_id"] = json["_id"]["$oid"];
                }
                return (ICommand)json.ToObject(type);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Failed to deserialize command");
                return null;
            }
        }

        public override void WriteJson(JsonWriter writer, ICommand value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
