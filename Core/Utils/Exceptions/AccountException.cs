using Core.Crawling;
using System;

namespace Core
{
    public class AccountException : Exception
    {
        public Account Account { get; private set; }

        public AccountException(string reason, Account account) : base(reason)
        {
            Account = account;
        }
    }
}
