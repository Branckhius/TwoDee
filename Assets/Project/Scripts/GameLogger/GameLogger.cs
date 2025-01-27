namespace Project.Scripts
{
    public static class GameLogger
    {
        private static LogLevel _logLevel;
        private static ILoggingService _loggingService = new UnityLoggingService();
        private static readonly IStringMessageBuilder _messageBuilder = new StringMessageBuilder();

#if UNITY_EDITOR
        static GameLogger()
        {
            _logLevel = LogLevel.T;
        }
#elif DEVELOPMENT_BUILD
        static GameLogger()
        {
            _logLevel = LogLevel.Trace;
        }
#else
        static GameLogger()
        {
            _logLevel = LogLevel.Error;
        }
#endif

        // Private log method (only used internally)
        private static void Log(object content, LogLevel logLevel)
        {
            if (logLevel >= _logLevel)
            {
                string message = _messageBuilder.Clear()
                    .AppendDateTime()
                    .AppendLogLevel(logLevel)
                    .AppendMessage(content.ToString())
                    .Build();

                _loggingService.Log(message, logLevel);
            }
        }

        // Public methods for specific log levels
        public static void LogTrace(object content) => Log(content, LogLevel.T);
        public static void LogDebug(object content) => Log(content, LogLevel.D);
        public static void LogInformation(object content) => Log(content, LogLevel.I);
        public static void LogWarning(object content) => Log(content, LogLevel.W);
        public static void LogError(object content) => Log(content, LogLevel.E);
        public static void LogCritical(object content) => Log(content, LogLevel.C);
    }

    public enum LogLevel
    {
        T, // Trace
        D, // Debug
        I, // Information
        W, // Warning
        E, // Error
        C  // Critical
    }
}
