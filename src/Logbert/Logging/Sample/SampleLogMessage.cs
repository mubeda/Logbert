using System;

namespace Logbert.Logging.Sample;

/// <summary>
/// Sample log message implementation for testing.
/// </summary>
public class SampleLogMessage : LogMessage
{
    private readonly DateTime _timestamp;
    private readonly string _message;
    private readonly LogLevel _level;

    /// <summary>
    /// Gets the timestamp of the log message.
    /// </summary>
    public override DateTime Timestamp => _timestamp + TimeShiftOffset;

    /// <summary>
    /// Gets the message text.
    /// </summary>
    public override string Message => _message;

    /// <summary>
    /// Gets the log level.
    /// </summary>
    public override LogLevel Level => _level;

    /// <summary>
    /// Initializes a new instance of the <see cref="SampleLogMessage"/> class.
    /// </summary>
    public SampleLogMessage(int index, DateTime timestamp, LogLevel level, string logger, string message)
        : base(message, index)
    {
        _timestamp = timestamp;
        _level = level;
        _message = message;
        mLogger = logger;
    }

    /// <summary>
    /// Gets the value for the specified column.
    /// </summary>
    public override object? GetValueForColumn(int columnIndex)
    {
        return columnIndex switch
        {
            0 => Index,
            1 => Timestamp,
            2 => Level,
            3 => Logger,
            4 => Message,
            _ => null
        };
    }
}
