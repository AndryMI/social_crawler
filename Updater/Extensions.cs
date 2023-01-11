using System;
using System.Diagnostics;

namespace Updater
{
    internal static class Extensions
    {
        public static bool WaitForExit(this Process process, TimeSpan time)
        {
            return process.WaitForExit((int)time.TotalMilliseconds);
        }
    }
}
