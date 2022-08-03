using Core.Data;
using MimeTypes;
using Serilog;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Core.Storages
{
    public class LocalMediaStorage : IMediaStorage
    {
        public bool WaitForBrowserLoading => true;
        public string Folder { get; private set; }

        public LocalMediaStorage(string folder)
        {
            Folder = folder;
        }

        public void StoreImage(ImageUrl image)
        {
            if (image.Data == null)
            {
                Log.Warning("Image not loaded {url}", image.Original);
                return;
            }
            using (var sha = SHA256.Create())
            {
                var stored = new StringBuilder();
                stored.Append(DateTimeOffset.UtcNow.ToString(@"\/yyyy-MM\/"));

                var hash = sha.ComputeHash(image.Data);
                stored.Append(hash[0].ToString("x2")).Append('/');
                stored.Append(hash[1].ToString("x2")).Append('/');
                for (var i = 2; i < hash.Length; i++)
                {
                    stored.Append(hash[i].ToString("x2"));
                }

                stored.Append(MimeTypeMap.GetExtension(image.MimeType, false));
                image.Stored = stored.ToString();
            }
            var path = Folder + image.Stored;
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllBytes(path, image.Data);
        }
    }
}
