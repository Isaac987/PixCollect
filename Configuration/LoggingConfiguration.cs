namespace PixCollect.Configuration;

public sealed class LoggingConfiguration
{
    public LogLevelOptions LogLevel { get; set; } = new();
    public ConsoleOptions Console { get; set; } = new();

    public sealed class LogLevelOptions
    {
        public string Default { get; set; }
        public string SystemNetHttpClient { get; set; }
    }

    public sealed class ConsoleOptions
    {
        public string FormatterName { get; set; }
        public Formatter FormatterOptions { get; set; } = new();

        public sealed class Formatter
        {
            public bool SingleLine { get; set; } = true;
        }
    }
}