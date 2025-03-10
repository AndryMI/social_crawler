﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.IO;

namespace Core
{
    public class JsonData : IRequestData
    {
        private readonly string data;

        public string ContentType => "application/json";

        public JsonData(object data)
        {
            this.data = JsonConvert.SerializeObject(data, new StringEnumConverter());
        }

        public void Serialize(StreamWriter writer)
        {
            writer.Write(data);
        }

        public override string ToString()
        {
            return data;
        }
    }
}