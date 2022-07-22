using Newtonsoft.Json;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Core.Crawling
{
    public abstract class Account
    {
        public virtual string BrowserProfile => (Email.GetHashCode() & 0xff).ToString("X2");

        public string Name;
        public string Email;
        public string Password;

        public readonly HashSet<string> AssignedUids = new HashSet<string>();

        public abstract void Login(ChromeDriver driver);

        public virtual string ToUid(string url)
        {
            var uri = new Uri(url);
            return uri.Host + uri.LocalPath;
        }
    }

    public class Accounts<T> where T : Account, new()
    {
        private static readonly T Sample = new T();
        public static readonly Accounts<T> Instance = new Accounts<T>();

        private readonly object locker = new object();
        private readonly Random random = new Random();
        private readonly List<T> accounts;

        private string FileName => $"Configs/{typeof(T).Name}.json";

        private Accounts()
        {
            lock (locker)
            {
                if (File.Exists(FileName))
                {
                    accounts = JsonConvert.DeserializeObject<List<T>>(File.ReadAllText(FileName));
                }
                if (accounts == null)
                {
                    throw new ArgumentNullException("accounts");
                }
            }
        }

        public T Get(string url)
        {
            if (accounts.Count == 0)
            {
                throw new Exception("Accounts list is empty");
            }
            lock (locker)
            {
                var uid = Sample.ToUid(url);
                var account = accounts.Find(x => x.AssignedUids.Contains(uid));
                if (account == null)
                {
                    var uids = accounts.Min(x => x.AssignedUids.Count);
                    var selected = accounts.Where(x => x.AssignedUids.Count == uids).ToArray();
                    account = selected[random.Next(selected.Length)];
                    account.AssignedUids.Add(uid);
                    File.WriteAllText(FileName, JsonConvert.SerializeObject(accounts, Formatting.Indented));
                }
                return account;
            }
        }
    }
}
