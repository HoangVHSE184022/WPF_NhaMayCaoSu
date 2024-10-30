using Serilog;

namespace WPF_NhaMayCaoSu.Core.Utils
{
    public static class LoggingHelper
    {
        public static void ConfigureLogger()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string logDirectory = Path.Combine(baseDirectory, "Logs");

            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            Log.Logger = new LoggerConfiguration()
               .WriteTo.File(
                   Path.Combine(logDirectory, "serialLog.txt"),
                   outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}",
                   rollingInterval: RollingInterval.Hour,
                   retainedFileCountLimit: 24
               )
               .CreateLogger();


            Log.Information("Logger is configured.");
        }
    }
}