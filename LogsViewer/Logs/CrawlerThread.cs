using Newtonsoft.Json.Linq;

namespace LogsViewer.Logs
{
    public class CrawlerThread
    {
        public List<CrawlerTask> tasks = new List<CrawlerTask>();
        public CrawlerTask task;

        public readonly int threadId;

        public CrawlerThread(int threadId)
        {
            this.threadId = threadId;
        }

        public void NewTask(JObject json)
        {
            tasks.Add(task = new CrawlerTask(threadId, json));
        }

        public void AddLine(string line)
        {
            task?.lines.Add(line);
        }
    }
}
