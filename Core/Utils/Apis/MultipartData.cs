﻿using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Core
{
    [Serializable]
    public class MultipartData : IRequestData
    {
        private readonly string boundary = "--------" + DateTimeOffset.UtcNow.Ticks.ToString("x");
        private readonly List<Item> items = new List<Item>();

        public string ContentType => "multipart/form-data; boundary=" + boundary;

        public int Size => items.Sum(x => x.Size ?? 0);
        public int FilesCount => items.Count(x => x.file != null);
        public int TotalCount => items.Count;

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

        public void Save(string path)
        {
            using (var stream = File.OpenWrite(path))
            {
                new BinaryFormatter().Serialize(stream, this);
            }
        }

        public static MultipartData Load(string path)
        {
            using (var stream = File.OpenRead(path))
            {
                return (MultipartData)new BinaryFormatter().Deserialize(stream);
            }
        }

        [Serializable]
        private struct Item
        {
            [JsonIgnore]
            public readonly byte[] data;
            public readonly string name;
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly string file;
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
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
