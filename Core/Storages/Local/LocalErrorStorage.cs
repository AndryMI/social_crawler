using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace Core.Storages
{
    public class LocalErrorStorage : IErrorStorage
    {
        private readonly string folder;
        private int nextUid = 0;

        public LocalErrorStorage(string folder)
        {
            this.folder = folder;
        }

        public void StoreException(CrawlingException ex)
        {
            var path = CreateNextDirectory();

            File.WriteAllText(path + "/task.json", JsonConvert.SerializeObject(ex.Task, Formatting.Indented));
            File.WriteAllText(path + "/error.txt", GetDetails(ex.InnerException));

            if (ex.Html != null)
            {
                File.WriteAllText(path + "/page.html", ex.Html);
            }
            if (ex.Screenshot != null)
            {
                File.WriteAllBytes(path + "/screenshot.png", ex.Screenshot.AsByteArray);
            }
        }

        public void StoreMultipart(Exception ex, MultipartData data)
        {
            var path = CreateNextDirectory();
            File.WriteAllText(path + "/error.txt", GetDetails(ex));
            data.Save(path + "/data.bin");
        }

        private string CreateNextDirectory()
        {
            var path = $"{folder}/{DateTimeOffset.Now:yyyy-MM-dd_HH-mm}_{nextUid++}";
            Directory.CreateDirectory(path);
            return path;
        }

        private static string GetDetails(Exception e)
        {
            var error = new StringBuilder();
            while (e != null)
            {
                error.AppendLine(e.Message);
                error.AppendLine(e.StackTrace);
                error.AppendLine();
                e = e.InnerException;
            }
            return error.ToString();
        }
    }
}
