using Xunit;
using Couchcoding.Logbert.Models;
using FluentAssertions;
using System.Text.Json;

namespace Logbert.Tests.Models;

/// <summary>
/// Unit tests for AppSettings class.
/// </summary>
public class AppSettingsTests
{
    #region Default Values Tests

    [Fact]
    public async Task DefaultWindowXShouldBe100()
    {
        // Arrange & Act
        var settings = await Task.FromResult(new AppSettings());

        // Assert
        settings.WindowX.Should().Be(100);
    }

    [Fact]
    public async Task DefaultWindowYShouldBe100()
    {
        // Arrange & Act
        var settings = await Task.FromResult(new AppSettings());

        // Assert
        settings.WindowY.Should().Be(100);
    }

    [Fact]
    public async Task DefaultWindowWidthShouldBe1200()
    {
        // Arrange & Act
        var settings = await Task.FromResult(new AppSettings());

        // Assert
        settings.WindowWidth.Should().Be(1200);
    }

    [Fact]
    public async Task DefaultWindowHeightShouldBe800()
    {
        // Arrange & Act
        var settings = await Task.FromResult(new AppSettings());

        // Assert
        settings.WindowHeight.Should().Be(800);
    }

    [Fact]
    public async Task DefaultWindowStateShouldBeNormal()
    {
        // Arrange & Act
        var settings = await Task.FromResult(new AppSettings());

        // Assert
        settings.WindowState.Should().Be("Normal");
    }

    [Fact]
    public async Task DefaultLeftPanelWidthShouldBe200()
    {
        // Arrange & Act
        var settings = await Task.FromResult(new AppSettings());

        // Assert
        settings.LeftPanelWidth.Should().Be(200);
    }

    [Fact]
    public async Task DefaultRightPanelWidthShouldBe200()
    {
        // Arrange & Act
        var settings = await Task.FromResult(new AppSettings());

        // Assert
        settings.RightPanelWidth.Should().Be(200);
    }

    [Fact]
    public async Task DefaultBottomPanelHeightShouldBe150()
    {
        // Arrange & Act
        var settings = await Task.FromResult(new AppSettings());

        // Assert
        settings.BottomPanelHeight.Should().Be(150);
    }

    [Fact]
    public async Task DefaultLoggerTreeVisibleShouldBeTrue()
    {
        // Arrange & Act
        var settings = await Task.FromResult(new AppSettings());

        // Assert
        settings.LoggerTreeVisible.Should().BeTrue();
    }

    [Fact]
    public async Task DefaultBookmarksVisibleShouldBeTrue()
    {
        // Arrange & Act
        var settings = await Task.FromResult(new AppSettings());

        // Assert
        settings.BookmarksVisible.Should().BeTrue();
    }

    [Fact]
    public async Task DefaultDetailsVisibleShouldBeTrue()
    {
        // Arrange & Act
        var settings = await Task.FromResult(new AppSettings());

        // Assert
        settings.DetailsVisible.Should().BeTrue();
    }

    [Fact]
    public async Task DefaultFilterVisibleShouldBeTrue()
    {
        // Arrange & Act
        var settings = await Task.FromResult(new AppSettings());

        // Assert
        settings.FilterVisible.Should().BeTrue();
    }

    [Fact]
    public async Task DefaultAlwaysOnTopShouldBeFalse()
    {
        // Arrange & Act
        var settings = await Task.FromResult(new AppSettings());

        // Assert
        settings.AlwaysOnTop.Should().BeFalse();
    }

    [Fact]
    public async Task DefaultMinimizeToTrayShouldBeFalse()
    {
        // Arrange & Act
        var settings = await Task.FromResult(new AppSettings());

        // Assert
        settings.MinimizeToTray.Should().BeFalse();
    }

    [Fact]
    public async Task DefaultShowWelcomeScreenShouldBeTrue()
    {
        // Arrange & Act
        var settings = await Task.FromResult(new AppSettings());

        // Assert
        settings.ShowWelcomeScreen.Should().BeTrue();
    }

    [Fact]
    public async Task DefaultMaxRecentFilesShouldBe10()
    {
        // Arrange & Act
        var settings = await Task.FromResult(new AppSettings());

        // Assert
        settings.MaxRecentFiles.Should().Be(10);
    }

