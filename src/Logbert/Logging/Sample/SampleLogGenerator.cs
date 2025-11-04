using System;
using System.Collections.Generic;

namespace Couchcoding.Logbert.Logging.Sample;

/// <summary>
/// Generates sample log messages for testing.
/// </summary>
public static class SampleLogGenerator
{
    private static readonly Random _random = new();

    private static readonly string[] _loggers = new[]
    {
        "Application.Startup",
        "Database.Connection",
        "API.Controller",
        "Service.Authentication",
        "Service.DataProcessing",
        "UI.MainWindow",
        "Network.HttpClient",
        "Cache.Redis",
        "Queue.MessageProcessor",
        "Security.Authorization"
    };

    private static readonly string[] _messages = new[]
    {
        "Application started successfully",
        "Database connection established",
        "Processing request for user {0}",
        "Cache hit for key: {0}",
        "Authentication successful",
        "Query executed in {0}ms",
        "API call completed",
        "Warning: Connection pool exhausted",
        "Error connecting to remote service",
        "Fatal: Out of memory exception",
        "Debug: Processing batch {0}",
        "Trace: Entering method ProcessData()",
        "Configuration loaded from {0}",
        "User session created: {0}",
        "Transaction committed successfully",
        "Retry attempt {0} of 3",
        "Resource cleanup completed",
        "Background task scheduled",
        "Event published to queue",
        "Health check passed"
    };

    /// <summary>
    /// Generates a collection of sample log messages.
    /// </summary>
    public static List<LogMessage> GenerateMessages(int count)
    {
        var messages = new List<LogMessage>();
        var startTime = DateTime.Now.AddHours(-2);

        for (int i = 0; i < count; i++)
        {
            var timestamp = startTime.AddSeconds(i * 2);
            var level = GetRandomLogLevel();
            var logger = _loggers[_random.Next(_loggers.Length)];
            var messageTemplate = _messages[_random.Next(_messages.Length)];
            var message = string.Format(messageTemplate, _random.Next(1000));

            messages.Add(new SampleLogMessage(i, timestamp, level, logger, message));
        }

        return messages;
    }

    private static LogLevel GetRandomLogLevel()
    {
        var value = _random.Next(100);
        return value switch
        {
            < 5 => LogLevel.Trace,
            < 20 => LogLevel.Debug,
            < 60 => LogLevel.Info,
            < 80 => LogLevel.Warning,
            < 95 => LogLevel.Error,
            _ => LogLevel.Fatal
        };
    }
}
