using Xunit;
using Couchcoding.Logbert.Logging;
using AwesomeAssertions;

namespace Logbert.Tests.Logging;

/// <summary>
/// Unit tests for LogLevel enum.
/// </summary>
public class LogLevelTests
{
    [Fact]
    public async Task LogLevelShouldHaveTraceValue()
    {
        // Arrange & Act
        var level = await Task.FromResult(LogLevel.Trace);

        // Assert
        level.Should().Be(LogLevel.Trace);
    }

    [Fact]
    public async Task LogLevelShouldHaveDebugValue()
    {
        // Arrange & Act
        var level = await Task.FromResult(LogLevel.Debug);

        // Assert
        level.Should().Be(LogLevel.Debug);
    }

    [Fact]
    public async Task LogLevelShouldHaveInfoValue()
    {
        // Arrange & Act
        var level = await Task.FromResult(LogLevel.Info);

        // Assert
        level.Should().Be(LogLevel.Info);
    }

    [Fact]
    public async Task LogLevelShouldHaveWarningValue()
    {
        // Arrange & Act
        var level = await Task.FromResult(LogLevel.Warning);

        // Assert
        level.Should().Be(LogLevel.Warning);
    }

    [Fact]
    public async Task LogLevelShouldHaveErrorValue()
    {
        // Arrange & Act
        var level = await Task.FromResult(LogLevel.Error);

        // Assert
        level.Should().Be(LogLevel.Error);
    }

    [Fact]
    public async Task LogLevelShouldHaveFatalValue()
    {
        // Arrange & Act
        var level = await Task.FromResult(LogLevel.Fatal);

        // Assert
        level.Should().Be(LogLevel.Fatal);
    }

    [Fact]
    public async Task TraceShouldBeLessThanDebug()
    {
        // Arrange & Act
        var trace = await Task.FromResult((int)LogLevel.Trace);
        var debug = await Task.FromResult((int)LogLevel.Debug);

        // Assert
        trace.Should().BeLessThan(debug);
    }

    [Fact]
    public async Task DebugShouldBeLessThanInfo()
    {
        // Arrange & Act
        var debug = await Task.FromResult((int)LogLevel.Debug);
        var info = await Task.FromResult((int)LogLevel.Info);

        // Assert
        debug.Should().BeLessThan(info);
    }

    [Fact]
    public async Task InfoShouldBeLessThanWarning()
    {
        // Arrange & Act
        var info = await Task.FromResult((int)LogLevel.Info);
        var warning = await Task.FromResult((int)LogLevel.Warning);

        // Assert
        info.Should().BeLessThan(warning);
    }

    [Fact]
    public async Task WarningShouldBeLessThanError()
    {
        // Arrange & Act
        var warning = await Task.FromResult((int)LogLevel.Warning);
        var error = await Task.FromResult((int)LogLevel.Error);

        // Assert
        warning.Should().BeLessThan(error);
    }

    [Fact]
    public async Task ErrorShouldBeLessThanFatal()
    {
        // Arrange & Act
        var error = await Task.FromResult((int)LogLevel.Error);
        var fatal = await Task.FromResult((int)LogLevel.Fatal);

        // Assert
        error.Should().BeLessThan(fatal);
    }

    [Fact]
    public async Task LogLevelShouldConvertToString()
    {
        // Arrange & Act
        var level = LogLevel.Error;
        var str = await Task.FromResult(level.ToString());

        // Assert
        str.Should().Be("Error");
    }

    [Fact]
    public async Task LogLevelShouldParseFromString()
    {
        // Arrange & Act
        var parsed = await Task.FromResult(Enum.Parse<LogLevel>("Warning"));

        // Assert
        parsed.Should().Be(LogLevel.Warning);
    }

    [Fact]
    public async Task LogLevelShouldSupportTryParse()
    {
        // Arrange & Act
        var success = Enum.TryParse<LogLevel>("Info", out var result);
        await Task.CompletedTask;

        // Assert
        success.Should().BeTrue();
        result.Should().Be(LogLevel.Info);
    }

    [Fact]
    public async Task LogLevelShouldHaveSixValues()
    {
        // Arrange & Act
        var values = await Task.FromResult(Enum.GetValues<LogLevel>());

        // Assert
        values.Should().HaveCount(6);
    }

    [Fact]
    public async Task LogLevelNamesShouldBeCorrect()
    {
        // Arrange & Act
        var names = await Task.FromResult(Enum.GetNames<LogLevel>());

        // Assert
        names.Should().Contain("Trace");
        names.Should().Contain("Debug");
        names.Should().Contain("Info");
        names.Should().Contain("Warning");
        names.Should().Contain("Error");
        names.Should().Contain("Fatal");
    }
}
