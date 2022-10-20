using Core.Crawling;
using System;

namespace Core
{
    public class AccountException : Exception
    {
        public Account Account { get; private set; }

        public AccountException(Account account)
        {
            Account = account;
        }
    }
}
