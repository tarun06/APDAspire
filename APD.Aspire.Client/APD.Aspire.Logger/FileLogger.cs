using System;
using System.IO;
using System.Threading;

namespace APD.Aspire.Logger
{
    public class FileLogger : LogBase
    {

        /// <summary>
        /// The format to be used by logging.
        /// </summary>
        private string m_Format = "{0:yyyy-MM-dd HH:mm:ss:ffff}|{1}|{2}|{3}";

        /// <summary>
        /// Light weight thread
        /// </summary>
        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1);

        private string fileName = Path.Combine(Environment.GetFolderPath(
            Environment.SpecialFolder.ApplicationData), "AspireContact.txt");
        public override void Log(LogLevel logLevel, string command, string message)
        {
            try
            {
                _semaphoreSlim.WaitAsync();
                WriteToFile(logLevel, command, message);
            }
            catch (Exception ex)
            {

            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        private void WriteToFile(LogLevel logLevel, string command, string message)
        {
            using (StreamWriter streamWriter = new StreamWriter(fileName, true))
            {
                var newFormatedLine = string.Format(m_Format, DateTime.Now, logLevel, command, message);

                streamWriter.WriteLine(newFormatedLine);
                streamWriter.Close();
            }
        }
    }
}
