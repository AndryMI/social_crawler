using OpenQA.Selenium.Chrome;

namespace RD_Team_TweetMonitor
{
    public interface IDriver
    {
        TaskType Type { get; }
        ChromeDriver Instance();

        void Destroy();
        void Suspend();
    }
}
