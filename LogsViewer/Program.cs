using LogsViewer.Logs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.

var log = new AppRuns();
log.StartNew();
log.AddLines(LogLines.PlainFile(@"C:\Work\RD_Team_Crawler\CrawlerApp\bin\20221031"));

var tasks = log.runs.SelectMany(x => x.crawlers.Values).SelectMany(x => x.tasks).ToDictionary(x => x.uid, x => x.lines);

app.MapGet("/task/{uid}", (int uid) =>
{
    return tasks.TryGetValue(uid, out var task) ? task : null;
});

app.MapGet("/task-list", () =>
{
    return log.runs.Select(run => new
    {
        run.start,
        tasks = run.crawlers.Values.SelectMany(crawler => crawler.tasks).OrderBy(task => task.start).Select(task => new
        {
            task.threadId,
            task.uid,
            task.type,
            task.start,
            task.status,
            lines = task.lines.Count,
        })
    });
});

app.MapGet("/{file}", (HttpContext context, string file) =>
{
    if (File.Exists("Html/" + file))
    {
        context.Response.ContentType = "text/html";
        return File.ReadAllText("Html/" + file);
    }
    return null;
});

app.Run();