    [Fact]
    public async Task DefaultSelectedThemeShouldBeSystem()
    {
        // Arrange & Act
        var settings = await Task.FromResult(new AppSettings());

        // Assert
        settings.SelectedTheme.Should().Be("System");
    }

    [Fact]
    public async Task DefaultEnableLoggingShouldBeFalse()
    {
        // Arrange & Act
        var settings = await Task.FromResult(new AppSettings());

        // Assert
        settings.EnableLogging.Should().BeFalse();
    }

    [Fact]
    public async Task DefaultLogRetentionDaysShouldBe7()
    {
        // Arrange & Act
        var settings = await Task.FromResult(new AppSettings());

        // Assert
        settings.LogRetentionDays.Should().Be(7);
    }

    [Fact]
    public async Task DefaultFontFamilyShouldBeConsolas()
    {
        // Arrange & Act
        var settings = await Task.FromResult(new AppSettings());

        // Assert
        settings.DefaultFontFamily.Should().Be("Consolas");
    }

    [Fact]
    public async Task DefaultFontSizeShouldBe11()
    {
        // Arrange & Act
        var settings = await Task.FromResult(new AppSettings());

        // Assert
        settings.DefaultFontSize.Should().Be(11);
    }

    [Fact]
    public async Task DefaultAutoScrollShouldBeTrue()
    {
        // Arrange & Act
        var settings = await Task.FromResult(new AppSettings());

        // Assert
        settings.AutoScroll.Should().BeTrue();
    }

    [Fact]
    public async Task DefaultShowTimestampsShouldBeTrue()
    {
        // Arrange & Act
        var settings = await Task.FromResult(new AppSettings());

        // Assert
        settings.ShowTimestamps.Should().BeTrue();
    }

    [Fact]
    public async Task DefaultTimestampFormatShouldBeCorrect()
    {
        // Arrange & Act
        var settings = await Task.FromResult(new AppSettings());

        // Assert
        settings.TimestampFormat.Should().Be("yyyy-MM-dd HH:mm:ss.fff");
    }

    [Fact]
    public async Task DefaultRecentFilesShouldBeEmpty()
    {
        // Arrange & Act
        var settings = await Task.FromResult(new AppSettings());

        // Assert
        settings.RecentFiles.Should().BeEmpty();
    }

    #endregion

    #region Column Widths Tests

    [Fact]
    public async Task DefaultColumnWidthsShouldContainExpectedColumns()
    {
        // Arrange & Act
        var settings = await Task.FromResult(new AppSettings());

        // Assert
        settings.ColumnWidths.Should().ContainKey("Number");
        settings.ColumnWidths.Should().ContainKey("Level");
        settings.ColumnWidths.Should().ContainKey("Timestamp");
        settings.ColumnWidths.Should().ContainKey("Logger");
        settings.ColumnWidths.Should().ContainKey("Thread");
        settings.ColumnWidths.Should().ContainKey("Message");
    }

    [Fact]
    public async Task DefaultNumberColumnWidthShouldBe80()
    {
        // Arrange & Act
        var settings = await Task.FromResult(new AppSettings());

        // Assert
        settings.ColumnWidths["Number"].Should().Be(80);
    }

    [Fact]
    public async Task DefaultMessageColumnWidthShouldBe500()
    {
        // Arrange & Act
        var settings = await Task.FromResult(new AppSettings());

        // Assert
        settings.ColumnWidths["Message"].Should().Be(500);
    }

    #endregion

    #region Serialization Tests

    [Fact]
    public async Task AppSettingsShouldSerializeToJson()
    {
        // Arrange
        var settings = new AppSettings
        {
            WindowX = 200,
            WindowY = 300,
            SelectedTheme = "Dark"
        };

        // Act
        var json = await Task.FromResult(JsonSerializer.Serialize(settings));

        // Assert
        json.Should().Contain("200");
        json.Should().Contain("300");
        json.Should().Contain("Dark");
    }

