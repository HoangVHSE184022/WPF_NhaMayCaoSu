using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                  .WriteTo.File("Logs/serialLog.txt",
                  outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}",
                  rollingInterval: RollingInterval.Day)
                  .CreateLogger();


            Log.Information("Logger is configured.");
        }
    }
}