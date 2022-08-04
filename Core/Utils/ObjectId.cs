using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace Core
{
    public static class ObjectId
    {
        private static string machine = InitMachineId();
        private static int counter = InitCounter();

        // Generation reference:
        // https://stackoverflow.com/questions/25356211/collection-id-length-in-mongodb
        //
        // The 12 bytes of an ObjectId are constructed using:
        // * a 4 byte value representing the seconds since the Unix epoch
        // * a 3 byte machine identifier
        // * a 2 byte process id
        // * a 3 byte counter (starting with a random value)

        public static string New()
        {
            var time = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            return time.ToString("x8") + machine + (++counter & 0xffffff).ToString("x6");
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
    }
}
