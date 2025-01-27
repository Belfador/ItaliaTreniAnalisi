namespace Importer
{
    internal class Logger
    {
        private static Logger? instance;
        private string logPath;

        public static Logger Instance => instance ??= new Logger();

        private Logger()
        {
            logPath = "log.txt";
        }

        public void Log(string message)
        {
            using (var writer = new System.IO.StreamWriter(logPath, true))
            {
                writer.WriteLine($"{DateTime.Now} - {message}");
            }

            Console.WriteLine($"{DateTime.Now} - {message}");
        }
    }
}
