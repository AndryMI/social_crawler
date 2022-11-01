using Newtonsoft.Json.Linq;

namespace LogsViewer.Logs
{
    public class AppRun
    {
        public Dictionary<int, CrawlerThread> crawlers = new Dictionary<int, CrawlerThread>();
        public Dictionary<string, CrawlerThread> sessions = new Dictionary<string, CrawlerThread>();
        public Dictionary<int, List<string>> threads = new Dictionary<int, List<string>>();

        public long start;

        public void AddLine(string line, JObject json)
        {
            var threadId = json["ThreadId"].Value<int>();
            var message = json["@mt"]?.ToString();

            if (start == default)
            {
                start = DateTimeOffset.Parse(json["@t"].ToString()).ToUnixTimeMilliseconds();
            }

            var name = json["ThreadName"]?.ToString();
            if (name == "Core.Crawling.CrawlerThread")
            {
                if (!crawlers.TryGetValue(threadId, out var crawler))
                {
                    crawlers.Add(threadId, crawler = new CrawlerThread(threadId));
                }
                if (message.StartsWith("Begin Task"))
                {
                    crawler.NewTask(json);
                }
                if (message.StartsWith("Net Init"))
                {
                    sessions.Add(json["SessionId"].ToString(), crawler);
                }
                if (message.StartsWith("Task"))
                {
                    if (crawler.task != null)
                    {
                        crawler.task.status = message;
                    }
                }

                crawler.AddLine(line);
                return;
            }

            var sessionId = json["SessionId"]?.ToString();
            if (sessionId != null)
            {
                if (sessions.TryGetValue(sessionId, out var crawler))
                {
                    crawler.AddLine(line);
                    return;
                }
            }

            if (!threads.TryGetValue(threadId, out var thread))
            {
                threads.Add(threadId, thread = new List<string>());
            }
            thread.Add(line);
        }
    }
}