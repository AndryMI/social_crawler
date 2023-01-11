using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading;

namespace Updater
{
    internal static class ZipUtils
    {
        public static List<string> Validate(string path)
        {
            var validated = new List<string>();

            using (var file = File.OpenRead(path))
            using (var zip = new ZipArchive(file, ZipArchiveMode.Read))
            {
                foreach (var entry in zip.Entries)
                {
                    using (var stream = entry.Open())
                    {
                        stream.CopyTo(Stream.Null);
                    }
                    validated.Add(entry.FullName);
                }
            }
            return validated;
        }

        public static void Backup(string path, List<string> entries)
        {
            using (var backup = File.OpenWrite(path))
            using (var zip = new ZipArchive(backup, ZipArchiveMode.Create))
            {
                foreach (var entry in entries)
                {
                    if (File.Exists(entry))
                    {
                        using (var file = File.OpenRead(entry))
                        using (var stream = zip.CreateEntry(entry).Open())
                        {
                            file.CopyTo(stream);
                        }
                    }
                }
            }
        }

        public static void Extract(string path)
        {
            using (var archive = File.OpenRead(path))
            using (var zip = new ZipArchive(archive, ZipArchiveMode.Read))
            {
                foreach (var entry in zip.Entries)
                {
                    var folder = Path.GetDirectoryName(entry.FullName);
                    Directory.CreateDirectory(folder);

                    using (var file = File.OpenWrite(entry.FullName))
                    using (var stream = entry.Open())
                    {
                        stream.CopyTo(file);
                    }
                }
            }
        }

        public static bool TryExtract(string path, int attempts)
        {
            while (attempts-- > 0)
            {
                try
                {
                    Extract(path);
                    return true;
                }
                catch
                {
                    Thread.Sleep(1000);
                }
            }
            return false;
        }
    }
}
