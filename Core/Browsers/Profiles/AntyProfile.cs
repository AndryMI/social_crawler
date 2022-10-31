using Core.Browsers.DevTools;
using Newtonsoft.Json;
using OpenQA.Selenium.Chrome;
using System.Threading;

namespace Core.Browsers.Profiles
{
    public class AntyProfile : IBrowserProfile
    {
        private static readonly AntyLocalApi Api = new AntyLocalApi();
        private static string LastVersion;

        public string Type => "Anty";
        public string Id { get; private set; }

        [JsonProperty]
        public string AntyUser { get; private set; }

        public ChromeDriver Start()
        {
            var automation = Api.Start(Id);
            Thread.Sleep(5000);
            try
            {
                var options = new ChromeOptions
                {
                    DebuggerAddress = "127.0.0.1:" + automation.port
                };

                return DriverService.Run(ref LastVersion, options);
            }
            catch
            {
                Api.Stop(Id);
                throw;
            }
        }

        public void Stop()
        {
            try
            {
                Api.Stop(Id);
            }
            catch { }
        }
    }
}
