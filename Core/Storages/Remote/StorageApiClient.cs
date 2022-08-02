using Core.Crawling;
using Core.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Core.Storages
{
    public class StorageApiClient : ApiServerClient
    {
        public void StoreProfiles(List<Args<IProfileInfo>> data)
        {
            if (data.Count > 0)
            {
                Request("POST", "/crawler/profiles", data);
            }
        }

        public void StorePosts(List<Args<IPostInfo>> data)
        {
            if (data.Count > 0)
            {
                Request("POST", "/crawler/posts", data);
            }
        }

        public void StoreComments(List<Args<ICommentInfo>> data)
        {
            if (data.Count > 0)
            {
                Request("POST", "/crawler/comments", data);
            }
        }

        [JsonConverter(typeof(ArgsConverter))]
        public class Args
        {
            public CrawlerTask task;
            public object data;
        }

        public class Args<T> : Args
        {
            public new T data
            {
                get => (T)base.data;
                set => base.data = value;
            }
        }

        public class ArgsConverter : JsonConverter<Args>
        {
            public override Args ReadJson(JsonReader reader, Type objectType, Args existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }

            public override void WriteJson(JsonWriter writer, Args value, JsonSerializer serializer)
            {
                var json = JToken.FromObject(value.data);
                json["CommandId"] = value.task.Command.Id;
                serializer.Serialize(writer, json);
            }
        }
    }
}
