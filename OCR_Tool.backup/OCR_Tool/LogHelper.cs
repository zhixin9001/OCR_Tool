using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCR_Tool
{
    class LogHelper
    {
        public const string LOG_DIR = "Logs";
        public const string LOG_SUFFIX = "log";

        public static void WriteLog(Exception e)
        {
            var dataDir = AppDomain.CurrentDomain.BaseDirectory;
            if (!Directory.Exists(Path.Combine(dataDir, LogHelper.LOG_DIR)))
            {
                Directory.CreateDirectory(Path.Combine(dataDir, LogHelper.LOG_DIR));
            }

            var currentMonthLogPath = Path.Combine(dataDir, LogHelper.LOG_DIR, LogHelper.GetCurrentMonthString());
            var currentMonthLogFile = currentMonthLogPath + "." + LogHelper.LOG_SUFFIX;
            if (!File.Exists(currentMonthLogPath + "." + LogHelper.LOG_SUFFIX))
            {
                using (var writer = File.Create(currentMonthLogFile)) { }
            }

            using (var writer = File.AppendText(currentMonthLogFile))
            {
                writer.WriteLine(LogHelper.GetExceptionString(e));
            }

        }

        private static string GetCurrentMonthString()
        {
            var currentDate = DateTime.Now;
            return string.Format("{0}-{1}", currentDate.Year, currentDate.Month);
        }

        private static string GetExceptionString(Exception e)
        {
            return string.Format("{0} - {1}", DateTime.Now.ToString(), JsonConvert.SerializeObject(e));
        }
    }
}
