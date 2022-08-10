using Core.Data;
using MimeTypes;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace Core.Storages.Remote
{
    public class UniqueMediaFilter
    {
        private readonly Dictionary<string, string> storage = ServerConfig.Instance.MediaWarmup;
        private readonly object locker = new object();
        private const int TrimTreshold = 1000000;

        public bool NeedStore(ImageUrl image)
        {
            if (image.Data == null)
            {
                Log.Warning("Image not loaded {url}", image.Original);
                return true;
            }
            using (var sha = SHA256.Create())
            {
                var hash = Convert.ToBase64String(sha.ComputeHash(image.Data));
                lock (locker)
                {
                    var found = storage.TryGetValue(hash, out var path);
                    if (!found)
                    {
                        path = DateTimeOffset.UtcNow.ToString(@"\/yyyy\/MM\/dd\/") + ObjectId.New() + MimeTypeMap.GetExtension(image.MimeType);
                        storage[hash] = path;
                        Trim(storage);
                    }

                    image.Stored = path;
                    return !found;
                }
            }
        }

        private static void Trim(Dictionary<string, string> storage)
        {
            if (storage.Count > TrimTreshold)
            {
                var keys = storage.OrderBy(x => x.Value).Take(TrimTreshold / 2).Select(x => x.Key).ToList();
                foreach (var key in keys)
                {
                    storage.Remove(key);
                }
            }
        }
    }
}