    [Fact]
    public async Task AppSettingsShouldDeserializeFromJson()
    {
        // Arrange
        var json = """
        {
            "WindowX": 500,
            "WindowY": 600,
            "WindowWidth": 1400,
            "WindowHeight": 900,
            "SelectedTheme": "Light"
        }
        """;

        // Act
        var settings = await Task.FromResult(JsonSerializer.Deserialize<AppSettings>(json));

        // Assert
        settings.Should().NotBeNull();
        settings!.WindowX.Should().Be(500);
        settings.WindowY.Should().Be(600);
        settings.WindowWidth.Should().Be(1400);
        settings.WindowHeight.Should().Be(900);
        settings.SelectedTheme.Should().Be("Light");
    }

    [Fact]
    public async Task AppSettingsShouldRoundTripSerialize()
    {
        // Arrange
        var original = new AppSettings
        {
            WindowX = 123,
            WindowY = 456,
            WindowWidth = 1500,
            WindowHeight = 1000,
            WindowState = "Maximized",
            SelectedTheme = "Dark",
            AutoScroll = false,
            DefaultFontSize = 14
        };

        // Act
        var json = JsonSerializer.Serialize(original);
        var deserialized = await Task.FromResult(JsonSerializer.Deserialize<AppSettings>(json));

        // Assert
        deserialized.Should().NotBeNull();
        deserialized!.WindowX.Should().Be(original.WindowX);
        deserialized.WindowY.Should().Be(original.WindowY);
        deserialized.WindowWidth.Should().Be(original.WindowWidth);
        deserialized.WindowHeight.Should().Be(original.WindowHeight);
        deserialized.WindowState.Should().Be(original.WindowState);
        deserialized.SelectedTheme.Should().Be(original.SelectedTheme);
        deserialized.AutoScroll.Should().Be(original.AutoScroll);
        deserialized.DefaultFontSize.Should().Be(original.DefaultFontSize);
    }

    [Fact]
    public async Task AppSettingsShouldSerializeRecentFiles()
    {
        // Arrange
        var settings = new AppSettings();
        settings.RecentFiles.Add("file1.log");
        settings.RecentFiles.Add("file2.log");

        // Act
        var json = JsonSerializer.Serialize(settings);
        var deserialized = await Task.FromResult(JsonSerializer.Deserialize<AppSettings>(json));

        // Assert
        deserialized.Should().NotBeNull();
        deserialized!.RecentFiles.Should().HaveCount(2);
        deserialized.RecentFiles.Should().Contain("file1.log");
        deserialized.RecentFiles.Should().Contain("file2.log");
    }

    [Fact]
    public async Task AppSettingsShouldSerializeColumnWidths()
    {
        // Arrange
        var settings = new AppSettings();
        settings.ColumnWidths["Custom"] = 250;

        // Act
        var json = JsonSerializer.Serialize(settings);
        var deserialized = await Task.FromResult(JsonSerializer.Deserialize<AppSettings>(json));

        // Assert
        deserialized.Should().NotBeNull();
        deserialized!.ColumnWidths.Should().ContainKey("Custom");
        deserialized.ColumnWidths["Custom"].Should().Be(250);
    }

    #endregion

    #region Property Modification Tests

    [Fact]
    public async Task ShouldAllowModifyingWindowPosition()
    {
        // Arrange
        var settings = new AppSettings();

        // Act
        settings.WindowX = 500;
        settings.WindowY = 600;
        var result = await Task.FromResult(settings);

        // Assert
        result.WindowX.Should().Be(500);
        result.WindowY.Should().Be(600);
    }

    [Fact]
    public async Task ShouldAllowAddingRecentFiles()
    {
        // Arrange
        var settings = new AppSettings();

        // Act
        settings.RecentFiles.Add("test.log");
        var result = await Task.FromResult(settings);

        // Assert
        result.RecentFiles.Should().Contain("test.log");
    }

    [Fact]
    public async Task ShouldAllowModifyingColumnWidths()
    {
        // Arrange
        var settings = new AppSettings();

        // Act
        settings.ColumnWidths["Message"] = 800;
        var result = await Task.FromResult(settings);

        // Assert
        result.ColumnWidths["Message"].Should().Be(800);
    }

    #endregion
}
