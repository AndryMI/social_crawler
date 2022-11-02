using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace Core
{
    public class ObjectId
    {
        private static string machine = InitMachineId();
        private static int counter = InitCounter();

        private readonly string value;

        // Generation reference:
        // https://stackoverflow.com/questions/25356211/collection-id-length-in-mongodb
        //
        // The 12 bytes of an ObjectId are constructed using:
        // * a 4 byte value representing the seconds since the Unix epoch
        // * a 3 byte machine identifier
        // * a 2 byte process id
        // * a 3 byte counter (starting with a random value)

        public ObjectId()
        {
            var time = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            value = time.ToString("x8") + machine + (++counter & 0xffffff).ToString("x6");
        }

        private ObjectId(string value)
        {
            this.value = value;
        }

        public override string ToString()
        {
            return value;
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is ObjectId id && id.value == value;
        }

        public static implicit operator string(ObjectId id)
        {
            return id.value;
        }

        public static implicit operator ObjectId(string value)
        {
            return new ObjectId(value);
        }

        private static string InitMachineId()
        {
            using (var random = RandomNumberGenerator.Create())
            {
                var result = new StringBuilder();
                var process = BitConverter.GetBytes(Process.GetCurrentProcess().Id);
                var machine = new byte[3];

                random.GetNonZeroBytes(machine);

                result.Append(machine[0].ToString("x2"));
                result.Append(machine[1].ToString("x2"));
                result.Append(machine[2].ToString("x2"));

                result.Append(process[0].ToString("x2"));
                result.Append(process[1].ToString("x2"));

                return result.ToString();
            }
        }

        private static int InitCounter()
        {
            using (var random = RandomNumberGenerator.Create())
            {
                var bytes = new byte[4];
                random.GetNonZeroBytes(bytes);
                return BitConverter.ToInt32(bytes, 0);
            }
        }

        public class Json : JsonConverter<ObjectId>
        {
            public override ObjectId ReadJson(JsonReader reader, Type objectType, ObjectId existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                var token = serializer.Deserialize<JToken>(reader);
                if (token is JObject)
                {
                    token = token["$oid"];
                }
                return new ObjectId(token.Value<string>());
            }

            public override void WriteJson(JsonWriter writer, ObjectId value, JsonSerializer serializer)
            {
                serializer.Serialize(writer, value.value);
            }
        }
    }
}
