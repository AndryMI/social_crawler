using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace LogsViewer.Logs
{
    public class CrawlerTask
    {
        private static int nextUid = 1;

        public List<string> lines = new List<string>();

        public readonly int uid = nextUid++;
        public readonly int threadId;
        public readonly long start;
        public readonly string type;
        public string status = "N/A";

        public CrawlerTask(int threadId, JObject line)
        {
            this.threadId = threadId;
            var task = JsonConvert.DeserializeObject<JObject>(line["task"].ToString());
            start = DateTimeOffset.Parse(line["@t"].ToString()).ToUnixTimeMilliseconds();
            type = task["Command"]["type"] + " " + task["Type"].ToString();
        }

        public override string ToString()
        {
            return $"{type} --- Status: {status} --- Lines: {lines.Count}";
        }
    }
}
