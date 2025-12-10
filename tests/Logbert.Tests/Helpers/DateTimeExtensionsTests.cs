using Xunit;
using Logbert.Helper;
using FluentAssertions;

namespace Logbert.Tests.Helpers;

/// <summary>
/// Unit tests for DateTimeExtensions class.
/// </summary>
public class DateTimeExtensionsTests
{
    [Fact]
    public async Task ToUnixTimestampShouldReturnZeroForUnixEpoch()
    {
        // Arrange
        var epochDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // Act
        var result = await Task.FromResult(epochDate.ToUnixTimestamp());

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public async Task ToUnixTimestampShouldReturnCorrectValueForKnownDate()
    {
        // Arrange
        // January 1, 2000 00:00:00 UTC = 946684800 seconds since epoch
        var date = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // Act
        var result = await Task.FromResult(date.ToUnixTimestamp());

        // Assert
        result.Should().Be(946684800);
    }

    [Fact]
    public async Task ToUnixTimestampShouldReturnPositiveValueForDateAfterEpoch()
    {
        // Arrange
        var date = new DateTime(2024, 1, 15, 12, 0, 0, DateTimeKind.Utc);

        // Act
        var result = await Task.FromResult(date.ToUnixTimestamp());

        // Assert
        result.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task ToUnixTimestampShouldReturnNegativeValueForDateBeforeEpoch()
    {
        // Arrange
        var date = new DateTime(1969, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // Act
        var result = await Task.FromResult(date.ToUnixTimestamp());

        // Assert
        result.Should().BeLessThan(0);
    }

    [Fact]
    public async Task ToUnixTimestampShouldHandleDateTimeMinValue()
    {
        // Arrange
        var date = DateTime.MinValue;

        // Act
        var result = await Task.FromResult(date.ToUnixTimestamp());

        // Assert
        result.Should().BeLessThan(0);
    }

    [Fact]
    public async Task ToUnixTimestampShouldIncrementByOneForOneSecondDifference()
    {
        // Arrange
        var date1 = new DateTime(2024, 1, 15, 12, 0, 0, DateTimeKind.Utc);
        var date2 = new DateTime(2024, 1, 15, 12, 0, 1, DateTimeKind.Utc);

        // Act
        var result1 = await Task.FromResult(date1.ToUnixTimestamp());
        var result2 = await Task.FromResult(date2.ToUnixTimestamp());

        // Assert
        (result2 - result1).Should().Be(1);
    }

    [Fact]
    public async Task ToUnixTimestampShouldHandleMillenniumDate()
    {
        // Arrange
        // December 31, 1999 23:59:59 UTC = 946684799
        var date = new DateTime(1999, 12, 31, 23, 59, 59, DateTimeKind.Utc);

        // Act
        var result = await Task.FromResult(date.ToUnixTimestamp());

        // Assert
        result.Should().Be(946684799);
    }

    [Fact]
    public async Task ToUnixTimestampShouldReturnConsistentResultsForSameDate()
    {
        // Arrange
        var date = new DateTime(2024, 6, 15, 10, 30, 45, DateTimeKind.Utc);

        // Act
        var result1 = await Task.FromResult(date.ToUnixTimestamp());
        var result2 = await Task.FromResult(date.ToUnixTimestamp());

        // Assert
        result1.Should().Be(result2);
    }
}
