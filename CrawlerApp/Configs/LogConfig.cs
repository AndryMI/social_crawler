using Core.Crawling;
using CrawlerApp.Utils;
using Serilog;
using Serilog.Filters;
using Serilog.Formatting.Compact;

public class LogConfig
{
    public LogConfig()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.File(new CompactJsonFormatter(), "Logs/.gz", rollingInterval: RollingInterval.Day, hooks: new GZipHooks())
            .WriteTo.Logger(log => log
                .Filter.ByExcluding(Matching.FromSource<BrowserNetwork>())
                .WriteTo.Console()
                .WriteTo.Debug()
            )
            .Enrich.FromLogContext()
            .Enrich.WithThreadId()
            .Enrich.WithThreadName()
            .CreateLogger();
    }
}