using System.Diagnostics;
using System.IO;

namespace Updater
{
    internal static class Program
    {
        public static void Main(string[] arguments)
        {
            var args = new Arguments(arguments);

            args.Print();

            var backup = Path.ChangeExtension(args.UpdatePath, ".bak.zip");
            var process = Process.GetProcessById(int.Parse(args.ProcessId));
            var entries = ZipUtils.Validate(args.UpdatePath);

            ZipUtils.Backup(backup, entries);

            process.WaitForExit();

            ZipUtils.TryExtract(args.UpdatePath, 10);

            Process.Start(new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = args.CrawlerPath,
                Arguments = args.CrawlerArgs,
            });
        }
    }
}
