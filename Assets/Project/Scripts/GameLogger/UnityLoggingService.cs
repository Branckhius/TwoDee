
using UnityEngine;

namespace Project.Scripts
{
    public class UnityLoggingService : ILoggingService
    {
        public void Log(string content, LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.T:
                case LogLevel.D:
                case LogLevel.I:
                    Debug.Log(content);
                    break;
                case LogLevel.W:
                    Debug.LogWarning(content);
                    break;
                case LogLevel.E:
                case LogLevel.C:
                    Debug.LogError(content);
                    break;
            }
        }
    }
}