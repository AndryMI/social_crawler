using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Core.Browsers.DevTools
{
    public abstract class DevTools
    {
        protected static readonly Type[] versions;

        public static readonly int LatestVersion;

        static DevTools()
        {
            versions = typeof(DevToolsSession).Assembly.GetExportedTypes()
                .Where(typeof(DevToolsDomains).IsAssignableFrom)
                .Where(type => type != typeof(DevToolsDomains))
                .ToArray();

            var regex = new Regex(@"V(\d+)", RegexOptions.Compiled);
            foreach (var type in versions)
            {
                var version = int.Parse(regex.Match(type.Name).Groups[1].Value);
                if (version > LatestVersion)
                {
                    LatestVersion = version;
                }
            }
        }

        public static void ValidateAll()
        {
            DevTools<Network>.Validate();
            DevTools<Console>.Validate();
        }
    }

    public abstract class DevTools<T> : DevTools
    {
        private static readonly Dictionary<Type, ConstructorInfo> constructors = InitConstructors();

        public static void Validate()
        {
            foreach (var version in versions)
            {
                if (!constructors.ContainsKey(version))
                {
                    throw new Exception($"Validation failed: Constructor of {typeof(T)} for {version} not found");
                }
            }
        }

        public static T Create(ChromeDriver driver)
        {
            var domains = driver.GetDevToolsSession().Domains;
            return (T)constructors[domains.GetType()].Invoke(new[] { domains });
        }

        private static Dictionary<Type, ConstructorInfo> InitConstructors()
        {
            var types = typeof(T).Assembly.GetExportedTypes().Where(typeof(T).IsAssignableFrom).ToList();
            var constructors = new Dictionary<Type, ConstructorInfo>();

            foreach (var version in versions)
            {
                foreach (var type in types)
                {
                    var ctor = type.GetConstructor(new[] { version });
                    if (ctor != null)
                    {
                        constructors.Add(version, ctor);
                    }
                }
            }
            return constructors;
        }
    }
}
