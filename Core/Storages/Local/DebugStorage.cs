using Core.Crawling;
using Core.Data;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Text;

namespace Core.Storages
{
    public class DebugStorage : IDataStorage, IErrorStorage
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
            WriteException(ex.InnerException);
            Debugger.Break();
        }

        public void StoreMultipart(Exception ex, MultipartData data)
        {
            WriteException(ex);
            Debugger.Break();
        }

        private static void WriteException(Exception e)
        {
            var error = new StringBuilder();
            while (e != null)
            {
                error.AppendLine(e.Message);
                error.AppendLine(e.StackTrace);
                error.AppendLine();
                e = e.InnerException;
            }
            Debug.WriteLine(error.ToString());
        }
    }
}
