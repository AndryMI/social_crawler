using Core.Crawling;
using Core.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Text;

namespace Core.Storages
{
    public abstract class MultipartStorage : IDataStorage, IMediaStorage
    {
        private readonly UniqueMediaFilter unique = new UniqueMediaFilter();
        private MultipartData data = new MultipartData();

        protected abstract void Enqueue(MultipartData data);
        protected abstract bool Dequeue(out MultipartData data);

        public void Flush()
        {
            lock (data)
            {
                Enqueue(data);
                data = new MultipartData();
            }
        }

        public bool TryDequeue(out MultipartData result)
        {
            if (!Dequeue(out result))
            {
                if (data.TotalCount == 0)
                {
                    return false;
                }
                lock (data)
                {
                    result = data;
                    data = new MultipartData();
                }
            }
            return true;
        }

        public bool WaitForBrowserLoading => true;

        public void StoreProfile(CrawlerTask task, IProfileInfo data)
        {
            StoreChunk("profiles[]", JsonConvert.SerializeObject(new Json { task = task, data = data }));
        }

        public void StorePost(CrawlerTask task, IPostInfo data)
        {
            StoreChunk("posts[]", JsonConvert.SerializeObject(new Json { task = task, data = data }));
        }

        public void StoreComment(CrawlerTask task, ICommentInfo data)
        {
            StoreChunk("comments[]", JsonConvert.SerializeObject(new Json { task = task, data = data }));
        }

        public void StoreRelation(CrawlerTask task, IRelationInfo data)
        {
            StoreChunk("relations[]", JsonConvert.SerializeObject(new Json { task = task, data = data }));
        }

        public void StoreImage(ImageUrl image)
        {
            if (unique.NeedStore(image))
            {
                StoreChunk("images[]", image.Data, image.Stored, image.MimeType);
            }
        }

        private void StoreChunk(string name, string value)
        {
            StoreChunk(name, Encoding.UTF8.GetBytes(value), null, null);
        }

        private void StoreChunk(string name, byte[] bytes, string file, string type)
        {
            lock (data)
            {
                data.Add(name, bytes, file, type);

                if (data.TotalCount >= Config.Instance.MultipartVarsThreshold ||
                    data.FilesCount >= Config.Instance.MultipartFilesThreshold ||
                    data.Size >= Config.Instance.MultipartSizeThreshold)
                {
                    Enqueue(data);
                    data = new MultipartData();
                }
            }
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
