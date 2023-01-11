using Core;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Deploy
{
    /// <summary> Packs and publishes CrawlerApp to all instances </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.FirstOrDefault() != "--deploy")
            {
                Console.WriteLine("Unintentional run guard. Type 'deploy' to start deploy process.");
                if (Console.ReadLine() != "deploy")
                {
                    return;
                }
            }

            Console.WriteLine("Packing...");
            var data = new MultipartData();
            data.Add("crawler", CreateArchive(), "crawler.zip", "application/octet-stream");

            Console.WriteLine("Uploading...");
            var client = new ApiServerClient();
            client.Login();

            var response = client.Request("POST", "/crawler/update", data);
            Console.WriteLine(response);
        }

        private static byte[] CreateArchive()
        {
            using (var memory = new MemoryStream())
            {
                using (var zip = new ZipArchive(memory, ZipArchiveMode.Create, true))
                {
                    foreach (var path in Directory.EnumerateFiles(".", "*.*", SearchOption.AllDirectories))
                    {
                        if (path.StartsWith(".\\Deploy."))
                        {
                            continue;
                        }
                        using (var file = File.OpenRead(path))
                        using (var stream = zip.CreateEntry(path).Open())
                        {
                            file.CopyTo(stream);
                        }
                        Debug.WriteLine(path);
                        Console.WriteLine(path);
                    }
                }
                return memory.ToArray();
            }
        }
    }
}
