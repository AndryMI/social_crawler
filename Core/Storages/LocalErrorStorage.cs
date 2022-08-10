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
            var error = new StringBuilder();
            for (var e = ex.InnerException; e != null; e = e.InnerException)
            {
                error.AppendLine(e.Message);
                error.AppendLine(e.StackTrace);
                error.AppendLine();
            }

            var path = $"{folder}/{DateTimeOffset.Now:yyyy-MM-dd_HH-mm}_{nextUid++}";
            Directory.CreateDirectory(path);

            File.WriteAllText(path + "/task.json", JsonConvert.SerializeObject(ex.Task, Formatting.Indented));
            File.WriteAllText(path + "/error.txt", error.ToString());

            if (ex.Html != null)
            {
                File.WriteAllText(path + "/page.html", ex.Html);
            }
            if (ex.Screenshot != null)
            {
                File.WriteAllBytes(path + "/screenshot.png", ex.Screenshot.AsByteArray);
            }
        }
    }
}
