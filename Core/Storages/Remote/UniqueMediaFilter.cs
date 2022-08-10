using Core.Data;
using MimeTypes;
using Serilog;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Core.Storages.Remote
{
    public class UniqueMediaFilter
    {
        private readonly Dictionary<string, string> storage = ServerConfig.Instance.MediaWarmup;

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
                var found = storage.TryGetValue(hash, out var path);

                if (!found)
                {
                    path = DateTimeOffset.UtcNow.ToString(@"\/yyyy\/MM\/dd\/") + ObjectId.New() + MimeTypeMap.GetExtension(image.MimeType);
                    storage[hash] = path;
                }

                image.Stored = path;
                return !found;
            }
        }
    }
}
