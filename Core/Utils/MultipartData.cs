using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Core
{
    public class MultipartData
    {
        private readonly string boundary = "--------" + DateTimeOffset.UtcNow.Ticks.ToString("x");
        private readonly List<Item> items = new List<Item>();

        public string ContentType => "multipart/form-data; boundary=" + boundary;

        public int Size => items.Sum(x => x.Size ?? 0);
        public int Count => items.Count;

        public void Add(string name, string data, string filename = null, string type = null)
        {
            Add(name, Encoding.UTF8.GetBytes(data), filename, type);
        }

        public void Add(string name, byte[] data, string filename, string type)
        {
            items.Add(new Item(name, data, filename, type));
        }

        public void Serialize(StreamWriter writer)
        {
            foreach (var item in items)
            {
                if (item.data == null)
                {
                    Log.Warning("Item is empty: {Name} {File} {Type}", item.name, item.file, item.type);
                    continue;
                }

                writer.Write("\r\n--");
                writer.Write(boundary);
                writer.Write("\r\n");
                writer.Write("Content-Disposition: form-data; name=\"");
                writer.Write(item.name);
                writer.Write('"');

                if (!string.IsNullOrEmpty(item.file))
                {
                    writer.Write("; filename=\"");
                    writer.Write(item.file);
                    writer.Write('"');
                }
                writer.Write("\r\n");

                if (!string.IsNullOrEmpty(item.type))
                {
                    writer.Write("Content-Type: ");
                    writer.Write(item.type);
                    writer.Write("\r\n");
                }
                writer.Write("\r\n");
                writer.Flush();
                writer.BaseStream.Write(item.data, 0, item.data.Length);
            }
            writer.Write("\r\n--");
            writer.Write(boundary);
            writer.Write("--\r\n");
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(items);
        }

        private struct Item
        {
            [JsonIgnore]
            public readonly byte[] data;
            public readonly string name;
            public readonly string file;
            public readonly string type;

            public int? Size => data?.Length;

            public Item(string name, byte[] data, string file, string type)
            {
                this.name = name;
                this.data = data;
                this.file = file;
                this.type = type;
            }
        }
    }
}
