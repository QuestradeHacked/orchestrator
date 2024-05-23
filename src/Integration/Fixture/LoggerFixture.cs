using Microsoft.Extensions.Logging;

namespace Integration.Fixture;

internal class LoggerFixture<TCategoryName> : ILogger<TCategoryName>
{
    private readonly List<LogEntry> _entries = new ();

    private readonly LogLevel _minimumLevel;

    public LoggerFixture() : this(LogLevel.Trace) { }

    private LoggerFixture(LogLevel minimumLevel)
    {
        _minimumLevel = minimumLevel;
    }

    public IDisposable BeginScope<TState>(TState state) => new MockLoggerScope();

    public string GetAllMessages() => GetAllMessages(null!);

    private string GetAllMessages(string? separator) =>
        string.Join(
            separator ?? Environment.NewLine,
            _entries.Select(logEntry => logEntry.Message));

    public bool IsEnabled(LogLevel logLevel) => logLevel >= _minimumLevel;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception, string> formatter)
    {
        if (IsEnabled(logLevel))
            _entries.Add(new LogEntry { Level = logLevel, Message = exception?.Message + formatter(state, exception!) });
    }
}

internal class LogEntry
{
    public LogLevel Level { get; set; }

    public string? Message { get; init; }
}

internal class MockLoggerScope : IDisposable
{
    public void Dispose() {}
}
