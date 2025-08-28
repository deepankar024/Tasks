using Grpc.Core;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.ConstrainedExecution;
using System.Web;

namespace FeedbackFormWebApp.Utils
{
    public static class AppLogger
    {
        private static string LogPath
        {
            get
            {
                var basePath = HttpContext.Current?.Server?.MapPath("~/App_Data/logs");
                if (basePath != null && !Directory.Exists(basePath))
                    Directory.CreateDirectory(basePath);
                return basePath != null ? Path.Combine(basePath, "app.log") : "app.log";
            }
        }

        public static void Info(string message) => Write("INFO", message, null);

        public static void Warn(string message) => Write("WARN", message, null);

        public static void Error(string message, Exception ex = null) => Write("ERROR", message, ex);

        private static void Write(string level, string message, Exception ex)
        {
            try
            {
                var timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss UTC");
                var logEntry = $"[{timestamp}] [{level}] {message}";

                if (ex != null)
                {
                    logEntry += $" :: Exception: {ex.Message}";
                    if (ex.InnerException != null)
                        logEntry += $" :: Inner Exception: {ex.InnerException.Message}";
                    logEntry += $" :: Stack Trace: {ex.StackTrace}";
                }

                File.AppendAllText(LogPath, logEntry + Environment.NewLine);
            }
            catch
            {
                // Swallow logging errors to prevent application crashes
                // In production, you might want to log to Windows Event Log as fallback
            }
        }
    }
}
//Static utility class for easy access
//Automatic log directory creation
//Exception details including stack traces
//UTC timestamps for consistency
//Fail - safe design(won't crash app if logging fails)