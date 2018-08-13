using System;

namespace APD.Aspire.Logger
{
    public static class LogHelper
    {
        private static FileLogger _logger = new FileLogger();
        public static void Log(LogLevel logLevel, string command, string message)
        {
            _logger.Log(logLevel, command, message);
        }
    }
}