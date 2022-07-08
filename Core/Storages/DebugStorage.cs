using Core.Crawling;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Text;

namespace Core.Storages
{
    public class DebugStorage : IStorage
    {
        public void StoreData(CrawlerTask task, Uri url, object data)
        {
            Debug.WriteLine($"--- {url.Host}{url.LocalPath} ---");
            Debug.WriteLine(JsonConvert.SerializeObject(data));
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
            Debug.WriteLine(error.ToString());
            Debugger.Break();
        }
    }
}
