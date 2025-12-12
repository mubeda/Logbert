using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Logbert.Logging;

namespace Logbert.Services;

/// <summary>
/// Service for exporting log messages to various formats.
/// </summary>
public class ExportService
{
    /// <summary>
    /// Export format options.
    /// </summary>
    public enum ExportFormat
    {
        Csv,
        Text
    }

    /// <summary>
    /// Export scope options.
    /// </summary>
    public enum ExportScope
    {
        All,
        Filtered,
        Selected
    }

    /// <summary>
    /// Progress event arguments for export operations.
    /// </summary>
    public class ExportProgressEventArgs : EventArgs
    {
        public int Current { get; }
        public int Total { get; }
        public double Percentage => Total > 0 ? (double)Current / Total * 100 : 0;

        public ExportProgressEventArgs(int current, int total)
        {
            Current = current;
            Total = total;
        }
    }

    /// <summary>
    /// Occurs when export progress changes.
    /// </summary>
    public event EventHandler<ExportProgressEventArgs>? ProgressChanged;

    /// <summary>
    /// Exports log messages to a CSV file.
    /// </summary>
    /// <param name="messages">The messages to export.</param>
    /// <param name="filePath">The output file path.</param>
    /// <param name="encoding">The text encoding to use.</param>
    /// <param name="includeHeaders">Whether to include column headers.</param>
    /// <param name="delimiter">The delimiter character (default: comma).</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task representing the export operation.</returns>
    public async Task ExportToCsvAsync(
        IReadOnlyList<LogMessage> messages,
        string filePath,
        Encoding encoding,
        bool includeHeaders = true,
        char delimiter = ',',
        CancellationToken cancellationToken = default)
    {
        if (messages == null || messages.Count == 0)
        {
            throw new ArgumentException("No messages to export.", nameof(messages));
        }

        if (string.IsNullOrEmpty(filePath))
        {
            throw new ArgumentException("File path cannot be empty.", nameof(filePath));
        }

        await using var writer = new StreamWriter(filePath, false, encoding);

        // Write header row
        if (includeHeaders)
        {
            var header = GetCsvHeader(delimiter);
            await writer.WriteLineAsync(header);
        }

        // Write data rows
        int total = messages.Count;
        for (int i = 0; i < total; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var line = FormatCsvLine(messages[i], delimiter);
            await writer.WriteLineAsync(line);

            // Report progress every 100 messages or at the end
            if (i % 100 == 0 || i == total - 1)
            {
                ProgressChanged?.Invoke(this, new ExportProgressEventArgs(i + 1, total));
            }
        }
    }

    /// <summary>
    /// Exports log messages to a text file.
    /// </summary>
    /// <param name="messages">The messages to export.</param>
    /// <param name="filePath">The output file path.</param>
    /// <param name="encoding">The text encoding to use.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task representing the export operation.</returns>
    public async Task ExportToTextAsync(
        IReadOnlyList<LogMessage> messages,
        string filePath,
        Encoding encoding,
        CancellationToken cancellationToken = default)
    {
        if (messages == null || messages.Count == 0)
        {
            throw new ArgumentException("No messages to export.", nameof(messages));
        }

        if (string.IsNullOrEmpty(filePath))
        {
            throw new ArgumentException("File path cannot be empty.", nameof(filePath));
        }

        await using var writer = new StreamWriter(filePath, false, encoding);

        int total = messages.Count;
        for (int i = 0; i < total; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var line = FormatTextLine(messages[i]);
            await writer.WriteLineAsync(line);

            // Report progress every 100 messages or at the end
            if (i % 100 == 0 || i == total - 1)
            {
                ProgressChanged?.Invoke(this, new ExportProgressEventArgs(i + 1, total));
            }
        }
    }

    /// <summary>
    /// Gets the CSV header row.
    /// </summary>
    /// <param name="delimiter">The delimiter character.</param>
    /// <returns>The header row string.</returns>
    private static string GetCsvHeader(char delimiter)
    {
        return string.Join(delimiter.ToString(),
            EscapeCsvField("Index"),
            EscapeCsvField("Level"),
            EscapeCsvField("Timestamp"),
            EscapeCsvField("Logger"),
            EscapeCsvField("Message"));
    }

    /// <summary>
    /// Formats a log message as a CSV line.
    /// </summary>
    /// <param name="message">The message to format.</param>
    /// <param name="delimiter">The delimiter character.</param>
    /// <returns>The formatted CSV line.</returns>
    private static string FormatCsvLine(LogMessage message, char delimiter)
    {
        return string.Join(delimiter.ToString(),
            EscapeCsvField(message.Index.ToString()),
            EscapeCsvField(message.Level.ToString()),
            EscapeCsvField(message.Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff")),
            EscapeCsvField(message.Logger ?? string.Empty),
            EscapeCsvField(message.Message ?? string.Empty));
    }

    /// <summary>
    /// Formats a log message as a plain text line.
    /// </summary>
    /// <param name="message">The message to format.</param>
    /// <returns>The formatted text line.</returns>
    private static string FormatTextLine(LogMessage message)
    {
        return $"[{message.Timestamp:yyyy-MM-dd HH:mm:ss.fff}] [{message.Level,-7}] [{message.Logger ?? "Unknown",-30}] {message.Message}";
    }

    /// <summary>
    /// Escapes a field value for CSV output.
    /// </summary>
    /// <param name="field">The field value to escape.</param>
    /// <returns>The escaped field value.</returns>
    private static string EscapeCsvField(string field)
    {
        if (string.IsNullOrEmpty(field))
        {
            return "\"\"";
        }

        // If the field contains quotes, commas, or newlines, wrap in quotes and escape internal quotes
        if (field.Contains('"') || field.Contains(',') || field.Contains('\n') || field.Contains('\r'))
        {
            return $"\"{field.Replace("\"", "\"\"")}\"";
        }

        return $"\"{field}\"";
    }
}
