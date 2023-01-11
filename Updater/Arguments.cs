using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Updater
{
    internal class Arguments
    {
        public readonly string ProcessId;
        public readonly string UpdatePath;
        public readonly string CrawlerPath;
        public readonly string CrawlerArgs;

        public Arguments(string[] args)
        {
            ProcessId = args[0];
            UpdatePath = args[1];
            CrawlerPath = args[2];
            CrawlerArgs = Escape(args.Skip(3).ToArray());
        }

        public void Print()
        {
            Console.WriteLine("ProcessId: " + ProcessId);
            Console.WriteLine("UpdatePath: " + UpdatePath);
            Console.WriteLine("CrawlerPath: " + CrawlerPath);
            Console.WriteLine("CrawlerArgs: " + CrawlerArgs);
        }

        /// <summary>
        /// Quotes all arguments that contain whitespace, or begin with a quote and returns a single
        /// argument string for use with Process.Start().
        /// </summary>
        /// <param name="args">A list of strings for arguments, may not contain null, '\0', '\r', or '\n'</param>
        /// <returns>The combined list of escaped/quoted strings</returns>
        /// <exception cref="System.ArgumentNullException">Raised when one of the arguments is null</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Raised if an argument contains '\0', '\r', or '\n'</exception>
        public static string Escape(params string[] args)
        {
            // Original source: http://csharptest.net/529/how-to-correctly-escape-command-line-arguments-in-c/index.html

            StringBuilder arguments = new StringBuilder();
            Regex invalidChar = new Regex("[\x00\x0a\x0d]");//  these can not be escaped
            Regex needsQuotes = new Regex(@"\s|""");//          contains whitespace or two quote characters
            Regex escapeQuote = new Regex(@"(\\*)(""|$)");//    one or more '\' followed with a quote or end of string
            for (int carg = 0; args != null && carg < args.Length; carg++)
            {
                if (args[carg] == null) { throw new ArgumentNullException("args[" + carg + "]"); }
                if (invalidChar.IsMatch(args[carg])) { throw new ArgumentOutOfRangeException("args[" + carg + "]"); }
                if (args[carg] == String.Empty) { arguments.Append("\"\""); }
                else if (!needsQuotes.IsMatch(args[carg])) { arguments.Append(args[carg]); }
                else
                {
                    arguments.Append('"');
                    arguments.Append(escapeQuote.Replace(args[carg], m =>
                    m.Groups[1].Value + m.Groups[1].Value +
                    (m.Groups[2].Value == "\"" ? "\\\"" : "")
                    ));
                    arguments.Append('"');
                }
                if (carg + 1 < args.Length)
                    arguments.Append(' ');
            }
            return arguments.ToString();
        }
    }
}
