using Core.Crawling;
using Core.Data;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;

namespace Core.Storages
{
    public class DebugStorage : IStorage
    {
        public void StoreProfile(CrawlerTask task, IProfileInfo data)
        {
            Debug.WriteLine($"--- {data.Link} ---");
            Debug.WriteLine(JsonConvert.SerializeObject(data));
        }

        public void StorePost(CrawlerTask task, IPostInfo data)
        {
            Debug.WriteLine($"--- {data.Link} ---");
            Debug.WriteLine(JsonConvert.SerializeObject(data));
        }

        public void StoreComment(CrawlerTask task, ICommentInfo data)
        {
            Debug.WriteLine($"--- {data.Link} ---");
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
