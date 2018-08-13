namespace APD.Aspire.Logger
{
    public abstract class LogBase
    {
        public abstract void Log(LogLevel logLevel, string command, string message);
    }
}
