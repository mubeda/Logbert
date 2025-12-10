using System.Collections.Generic;

namespace Couchcoding.Logbert.Models;

/// <summary>
/// Application settings POCO for JSON serialization.
/// </summary>
public class AppSettings
{
    #region Window State

    /// <summary>
    /// Gets or sets the main window X position.
    /// </summary>
    public int WindowX { get; set; } = 100;

    /// <summary>
    /// Gets or sets the main window Y position.
    /// </summary>
    public int WindowY { get; set; } = 100;

    /// <summary>
    /// Gets or sets the main window width.
    /// </summary>
    public int WindowWidth { get; set; } = 1200;

    /// <summary>
    /// Gets or sets the main window height.
    /// </summary>
    public int WindowHeight { get; set; } = 800;

    /// <summary>
    /// Gets or sets the window state (Normal, Maximized, Minimized).
    /// </summary>
    public string WindowState { get; set; } = "Normal";

    #endregion

    #region Panel Layout

    /// <summary>
    /// Gets or sets the left panel width (logger tree).
    /// </summary>
    public double LeftPanelWidth { get; set; } = 200;

    /// <summary>
    /// Gets or sets the right panel width (bookmarks).
    /// </summary>
    public double RightPanelWidth { get; set; } = 200;

    /// <summary>
    /// Gets or sets the bottom panel height (details).
    /// </summary>
    public double BottomPanelHeight { get; set; } = 150;

    /// <summary>
    /// Gets or sets whether the logger tree panel is visible.
    /// </summary>
    public bool LoggerTreeVisible { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the bookmarks panel is visible.
    /// </summary>
    public bool BookmarksVisible { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the details panel is visible.
    /// </summary>
    public bool DetailsVisible { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the filter panel is visible.
    /// </summary>
    public bool FilterVisible { get; set; } = true;

    #endregion

    #region Column Widths

    /// <summary>
    /// Gets or sets the DataGrid column widths (column name -> width).
    /// </summary>
    public Dictionary<string, double> ColumnWidths { get; set; } = new()
    {
        { "Number", 80 },
        { "Level", 80 },
        { "Timestamp", 150 },
        { "Logger", 200 },
        { "Thread", 80 },
        { "Message", 500 }
    };

    #endregion

    #region User Preferences

    /// <summary>
    /// Gets or sets whether the window should always be on top.
    /// </summary>
    public bool AlwaysOnTop { get; set; } = false;

    /// <summary>
    /// Gets or sets whether to minimize to system tray.
    /// </summary>
    public bool MinimizeToTray { get; set; } = false;

    /// <summary>
    /// Gets or sets whether to show the welcome screen on startup.
    /// </summary>
    public bool ShowWelcomeScreen { get; set; } = true;

    /// <summary>
    /// Gets or sets the maximum number of recent files to track.
    /// </summary>
    public int MaxRecentFiles { get; set; } = 10;

    /// <summary>
    /// Gets or sets the selected theme (Light, Dark, System).
    /// </summary>
    public string SelectedTheme { get; set; } = "System";

    /// <summary>
    /// Gets or sets whether logging is enabled.
    /// </summary>
    public bool EnableLogging { get; set; } = false;

    /// <summary>
    /// Gets or sets the number of days to retain logs.
    /// </summary>
    public int LogRetentionDays { get; set; } = 7;

    /// <summary>
    /// Gets or sets the default font family for log display.
    /// </summary>
    public string DefaultFontFamily { get; set; } = "Consolas";

    /// <summary>
    /// Gets or sets the default font size for log display.
    /// </summary>
    public int DefaultFontSize { get; set; } = 11;

    #endregion

    #region Behavior

    /// <summary>
    /// Gets or sets whether to auto-scroll to the latest log message.
    /// </summary>
    public bool AutoScroll { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to show timestamps in the log view.
    /// </summary>
    public bool ShowTimestamps { get; set; } = true;

    /// <summary>
    /// Gets or sets the timestamp format string.
    /// </summary>
    public string TimestampFormat { get; set; } = "yyyy-MM-dd HH:mm:ss.fff";

    #endregion

    #region Recent Files

    /// <summary>
    /// Gets or sets the list of recently opened files.
    /// </summary>
    public List<string> RecentFiles { get; set; } = new();

    #endregion
}
