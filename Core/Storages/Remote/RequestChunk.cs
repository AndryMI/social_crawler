using Core.Crawling;
using Core.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Text;

namespace Core.Storages.Remote
{
    public class RequestChunk
    {
        public readonly string name;
        public readonly string file;
        public readonly string type;
        public readonly byte[] data;

        public RequestChunk(CrawlerTask task, IProfileInfo data) : this(task, "profiles[]", data) { }
        public RequestChunk(CrawlerTask task, IPostInfo data) : this(task, "posts[]", data) { }
        public RequestChunk(CrawlerTask task, ICommentInfo data) : this(task, "comments[]", data) { }

        private RequestChunk(CrawlerTask task, string name, object data)
        {
            var json = JsonConvert.SerializeObject(new Json { task = task, data = data });
            this.data = Encoding.UTF8.GetBytes(json);
            this.name = name;
        }

        public RequestChunk(ImageUrl image)
        {
            name = "images[]";
            file = image.Stored;
            type = image.MimeType;
            data = image.Data;
        }

        [JsonConverter(typeof(Json))]
        private class Json : JsonConverter<Json>
        {
            public CrawlerTask task;
            public object data;

            public override Json ReadJson(JsonReader reader, Type objectType, Json existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }

            public override void WriteJson(JsonWriter writer, Json value, JsonSerializer serializer)
            {
                var json = JToken.FromObject(value.data);
                json["CommandId"] = value.task.Command.Id;
                serializer.Serialize(writer, json);
            }
        }
    }
}
