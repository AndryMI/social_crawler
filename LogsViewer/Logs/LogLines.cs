namespace LogsViewer.Logs
{
    public static class LogLines
    {
        public static IEnumerable<string> PlainFile(string path)
        {
            using (var reader = new StreamReader(path))
            {
                while (true)
                {
                    var line = reader.ReadLine();
                    if (line == null)
                    {
                        break;
                    }
                    yield return line;
                }
            }
        }
    }
}
