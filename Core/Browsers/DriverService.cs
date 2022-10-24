using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace Core.Browsers
{
    public static class DriverService
    {
        private const string BaseUrl = "http://chromedriver.storage.googleapis.com";
        private const string Folder = "Drivers";

        private static readonly Dictionary<string, VersionInfo> versions = new Dictionary<string, VersionInfo>();
        private static readonly WebClient client = new WebClient();

        public static ChromeDriver Run(ref string version, ChromeOptions options)
        {
            if (string.IsNullOrEmpty(version))
            {
                version = GetDevToolsVersion();
            }
            var timeout = TimeSpan.FromSeconds(Config.Instance.WaitTimeout);
            var service = CreateService(version);
            try
            {
                var driver = new ChromeDriver(service, options, timeout);
                var caps = driver.Capabilities;

                if (GetMajorVersion(version) != GetMajorVersion(caps["browserVersion"].ToString()))
                {
                    if (string.IsNullOrEmpty(options.DebuggerAddress))
                    {
                        driver.Close();
                    }
                    driver.Dispose();

                    throw new WebDriverException($"Browser and Driver versions may be incompatible. Use {caps["browserVersion"]}");
                }
                return driver;
            }
            catch (WebDriverException e)
            {
                service.Dispose();

                var match = Regex.Match(e.Message, @"\d+\.\d+\.\d+\.\d+", RegexOptions.RightToLeft);
                if (!match.Success)
                {
                    throw;
                }
                try
                {
                    service = CreateService(version = match.Value);
                    return new ChromeDriver(service, options, timeout);
                }
                catch
                {
                    service.Dispose();
                    throw;
                }
            }
        }

        private static ChromeDriverService CreateService(string version)
        {
            var major = GetMajorVersion(version);
            var latest = GetLatestVersion(major);

            var file = new FileInfo($"{Folder}/{latest}/chromedriver.exe");
            if (!file.Exists)
            {
                lock (client)
                {
                    if (!file.Exists)
                    {
                        Log.Information("Downloading chrome driver {Version}", latest);
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

        private static string GetDevToolsVersion()
        {
            var regex = new Regex(@"Selenium.DevTools.V(\d+)", RegexOptions.Compiled);

            var latest = typeof(OpenQA.Selenium.DevTools.DevToolsSession).Assembly.GetExportedTypes()
                .Select(x => regex.Match(x.Namespace))
                .Max(x => x.Success && int.TryParse(x.Groups[1].Value, out var ver) ? ver : 0);

            return $"{latest}.0.0.0";
        }

        private static string GetMajorVersion(string version)
        {
            return version.Substring(0, version.IndexOf('.'));
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
                    info.NextCheck = DateTimeOffset.Now.AddHours(1);
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
