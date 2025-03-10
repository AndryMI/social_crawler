﻿using Core.Data;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;

namespace Core.Crawling
{
    public class ImageUrlCollector : JsonConverter<ImageUrl>
    {
        private readonly List<ImageUrl> images = new List<ImageUrl>();
        private readonly BrowserNetwork network;
        private readonly NetworkCache cache;

        public ImageUrlCollector(BrowserNetwork network, NetworkCache cache)
        {
            this.network = network;
            this.cache = cache;
        }

        public List<ImageUrl>.Enumerator GetEnumerator()
        {
            return images.GetEnumerator();
        }

        public void WaitForLoading()
        {
            var urls = images.Select(x => x.Original).ToList();
            for (var i = 0; !network.IsComplete(urls) && i < Config.Instance.WaitTimeout; i++)
            {
                Thread.Sleep(1000);
            }
            foreach (var image in images)
            {
                var response = cache.Get(image.Original, url => network.GetResponse(url) ?? DirectDownload(url));
                if (response != null)
                {
                    image.MimeType = response.MimeType;
                    image.Data = response.Data;
                }
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

        private static NetworkResponse DirectDownload(string url)
        {
            try
            {
                Log.Verbose("Try direct download: {url}", url);
                using (var client = new WebClient())
                {
                    return new NetworkResponse(client.DownloadData(url), client.ResponseHeaders.Get("Content-Type"));
                }
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Direct download failed");
                return null;
            }
        }
    }
}
