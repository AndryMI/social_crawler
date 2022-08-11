using Serilog.Events;
using Serilog.Formatting;
using System.IO;
using System.IO.Compression;

namespace CrawlerApp.Utils
{
    public class GZipFormatter : ITextFormatter
    {
        private readonly ITextFormatter formatter;

        public GZipFormatter(ITextFormatter formatter)
        {
            this.formatter = formatter;
        }

        public void Format(LogEvent logEvent, TextWriter output)
        {
            var writer = (StreamWriter)output;
            using (var gzip = new GZipStream(writer.BaseStream, CompressionLevel.Fastest, true))
            using (var text = new StreamWriter(gzip))
            {
                formatter.Format(logEvent, text);
            }
        }
    }
}
