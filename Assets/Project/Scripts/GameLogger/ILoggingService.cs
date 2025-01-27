namespace Project.Scripts
{
    public interface ILoggingService
    {
        void Log(string content, LogLevel logLevel);
    }
}