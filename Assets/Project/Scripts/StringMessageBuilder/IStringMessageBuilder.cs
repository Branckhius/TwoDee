namespace Project.Scripts
{
    public interface IStringMessageBuilder
    {
        StringMessageBuilder AppendDateTime();
        StringMessageBuilder AppendLogLevel(LogLevel logLevel);
        StringMessageBuilder AppendMessage(string message);
        string Build();
        StringMessageBuilder Clear();
    }
}