using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Core.Crawling
{
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

                var account = (
                    from acc in accounts
                    let available = acc.Limits.GetAvailableCount()
                    where available > 0
                    let assigned = acc.AssignedUids.Contains(uid)
                    let uids = assigned ? 0 : acc.AssignedUids.Count
                    let rand = random.Next()
                    orderby assigned descending, available descending, uids ascending, rand ascending
                    select acc
                ).FirstOrDefault();

                if (account == null)
                {
                    var time = accounts.Min(acc => acc.Limits.GetAvailableTime());
                    throw new TryLaterException(time);
                }

                if (account.AssignedUids.Add(uid))
                {
                    File.WriteAllText(FileName, JsonConvert.SerializeObject(accounts, Formatting.Indented));
                }
                return account;
            }
        }
    }
}
