using System;
using System.IO;

namespace Core.Crawling
{
    public struct BrowserProfile : IDisposable
    {
        private static long nextUid = DateTimeOffset.UtcNow.Ticks;
        
        private string path;
        private bool temporary;

        public readonly string Name;
        public bool IsAnonymous => Name == null;

        public BrowserProfile(string name) : this()
        {
            Name = string.IsNullOrWhiteSpace(name) ? null : name;
        }

        public string FullPath
        {
            get
            {
                if (IsAnonymous)
                {
                    return null;
                }
                if (path == null)
                {
                    path = Path.GetFullPath("Browsers/" + Name);

                    if (IsLocked(path))
                    {
                        CopyCookies(path, path += "-" + (nextUid++).ToString("x"));
                        temporary = true;
                    }
                }
                return path;
            }
        }

        public void Dispose()
        {
            if (temporary)
            {
                try { Directory.Delete(path, true); } catch { }
            }
            this = default;
        }

        private static void CopyCookies(string source, string target)
        {
            CopyFiles(source + "/Default/Network", target + "/Default/Network");
            CopyFiles(source, target);
        }

        private static void CopyFiles(string source, string target)
        {
            var folder = Directory.CreateDirectory(target);
            foreach (var file in new DirectoryInfo(source).GetFiles())
            {
                file.CopyTo(Path.Combine(folder.FullName, file.Name), true);
            }
        }

        private static bool IsLocked(string path)
        {
            var lockfile = path + "/lockfile";
            try
            {
                if (!File.Exists(lockfile))
                {
                    return false;
                }
                using (File.OpenWrite(lockfile))
                {
                    return false;
                }
            }
            catch
            {
                return true;
            }
        }
    }
}