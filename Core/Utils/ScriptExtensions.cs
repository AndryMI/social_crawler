using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.IO;
using System.Text;

namespace Core.Utils
{
    public static class ScriptExtensions
    {
        public static T RunCollector<T>(this ChromeDriver driver, string path)
        {
            var script = new StringBuilder()
                .Append("try {")
                .Append(" return (function __FN__() {")
                .Append(File.ReadAllText(path))
                .Append(" })()")
                .Append("}")
                .Append("catch (error) {")
                .Append(" return '!' + error.stack")
                .Append("}");

            var json = driver.ExecuteScript(script.ToString()) as string;
            if (json.StartsWith("!"))
            {
                throw new JavaScriptException(json.Replace("__FN__", path));
            }
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
