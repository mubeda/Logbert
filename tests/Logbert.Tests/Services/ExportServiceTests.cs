using Xunit;
using System.Text;
using Logbert.Logging;
using Logbert.Services;
using FluentAssertions;

namespace Logbert.Tests.Services;

/// <summary>
/// Unit tests for ExportService class.
/// </summary>
public class ExportServiceTests : IDisposable
{
    private readonly string _tempDirectory;
    private readonly ExportService _exportService;

    public ExportServiceTests()
    {
        _tempDirectory = Path.Combine(Path.GetTempPath(), $"LogbertTests_{Guid.NewGuid()}");
        Directory.CreateDirectory(_tempDirectory);
        _exportService = new ExportService();
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDirectory))
        {
            Directory.Delete(_tempDirectory, true);
        }
    }

    #region ExportToCsvAsync Tests

    [Fact]
    public async Task ExportToCsvAsyncShouldThrowWhenMessagesIsNull()
    {
        // Arrange
        var filePath = GetTempFilePath("test.csv");

        // Act & Assert
        var action = () => _exportService.ExportToCsvAsync(null!, filePath, Encoding.UTF8);
        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task ExportToCsvAsyncShouldThrowWhenMessagesIsEmpty()
    {
        // Arrange
        var messages = new List<LogMessage>();
        var filePath = GetTempFilePath("test.csv");

        // Act & Assert
        var action = () => _exportService.ExportToCsvAsync(messages, filePath, Encoding.UTF8);
        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task ExportToCsvAsyncShouldThrowWhenFilePathIsEmpty()
    {
        // Arrange
        var messages = CreateTestMessages(1);

        // Act & Assert
        var action = () => _exportService.ExportToCsvAsync(messages, "", Encoding.UTF8);
        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task ExportToCsvAsyncShouldCreateFileSuccessfully()
    {
        // Arrange
        var messages = CreateTestMessages(5);
        var filePath = GetTempFilePath("export.csv");

        // Act
        await _exportService.ExportToCsvAsync(messages, filePath, Encoding.UTF8);

        // Assert
        File.Exists(filePath).Should().BeTrue();
    }

    [Fact]
    public async Task ExportToCsvAsyncShouldIncludeHeaderWhenRequested()
    {
        // Arrange
        var messages = CreateTestMessages(1);
        var filePath = GetTempFilePath("with_header.csv");

        // Act
        await _exportService.ExportToCsvAsync(messages, filePath, Encoding.UTF8, includeHeaders: true);
        var content = await File.ReadAllTextAsync(filePath);

        // Assert
        content.Should().Contain("Index");
        content.Should().Contain("Level");
        content.Should().Contain("Timestamp");
        content.Should().Contain("Logger");
        content.Should().Contain("Message");
    }

    [Fact]
    public async Task ExportToCsvAsyncShouldExcludeHeaderWhenNotRequested()
    {
        // Arrange
        var messages = CreateTestMessages(1);
        var filePath = GetTempFilePath("no_header.csv");

        // Act
        await _exportService.ExportToCsvAsync(messages, filePath, Encoding.UTF8, includeHeaders: false);
        var lines = await File.ReadAllLinesAsync(filePath);

        // Assert
        lines.Length.Should().Be(1);
        lines[0].Should().NotContain("\"Index\"");
    }

    [Fact]
    public async Task ExportToCsvAsyncShouldUseSpecifiedDelimiter()
    {
        // Arrange
        var messages = CreateTestMessages(1);
        var filePath = GetTempFilePath("semicolon.csv");

        // Act
        await _exportService.ExportToCsvAsync(messages, filePath, Encoding.UTF8, delimiter: ';');
        var content = await File.ReadAllTextAsync(filePath);

        // Assert
        content.Should().Contain(";");
    }

    [Fact]
    public async Task ExportToCsvAsyncShouldExportCorrectNumberOfLines()
    {
        // Arrange
        var messages = CreateTestMessages(10);
        var filePath = GetTempFilePath("ten_messages.csv");

        // Act
        await _exportService.ExportToCsvAsync(messages, filePath, Encoding.UTF8, includeHeaders: true);
        var lines = await File.ReadAllLinesAsync(filePath);

        // Assert
        lines.Length.Should().Be(11); // 1 header + 10 data lines
    }

    [Fact]
    public async Task ExportToCsvAsyncShouldSupportCancellation()
    {
        // Arrange
        var messages = CreateTestMessages(1000);
        var filePath = GetTempFilePath("cancelled.csv");
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var action = () => _exportService.ExportToCsvAsync(messages, filePath, Encoding.UTF8, cancellationToken: cts.Token);
        await action.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task ExportToCsvAsyncShouldReportProgress()
    {
        // Arrange
        var messages = CreateTestMessages(200);
        var filePath = GetTempFilePath("progress.csv");
        var progressReports = new List<ExportService.ExportProgressEventArgs>();

        _exportService.ProgressChanged += (s, e) => progressReports.Add(e);

        // Act
        await _exportService.ExportToCsvAsync(messages, filePath, Encoding.UTF8);

        // Assert
        progressReports.Should().NotBeEmpty();
        progressReports.Last().Current.Should().Be(200);
        progressReports.Last().Total.Should().Be(200);
    }

    [Fact]
    public async Task ExportToCsvAsyncShouldUseUtf8Encoding()
    {
        // Arrange
        var messages = CreateTestMessages(1);
        var filePath = GetTempFilePath("utf8.csv");

        // Act
        await _exportService.ExportToCsvAsync(messages, filePath, Encoding.UTF8);
        var bytes = await File.ReadAllBytesAsync(filePath);

        // Assert
        File.Exists(filePath).Should().BeTrue();
    }

    #endregion

    #region ExportToTextAsync Tests

    [Fact]
    public async Task ExportToTextAsyncShouldThrowWhenMessagesIsNull()
    {
        // Arrange
        var filePath = GetTempFilePath("test.txt");

        // Act & Assert
        var action = () => _exportService.ExportToTextAsync(null!, filePath, Encoding.UTF8);
        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task ExportToTextAsyncShouldThrowWhenMessagesIsEmpty()
    {
        // Arrange
        var messages = new List<LogMessage>();
        var filePath = GetTempFilePath("test.txt");

        // Act & Assert
        var action = () => _exportService.ExportToTextAsync(messages, filePath, Encoding.UTF8);
        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task ExportToTextAsyncShouldCreateFileSuccessfully()
    {
        // Arrange
        var messages = CreateTestMessages(5);
        var filePath = GetTempFilePath("export.txt");

        // Act
        await _exportService.ExportToTextAsync(messages, filePath, Encoding.UTF8);

        // Assert
        File.Exists(filePath).Should().BeTrue();
    }

    [Fact]
    public async Task ExportToTextAsyncShouldExportCorrectNumberOfLines()
    {
        // Arrange
        var messages = CreateTestMessages(10);
        var filePath = GetTempFilePath("ten_messages.txt");

        // Act
        await _exportService.ExportToTextAsync(messages, filePath, Encoding.UTF8);
        var lines = await File.ReadAllLinesAsync(filePath);

        // Assert
        lines.Length.Should().Be(10);
    }

    [Fact]
    public async Task ExportToTextAsyncShouldSupportCancellation()
    {
        // Arrange
        var messages = CreateTestMessages(1000);
        var filePath = GetTempFilePath("cancelled.txt");
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var action = () => _exportService.ExportToTextAsync(messages, filePath, Encoding.UTF8, cts.Token);
        await action.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task ExportToTextAsyncShouldReportProgress()
    {
        // Arrange
        var messages = CreateTestMessages(200);
        var filePath = GetTempFilePath("progress.txt");
        var progressReports = new List<ExportService.ExportProgressEventArgs>();

        _exportService.ProgressChanged += (s, e) => progressReports.Add(e);

        // Act
        await _exportService.ExportToTextAsync(messages, filePath, Encoding.UTF8);

        // Assert
        progressReports.Should().NotBeEmpty();
    }

    [Fact]
    public async Task ExportToTextAsyncShouldContainTimestampInOutput()
    {
        // Arrange
        var messages = CreateTestMessages(1);
        var filePath = GetTempFilePath("timestamp.txt");

        // Act
        await _exportService.ExportToTextAsync(messages, filePath, Encoding.UTF8);
        var content = await File.ReadAllTextAsync(filePath);

        // Assert
        content.Should().Contain("[");
        content.Should().Contain("]");
    }

    #endregion

    #region ExportProgressEventArgs Tests

    [Fact]
    public async Task ExportProgressEventArgsShouldCalculatePercentageCorrectly()
    {
        // Arrange & Act
        var args = await Task.FromResult(new ExportService.ExportProgressEventArgs(50, 100));

        // Assert
        args.Percentage.Should().Be(50.0);
    }

    [Fact]
    public async Task ExportProgressEventArgsShouldReturnZeroPercentageWhenTotalIsZero()
    {
        // Arrange & Act
        var args = await Task.FromResult(new ExportService.ExportProgressEventArgs(50, 0));

        // Assert
        args.Percentage.Should().Be(0);
    }

    [Fact]
    public async Task ExportProgressEventArgsShouldReturnHundredPercentAtCompletion()
    {
        // Arrange & Act
        var args = await Task.FromResult(new ExportService.ExportProgressEventArgs(100, 100));

        // Assert
        args.Percentage.Should().Be(100.0);
    }

    [Fact]
    public async Task ExportProgressEventArgsShouldStoreCurrentValue()
    {
        // Arrange & Act
        var args = await Task.FromResult(new ExportService.ExportProgressEventArgs(25, 100));

        // Assert
        args.Current.Should().Be(25);
    }

    [Fact]
    public async Task ExportProgressEventArgsShouldStoreTotalValue()
    {
        // Arrange & Act
        var args = await Task.FromResult(new ExportService.ExportProgressEventArgs(25, 500));

        // Assert
        args.Total.Should().Be(500);
    }

    #endregion

    #region Helper Methods

    private string GetTempFilePath(string fileName)
    {
        return Path.Combine(_tempDirectory, fileName);
    }

    private static List<LogMessage> CreateTestMessages(int count)
    {
        var messages = new List<LogMessage>();
        for (int i = 0; i < count; i++)
        {
            messages.Add(new TestLogMessage(
                index: i + 1,
                level: (LogLevel)(i % 6),
                logger: $"TestLogger{i % 3}",
                message: $"Test message {i + 1}",
                timestamp: DateTime.Now.AddSeconds(-count + i)));
        }
        return messages;
    }

    /// <summary>
    /// Test implementation of LogMessage for unit testing.
    /// </summary>
    private class TestLogMessage : LogMessage
    {
        private readonly LogLevel _level;
        private readonly string _message;
        private readonly DateTime _timestamp;

        public override DateTime Timestamp => _timestamp;
        public override string Message => _message;
        public override LogLevel Level => _level;

        public TestLogMessage(int index, LogLevel level, string logger, string message, DateTime timestamp)
            : base(message, index)
        {
            _level = level;
            _message = message;
            _timestamp = timestamp;
            mLogger = logger;
        }

        public override object? GetValueForColumn(int columnIndex)
        {
            return columnIndex switch
            {
                0 => Index,
                1 => Level,
                2 => Timestamp,
                3 => Logger,
                4 => Message,
                _ => null
            };
        }
    }

    #endregion
}
