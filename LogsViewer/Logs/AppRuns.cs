using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace LogsViewer.Logs
{
    public class AppRuns
    {
        public List<AppRun> runs = new List<AppRun>();
        public AppRun run = null;

        public void StartNew()
        {
            runs.Add(run = new AppRun());
        }

        public void AddLines(IEnumerable<string> lines)
        {
            foreach (var line in lines)
            {
                var json = JsonConvert.DeserializeObject<JObject>(line);
                if (json["@mt"].ToString() == "Logger initialized")
                {
                    StartNew();
                }
                run.AddLine(line, json);
            }
        }
    }
}
