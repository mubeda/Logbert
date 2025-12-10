using Xunit;
using Couchcoding.Logbert.Helper;
using FluentAssertions;

namespace Logbert.Tests.Helpers;

/// <summary>
/// Unit tests for StringExtensions class.
/// </summary>
public class StringExtensionsTests
{
    #region ToCsv Tests

    [Fact]
    public async Task ToCsvShouldReturnEmptyStringWhenInputIsEmpty()
    {
        // Arrange
        string input = "";

        // Act
        var result = await Task.FromResult(input.ToCsv());

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task ToCsvShouldReturnSameStringWhenNoQuotesPresent()
    {
        // Arrange
        string input = "Hello World";

        // Act
        var result = await Task.FromResult(input.ToCsv());

        // Assert
        result.Should().Be("Hello World");
    }

    [Fact]
    public async Task ToCsvShouldEscapeDoubleQuotes()
    {
        // Arrange
        string input = "Hello \"World\"";

        // Act
        var result = await Task.FromResult(input.ToCsv());

        // Assert
        result.Should().Be("Hello \"\"World\"\"");
    }

    [Fact]
    public async Task ToCsvShouldEscapeMultipleDoubleQuotes()
    {
        // Arrange
        string input = "\"Hello\" \"World\"";

        // Act
        var result = await Task.FromResult(input.ToCsv());

        // Assert
        result.Should().Be("\"\"Hello\"\" \"\"World\"\"");
    }

    [Fact]
    public async Task ToCsvShouldHandleIntegerValues()
    {
        // Arrange
        int input = 42;

        // Act
        var result = await Task.FromResult(input.ToCsv());

        // Assert
        result.Should().Be("42");
    }

    [Fact]
    public async Task ToCsvShouldHandleDateTimeValues()
    {
        // Arrange
        var input = new DateTime(2024, 1, 15, 14, 30, 0);

        // Act
        var result = await Task.FromResult(input.ToCsv());

        // Assert
        result.Should().Contain("2024");
    }

    #endregion

    #region ToRegex Tests

    [Fact]
    public async Task ToRegexShouldReturnEmptyStringWhenInputIsEmpty()
    {
        // Arrange
        string input = "";

        // Act
        var result = await Task.FromResult(input.ToRegex());

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task ToRegexShouldReturnEmptyStringWhenInputIsNull()
    {
        // Arrange
        string? input = null;

        // Act
        var result = await Task.FromResult(input!.ToRegex());

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task ToRegexShouldConvertAsteriskToWildcard()
    {
        // Arrange
        string input = "*.txt";

        // Act
        var result = await Task.FromResult(input.ToRegex());

        // Assert
        result.Should().Be(@".*\.txt");
    }

    [Fact]
    public async Task ToRegexShouldConvertQuestionMarkToSingleCharacter()
    {
        // Arrange
        string input = "file?.log";

        // Act
        var result = await Task.FromResult(input.ToRegex());

        // Assert
        result.Should().Be(@"file.\.log");
    }

    [Fact]
    public async Task ToRegexShouldConvertMultipleWildcards()
    {
        // Arrange
        string input = "test*file?.txt";

        // Act
        var result = await Task.FromResult(input.ToRegex());

        // Assert
        result.Should().Be(@"test.*file.\.txt");
    }

    [Fact]
    public async Task ToRegexShouldEscapeSpecialRegexCharacters()
    {
        // Arrange
        string input = "file[1].txt";

        // Act
        var result = await Task.FromResult(input.ToRegex());

        // Assert
        // Note: Regex.Escape only escapes '[' not ']' in this context
        result.Should().Be(@"file\[1]\.txt");
    }

    [Fact]
    public async Task ToRegexShouldEscapeDotCharacter()
    {
        // Arrange
        string input = "config.json";

        // Act
        var result = await Task.FromResult(input.ToRegex());

        // Assert
        result.Should().Be(@"config\.json");
    }

    [Fact]
    public async Task ToRegexShouldHandleComplexPattern()
    {
        // Arrange
        string input = "logs/*.log.?";

        // Act
        var result = await Task.FromResult(input.ToRegex());

        // Assert
        result.Should().Be(@"logs/.*\.log\..");
    }

    #endregion
}
