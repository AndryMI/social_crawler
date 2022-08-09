using System;
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
    }
}
