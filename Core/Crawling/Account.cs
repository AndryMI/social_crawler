﻿using Newtonsoft.Json;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;

namespace Core.Crawling
{
    public abstract class Account
    {
        public virtual string BrowserProfile => (Email.GetHashCode() & 0xff).ToString("X2");

        public string Name;
        public string Email;
        public string Password;

        public readonly HashSet<string> AssignedUids = new HashSet<string>();

        [JsonProperty("LastRequests", NullValueHandling = NullValueHandling.Ignore)]
        public readonly RequestLimits Limits;

        public abstract void Login(ChromeDriver driver);
        public virtual RequestLimits GetRequestLimits() => null;

        protected Account()
        {
            Limits = GetRequestLimits();
        }

        public virtual string ToUid(string url)
        {
            var uri = new Uri(url);
            return uri.Host + uri.LocalPath;
        }
    }
}
