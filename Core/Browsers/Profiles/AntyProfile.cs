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
            try
            {
                var options = new ChromeOptions
                {
                    DebuggerAddress = "127.0.0.1:" + StartPort()
                };
                return DriverService.Run(ref LastVersion, options);
            }
            catch
            {
                Stop();
                throw;
            }
        }

        private int StartPort()
        {
            lock (Api)
            {
                var automation = Api.Start(Id);
                for (var i = 0; !Api.IsRunning(Id) && i < Config.Instance.WaitTimeout; i++)
                {
                    Thread.Sleep(1000);
                }
                return automation.port;
            }
        }

        public void Stop()
        {
            try
            {
                lock (Api)
                {
                    Api.Stop(Id);

                    for (var i = 1; Api.IsRunning(Id) && i < Config.Instance.WaitTimeout; i++)
                    {
                        if (i % 10 == 0)
                        {
                            Api.Stop(Id);
                        }
                        Thread.Sleep(1000);
                    }
                }
            }
            catch { }
        }
    }
}
