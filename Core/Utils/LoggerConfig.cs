using Microsoft.CSharp;
using Serilog;
using System;
using System.CodeDom.Compiler;
using System.IO;

namespace Core
{
    public static class LoggerConfig
    {
        public static void Init()
        {
            InitDefault();
            InitCompiled();
        }

        private static void InitDefault()
        {
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in asm.GetTypes())
                {
                    if (type.FullName == "LogConfig")
                    {
                        Activator.CreateInstance(type);
                        return;
                    }
                }
            }
        }

        private static void InitCompiled()
        {
            if (!File.Exists("Configs/LogConfig.cs"))
            {
                return;
            }

            var source = File.ReadAllText("Configs/LogConfig.cs");
            var param = new CompilerParameters
            {
                GenerateExecutable = false,
                IncludeDebugInformation = false,
                GenerateInMemory = true
            };
            foreach (var file in Directory.EnumerateFiles(".", "*.dll"))
            {
                param.ReferencedAssemblies.Add(file);
            }

            var results = new CSharpCodeProvider().CompileAssemblyFromSource(param, source);
            if (results.Errors.HasErrors)
            {
                foreach (var error in results.Errors)
                {
                    Log.Error(error.ToString());
                }
            }
            else
            {
                results.CompiledAssembly.CreateInstance("LogConfig");
            }
        }
    }
}
