using Xunit;
using Couchcoding.Logbert.Receiver;
using FluentAssertions;

namespace Logbert.Tests.Receiver;

/// <summary>
/// Unit tests for ValidationResult struct.
/// </summary>
public class ValidationResultTests
{
    #region Success Tests

    [Fact]
    public async Task SuccessShouldReturnIsSuccessTrue()
    {
        // Arrange & Act
        var result = await Task.FromResult(ValidationResult.Success);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task SuccessShouldReturnEmptyErrorMessage()
    {
        // Arrange & Act
        var result = await Task.FromResult(ValidationResult.Success);

        // Assert
        result.ErrorMsg.Should().BeEmpty();
    }

    [Fact]
    public async Task SuccessShouldReturnSameInstanceOnMultipleCalls()
    {
        // Arrange & Act
        var result1 = await Task.FromResult(ValidationResult.Success);
        var result2 = await Task.FromResult(ValidationResult.Success);

        // Assert
        result1.IsSuccess.Should().Be(result2.IsSuccess);
        result1.ErrorMsg.Should().Be(result2.ErrorMsg);
    }

    #endregion

    #region Error Tests

    [Fact]
    public async Task ErrorShouldReturnIsSuccessFalse()
    {
        // Arrange & Act
        var result = await Task.FromResult(ValidationResult.Error("Test error"));

        // Assert
        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task ErrorShouldReturnProvidedErrorMessage()
    {
        // Arrange
        const string errorMessage = "This is a test error message";

        // Act
        var result = await Task.FromResult(ValidationResult.Error(errorMessage));

        // Assert
        result.ErrorMsg.Should().Be(errorMessage);
    }

    [Fact]
    public async Task ErrorShouldHandleEmptyMessage()
    {
        // Arrange & Act
        var result = await Task.FromResult(ValidationResult.Error(""));

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMsg.Should().BeEmpty();
    }

    [Fact]
    public async Task ErrorShouldPreserveSpecialCharactersInMessage()
    {
        // Arrange
        const string errorMessage = "Error: <special> \"characters\" & 'symbols' \n\t";

        // Act
        var result = await Task.FromResult(ValidationResult.Error(errorMessage));

        // Assert
        result.ErrorMsg.Should().Be(errorMessage);
    }

    [Fact]
    public async Task ErrorShouldHandleLongMessages()
    {
        // Arrange
        var errorMessage = new string('x', 10000);

        // Act
        var result = await Task.FromResult(ValidationResult.Error(errorMessage));

        // Assert
        result.ErrorMsg.Should().Be(errorMessage);
        result.ErrorMsg.Length.Should().Be(10000);
    }

    [Fact]
    public async Task ErrorShouldHandleUnicodeCharacters()
    {
        // Arrange
        const string errorMessage = "Error: 日本語 中文 한국어 🚀";

        // Act
        var result = await Task.FromResult(ValidationResult.Error(errorMessage));

        // Assert
        result.ErrorMsg.Should().Be(errorMessage);
    }

    #endregion

    #region Struct Behavior Tests

    [Fact]
    public async Task ValidationResultShouldBeValueType()
    {
        // Arrange & Act
        var result = await Task.FromResult(ValidationResult.Success);

        // Assert
        result.GetType().IsValueType.Should().BeTrue();
    }

    [Fact]
    public async Task ValidationResultShouldSupportCopying()
    {
        // Arrange
        var original = ValidationResult.Error("Original error");

        // Act
        var copy = await Task.FromResult(original);

        // Assert
        copy.IsSuccess.Should().Be(original.IsSuccess);
        copy.ErrorMsg.Should().Be(original.ErrorMsg);
    }

    [Fact]
    public async Task SuccessAndErrorShouldBeDifferent()
    {
        // Arrange & Act
        var success = await Task.FromResult(ValidationResult.Success);
        var error = await Task.FromResult(ValidationResult.Error("Error"));

        // Assert
        success.IsSuccess.Should().NotBe(error.IsSuccess);
    }

    #endregion
}
