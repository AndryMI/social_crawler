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

        public HashSet<string> AssignedUrls = new HashSet<string>();

        public abstract void Login(ChromeDriver driver);
    }

    public class Accounts<T> where T : Account
    {
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
            lock (locker)
            {
                var account = accounts.Find(x => x.AssignedUrls.Contains(url));
                if (account == null)
                {
                    var urls = accounts.Min(x => x.AssignedUrls.Count);
                    var selected = accounts.Where(x => x.AssignedUrls.Count == urls).ToArray();
                    account = selected[random.Next(selected.Length)];
                    account.AssignedUrls.Add(url);
                    File.WriteAllText(FileName, JsonConvert.SerializeObject(accounts, Formatting.Indented));
                }
                return account;
            }
        }
    }
}
