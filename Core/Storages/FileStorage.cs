using Core.Crawling;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace Core.Storages
{
    public class FileStorage : IStorage
    {
        private readonly string folder;
        private long nextUid = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        public FileStorage(string folder)
        {
            this.folder = folder;
        }

        public void StoreData(CrawlerTask task, Uri url, object data)
        {
            var path = $"{folder}/{url.Host}{url.LocalPath}";
            var file = $"/{data.GetType()}.{nextUid++}.json";
            Directory.CreateDirectory(path);
            File.WriteAllText(path + file, JsonConvert.SerializeObject(data, Formatting.Indented));
        }

        public void StoreException(CrawlingException ex)
        {
            var error = new StringBuilder();
            for (var e = ex.InnerException; e != null; e = e.InnerException)
            {
                error.AppendLine(e.Message);
                error.AppendLine(e.StackTrace);
                error.AppendLine();
            }

            var path = $"{folder}/Errors/{nextUid++}";
            Directory.CreateDirectory(path);

            File.WriteAllText(path + "/task.json", JsonConvert.SerializeObject(ex.Task, Formatting.Indented));
            File.WriteAllText(path + "/page.html", ex.Html);
            File.WriteAllText(path + "/error.txt", error.ToString());
            File.WriteAllBytes(path + "/screenshot.png", ex.Screenshot.AsByteArray);
        }
    }
}
