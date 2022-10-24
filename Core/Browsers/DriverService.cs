using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Runtime.ConstrainedExecution;

namespace Core
{
    public static class DriverService
    {
        private const string BaseUrl = "http://chromedriver.storage.googleapis.com";
        private const string Folder = "Drivers";

        private static readonly Dictionary<string, VersionInfo> versions = new Dictionary<string, VersionInfo>();
        private static readonly WebClient client = new WebClient();

        public static ChromeDriverService AutoRun(string version)
        {
            var major = version.Substring(0, version.IndexOf('.'));
            var latest = GetLatestVersion(major);

            var file = new FileInfo($"{Folder}/{latest}/chromedriver.exe");
            if (!file.Exists)
            {
                lock (client)
                {
                    if (!file.Exists)
                    {
                        var archive = $"{file.Directory.FullName}/chromedriver_win32.zip";
                        Directory.CreateDirectory(file.Directory.FullName);
                        client.DownloadFile($"{BaseUrl}/{latest}/chromedriver_win32.zip", archive);
                        ZipFile.ExtractToDirectory(archive, file.Directory.FullName);
                        File.Delete(archive);
                    }
                }
            }
            var service = ChromeDriverService.CreateDefaultService(file.Directory.FullName, file.Name);
            service.HideCommandPromptWindow = true;
            return service;
        }

        private static string GetLatestVersion(string major)
        {
            lock (versions)
            {
                if (!versions.TryGetValue(major, out var info))
                {
                    versions.Add(major, info = new VersionInfo());
                }
                if (info.NextCheck < DateTimeOffset.Now)
                {
                    info.LatestVersion = client.DownloadString($"{BaseUrl}/LATEST_RELEASE_{major}");
                    info.NextCheck = DateTimeOffset.Now.AddDays(1);
                }
                return info.LatestVersion;
            }
        }

        private class VersionInfo
        {
            public DateTimeOffset NextCheck;
            public string LatestVersion;
        }
    }
}
