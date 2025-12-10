using Xunit;
using System.Text;
using Couchcoding.Logbert.Helper;
using AwesomeAssertions;

namespace Logbert.Tests.Helpers;

/// <summary>
/// Unit tests for EncodingWrapper class.
/// </summary>
public class EncodingWrapperTests
{
    [Fact]
    public async Task EncodingWrapperShouldStoreEncodingInfo()
    {
        // Arrange
        var encodingInfo = Encoding.UTF8.EncodingName;

        // Act
        var utf8Info = Encoding.GetEncodings().First(e => e.Name == "utf-8");
        var wrapper = await Task.FromResult(new EncodingWrapper(utf8Info));

        // Assert
        wrapper.Encoding.Should().NotBeNull();
    }

    [Fact]
    public async Task CodepageShouldReturnCorrectValueForUtf8()
    {
        // Arrange
        var utf8Info = Encoding.GetEncodings().First(e => e.Name == "utf-8");
        var wrapper = new EncodingWrapper(utf8Info);

        // Act
        var codepage = await Task.FromResult(wrapper.Codepage);

        // Assert
        codepage.Should().Be(65001); // UTF-8 codepage
    }

    [Fact]
    public async Task CodepageShouldReturnCorrectValueForAscii()
    {
        // Arrange
        var asciiInfo = Encoding.GetEncodings().First(e => e.Name == "us-ascii");
        var wrapper = new EncodingWrapper(asciiInfo);

        // Act
        var codepage = await Task.FromResult(wrapper.Codepage);

        // Assert
        codepage.Should().Be(20127); // ASCII codepage
    }

    [Fact]
    public async Task ToStringShouldReturnDisplayName()
    {
        // Arrange
        var utf8Info = Encoding.GetEncodings().First(e => e.Name == "utf-8");
        var wrapper = new EncodingWrapper(utf8Info);

        // Act
        var displayName = await Task.FromResult(wrapper.ToString());

        // Assert
        displayName.Should().NotBeNullOrEmpty();
        displayName.Should().Be(utf8Info.DisplayName);
    }

    [Fact]
    public async Task DifferentEncodingsShouldHaveDifferentCodepages()
    {
        // Arrange
        var utf8Info = Encoding.GetEncodings().First(e => e.Name == "utf-8");
        var asciiInfo = Encoding.GetEncodings().First(e => e.Name == "us-ascii");
        var utf8Wrapper = new EncodingWrapper(utf8Info);
        var asciiWrapper = new EncodingWrapper(asciiInfo);

        // Act
        var utf8Codepage = await Task.FromResult(utf8Wrapper.Codepage);
        var asciiCodepage = await Task.FromResult(asciiWrapper.Codepage);

        // Assert
        utf8Codepage.Should().NotBe(asciiCodepage);
    }

    [Fact]
    public async Task EncodingPropertyShouldMatchConstructorArgument()
    {
        // Arrange
        var encodingInfo = Encoding.GetEncodings().First(e => e.Name == "utf-8");

        // Act
        var wrapper = await Task.FromResult(new EncodingWrapper(encodingInfo));

        // Assert
        wrapper.Encoding.Should().Be(encodingInfo);
    }

    [Fact]
    public async Task ShouldSupportUnicodeEncoding()
    {
        // Arrange
        var unicodeInfo = Encoding.GetEncodings().First(e => e.Name == "utf-16");
        var wrapper = new EncodingWrapper(unicodeInfo);

        // Act
        var codepage = await Task.FromResult(wrapper.Codepage);

        // Assert
        codepage.Should().Be(1200); // UTF-16 LE codepage
    }

    [Fact]
    public async Task ShouldSupportLatin1Encoding()
    {
        // Arrange
        var latin1Info = Encoding.GetEncodings().FirstOrDefault(e => e.CodePage == 28591);
        if (latin1Info == null)
        {
            // Skip test if Latin1 is not available on this platform
            await Task.CompletedTask;
            return;
        }

        var wrapper = new EncodingWrapper(latin1Info);

        // Act
        var codepage = await Task.FromResult(wrapper.Codepage);

        // Assert
        codepage.Should().Be(28591);
    }
}
