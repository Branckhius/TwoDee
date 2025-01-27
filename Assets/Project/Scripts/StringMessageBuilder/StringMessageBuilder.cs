using System;
using System.Text;

namespace Project.Scripts
{
    public class StringMessageBuilder : IStringMessageBuilder
    {
        private readonly StringBuilder _stringBuilder;

        public StringMessageBuilder()
        {
            _stringBuilder = new StringBuilder();
        }

        public StringMessageBuilder AppendDateTime()
        {
            _stringBuilder.Append($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ");
            return this;
        }

        public StringMessageBuilder AppendLogLevel(LogLevel logLevel)
        {
            _stringBuilder.Append($"[{logLevel}] ");
            return this;
        }

        public StringMessageBuilder AppendMessage(string message)
        {
            _stringBuilder.Append($"{message} ");
            return this;
        }

        public string Build()
        {
            return _stringBuilder.ToString().Trim();
        }

        public StringMessageBuilder Clear()
        {
            _stringBuilder.Clear();
            return this;
        }
    }
}