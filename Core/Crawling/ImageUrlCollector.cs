using Core.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Core.Crawling
{
    public class ImageUrlCollector : JsonConverter<ImageUrl>
    {
        private readonly List<ImageUrl> images = new List<ImageUrl>();
        private readonly BrowserNetwork network;

        public ImageUrlCollector(BrowserNetwork network)
        {
            this.network = network;
        }

        public void WaitForLoading()
        {
            var urls = images.Select(x => x.Original).ToList();
            while (!network.IsComplete(urls))
            {
                Thread.Sleep(1000);
            }
            foreach (var image in images)
            {
                image.MimeType = network.GetMimeType(image.Original);
                image.Data = network.GetResponseBody(image.Original);
            }
        }

        public override ImageUrl ReadJson(JsonReader reader, Type objectType, ImageUrl existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var result = new ImageUrl((string)reader.Value);
            images.Add(result);
            return result;
        }

        public override void WriteJson(JsonWriter writer, ImageUrl value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
