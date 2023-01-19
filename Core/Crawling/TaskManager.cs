using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace Core.Crawling
{
    public class TaskManager
    {
        private readonly string store;
        private readonly object locker = new object();
        private readonly Dictionary<ICommand, State> states;
        private readonly HashSet<string> blacklist = new HashSet<string>(ServerConfig.Instance.Blacklists);

        public TaskManager(string store = null)
        {
            this.store = string.IsNullOrWhiteSpace(store) ? null : store;
            this.states = Load(this.store);
        }

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

        public void SetBlacklist(string[] blacklist)
        {
            lock (locker)
            {
                this.blacklist.Clear();

                foreach (var item in blacklist)
                {
                    this.blacklist.Add(item);
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
                        if (!blacklist.Contains(task.Url))
                        {
                            state.Add(task);
                        }
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
                    if (!blacklist.Contains(task.Url))
                    {
                        state.Add(task);
                    }
                }
            }
        }

        public bool TryGet(Browser browser, out CrawlerTask result)
        {
            lock (locker)
            {
                var query =
                    from state in states.Values
                    let remained = Math.Min(10, state.tasks.Count)
                    from task in state.tasks
                    where task.RunAt <= DateTimeOffset.UtcNow
                    orderby remained ascending, task.Priority descending
                    select new { task, state };

                var selected = query.FirstOrDefault();
                if (selected != null)
                {
                    selected.state.Activate(selected.task);
                    result = selected.task;
                    return true;
                }
                result = null;
                return false;
            }
        }

        public void Delay(ICommand command, DateTimeOffset time = default)
        {
            lock (locker)
            {
                if (states.TryGetValue(command, out var state))
                {
                    var runAt = DateTimeOffset.UtcNow.AddSeconds(Config.Instance.RetryTimeout);
                    if (runAt < time)
                    {
                        runAt = time;
                    }
                    foreach (var task in state.tasks)
                    {
                        task.RunAt = runAt;
                    }
                }
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

        public void Save()
        {
            if (store == null)
            {
                return;
            }
            lock (locker)
            {
                try
                {
                    using (var file = File.OpenWrite(store))
                    {
                        new BinaryFormatter().Serialize(file, states);
                    }
                }
                catch (Exception e)
                {
                    Log.Warning(e, "Failed to save tasks");
                }
            }
        }

        private static Dictionary<ICommand, State> Load(string path)
        {
            try
            {
                using (var file = File.OpenRead(path))
                {
                    var states = (Dictionary<ICommand, State>)new BinaryFormatter().Deserialize(file);
                    var count = 0;
                    foreach (var state in states.Values)
                    {
                        // Reset active tasks to scheduled tasks on startup
                        foreach (var task in state.active.ToList())
                        {
                            state.Complete(task);
                            state.Add(task);
                        }
                        count += state.tasks.Count;
                    }
                    Log.Information("Restored {count} tasks for Task manager", count);
                    return states;
                }
            }
            catch
            {
                Log.Information("Creating empty Task manager");
                return new Dictionary<ICommand, State>();
            }
        }

        [Serializable]
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

        [Serializable]
        [DebuggerDisplay("Tasks: {ActiveTasks} + {tasks.Count} + {CompleteTasks} = {TotalTasks}")]
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
