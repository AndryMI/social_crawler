using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Crawling
{
    public class TaskManager
    {
        private readonly object locker = new object();
        private readonly Dictionary<ICommand, State> states = new Dictionary<ICommand, State>();

        public List<Status> Progress
        {
            get
            {
                lock (locker)
                {
                    var result = new List<Status>(states.Values);
                    foreach (var state in result)
                    {
                        if (state.IsOutdated)
                        {
                            states.Remove(state.Command);
                        }
                    }
                    return result;
                }
            }
        }

        public void Add(ICommand command)
        {
            lock (locker)
            {
                if (command is CancelCommand)
                {
                    foreach (var key in states.Keys.Where(x => x.Id == command.Id).ToList())
                    {
                        states.Remove(key);
                    }
                }
                else if (command != null)
                {
                    if (!states.TryGetValue(command, out var state))
                    {
                        states.Add(command, state = new State(command));
                    }
                    foreach (var task in command.CreateTasks())
                    {
                        state.Add(task);
                    }
                }
            }
        }

        public void Add(CrawlerTask task)
        {
            lock (locker)
            {
                if (states.TryGetValue(task.Command, out var state))
                {
                    state.Add(task);
                }
            }
        }

        public bool TryGet(Browser browser, out CrawlerTask task)
        {
            lock (locker)
            {
                task = Query().Where(x => x.RunAt <= DateTimeOffset.UtcNow).OrderByDescending(x => x.Priority).FirstOrDefault();
                if (task != null && states.TryGetValue(task.Command, out var state))
                {
                    state.Activate(task);
                    return true;
                }
                return false;
            }
        }

        public void Delay(CrawlerTask task, DateTimeOffset time = default)
        {
            lock (locker)
            {
                if (states.TryGetValue(task.Command, out var state))
                {
                    task.RunAt = DateTimeOffset.UtcNow.AddSeconds(Config.Instance.RetryTimeout);
                    if (task.RunAt < time)
                    {
                        task.RunAt = time;
                    }
                    state.Add(task);
                }
            }
        }

        public void Retry(CrawlerTask task, DateTimeOffset time = default)
        {
            if (task.RunFails++ < Config.Instance.RetryAttempts)
            {
                Delay(task, time);
            }
            else Log.Error("Task skipped: {task}", task);
        }

        public void Complete(CrawlerTask task)
        {
            lock (locker)
            {
                if (states.TryGetValue(task.Command, out var state))
                {
                    state.Complete(task);
                }
            }
        }

        private IEnumerable<CrawlerTask> Query()
        {
            foreach (var state in states.Values)
            {
                foreach (var task in state.tasks)
                {
                    yield return task;
                }
            }
        }

        public abstract class Status
        {
            public ICommand Command { get; protected set; }
            public DateTimeOffset AddedAt { get; protected set; } = DateTimeOffset.UtcNow;
            public DateTimeOffset UpdatedAt { get; protected set; } = DateTimeOffset.UtcNow;

            public int ActiveTasks { get; protected set; }
            public int CompleteTasks { get; protected set; }
            public int TotalTasks { get; protected set; }

            [JsonIgnore]
            public bool IsOutdated
            {
                get => CompleteTasks >= TotalTasks && UpdatedAt < DateTimeOffset.UtcNow.AddMinutes(-1);
            }
        }

        private class State : Status
        {
            [JsonIgnore]
            public readonly HashSet<CrawlerTask> active = new HashSet<CrawlerTask>();

            [JsonIgnore]
            public readonly HashSet<CrawlerTask> tasks = new HashSet<CrawlerTask>();

            public State(ICommand command)
            {
                Command = command;
            }

            public void Add(CrawlerTask task)
            {
                if (tasks.Add(task))
                {
                    TotalTasks++;
                    UpdatedAt = DateTimeOffset.UtcNow;
                }
            }

            public void Activate(CrawlerTask task)
            {
                active.Add(task);
                tasks.Remove(task);
                ActiveTasks = active.Count;
                UpdatedAt = DateTimeOffset.UtcNow;
            }

            public void Complete(CrawlerTask task)
            {
                active.Remove(task);
                CompleteTasks++;
                ActiveTasks = active.Count;
                UpdatedAt = DateTimeOffset.UtcNow;
            }
        }
    }
}
