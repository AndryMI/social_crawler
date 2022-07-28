using System.Collections.Generic;

namespace Core.Crawling
{
    /// <summary>High-level crawling command. Creates low-level crawling tasks</summary>
    public interface ICommand
    {
        string Type { get; }
        IEnumerable<CrawlerTask> CreateTasks();
    }
}
