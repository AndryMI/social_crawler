using System.Collections.Concurrent;
using System.IO;

namespace Core.Storages
{
    public class LocalMultipartStorage : MultipartStorage
    {
        private readonly ConcurrentQueue<string> files;
        private readonly string folder;

        public LocalMultipartStorage(string folder)
        {
            Directory.CreateDirectory(this.folder = folder);
            files = new ConcurrentQueue<string>(Directory.EnumerateFiles(folder, "*.part"));
        }

        protected override bool Dequeue(out MultipartData data)
        {
            if (files.TryDequeue(out var path))
            {
                data = MultipartData.Load(path);
                File.Delete(path);
                return true;
            }
            data = null;
            return false;
        }

        protected override void Enqueue(MultipartData data)
        {
            var path = folder + "/" + new ObjectId() + ".part";
            data.Save(path);
            files.Enqueue(path);
        }
    }
}