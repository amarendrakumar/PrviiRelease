using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Prvii.ExceptionHandling
{
    public class ExceptionHandler
    {
        public static void HandleException(Exception ex)
        {
            //Do not log any thread abort exception
            //This exception generally occurs from Response.Redirect
            if (ex.GetType() != typeof(ThreadAbortException))
                LogMessageToFile(ex.Message + Environment.NewLine + ex.StackTrace);
        }

        private static void LogMessageToFile(string message)
        {
            StreamWriter streamWriter;
            FileInfo fileInfo;
            string filePath = string.Empty;
            bool fileLoggingEnabled = false;
            long maxLogFileSize = 0;
            long currentFileSize = 0;
            Decimal currentFileSizeInMB = 0;

            fileLoggingEnabled = Convert.ToBoolean(ConfigurationManager.AppSettings["LoggingEnabled"]);
            if (fileLoggingEnabled)
            {
                filePath = ConfigurationManager.AppSettings["LogFilePath"];
                fileInfo = new FileInfo(filePath);

                maxLogFileSize = Convert.ToInt64(ConfigurationManager.AppSettings["MaxLogFileSize"]);

                if (fileInfo.Exists)
                {
                    currentFileSize = fileInfo.Length;
                    currentFileSizeInMB = currentFileSize / (1024 * 1024);
                }

                if (!fileInfo.Exists || currentFileSizeInMB > maxLogFileSize)
                    streamWriter = fileInfo.CreateText();
                else
                    streamWriter = new StreamWriter(filePath, true);

                streamWriter.WriteLine(Convert.ToString(DateTime.Now) + " " + message);
                streamWriter.WriteLine("============================================");
                streamWriter.Close();
                streamWriter.Dispose();
            }

        }


        public static void LogMessage(string message, bool loggingEnabled, long maxLogFileSize, string logFilePath)
        {
            FileInfo fileInfo;
            StreamWriter streamWriter;
            long currentfileSize = 0;
            Decimal currentFileSizeInMB = 0;

            if (loggingEnabled)
            {
                fileInfo = new FileInfo(logFilePath);

                if (fileInfo.Exists)
                {
                    currentfileSize = fileInfo.Length;
                    currentFileSizeInMB = currentfileSize / (1024 * 1024);
                }

                if (!fileInfo.Exists || currentFileSizeInMB > maxLogFileSize)
                    streamWriter = fileInfo.CreateText();
                else
                    streamWriter = new StreamWriter(logFilePath, true);

                streamWriter.WriteLine("==================================================================");
                streamWriter.WriteLine(DateTime.Now.ToString());
                streamWriter.WriteLine(message);
                streamWriter.WriteLine("==================================================================");
                streamWriter.Flush();
                streamWriter.Close();
                streamWriter.Dispose();
            }
        }
    }
}
