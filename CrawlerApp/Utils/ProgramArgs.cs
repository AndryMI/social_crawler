﻿using System;
using System.Text;
using System.Text.RegularExpressions;

namespace CrawlerApp
{
    public static class ProgramArgs
    {
        public static void Handle(string[] args)
        {
            for (var i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith("--"))
                {
                    var name = Regex.Replace(args[i], @"-+(\w)", match => match.Groups[1].Value.ToUpper());
                    try
                    {
                        var method = typeof(Program).GetMethod(name);
                        var parameters = new string[method.GetParameters().Length];

                        for (var j = 0; j < parameters.Length; j++)
                        {
                            parameters[j] = args[++i];
                        }
                        method.Invoke(null, parameters);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Failed to process: " + name, ex);
                    }
                }
            }
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
