using OpenQA.Selenium.Chrome;

namespace Core.Browsers.Profiles
{
    public class AnonymousProfile : IBrowserProfile
    {
        private static readonly IBrowserProfile impl = new ChromeProfile();

        public string Type => "Anonymous";
        public string Id => null;

        public ChromeDriver Start()
        {
            return impl.Start();
        }
    }
}
