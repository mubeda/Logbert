using System.Collections.Generic;

namespace Logbert.Models;

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

    /// <summary>
    /// Gets or sets the DataGrid column visibility (column name -> visible).
    /// </summary>
    public Dictionary<string, bool> ColumnVisibility { get; set; } = new()
    {
        { "Number", true },
        { "Level", true },
        { "Timestamp", true },
        { "Logger", true },
        { "Thread", true },
        { "Message", true },
        { "Exception", false },
        { "Class", false },
        { "Method", false },
        { "File", false },
        { "Line", false }
    };

    /// <summary>
    /// Gets or sets the DataGrid column order (column name -> display index).
    /// </summary>
    public Dictionary<string, int> ColumnOrder { get; set; } = new()
    {
        { "Number", 0 },
        { "Level", 1 },
        { "Timestamp", 2 },
        { "Logger", 3 },
        { "Thread", 4 },
        { "Message", 5 },
        { "Exception", 6 },
        { "Class", 7 },
        { "Method", 8 },
        { "File", 9 },
        { "Line", 10 }
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
    /// Cross-platform monospace font stack: Menlo (macOS), Consolas (Windows), DejaVu Sans Mono (Linux).
    /// </summary>
    public string DefaultFontFamily { get; set; } = "Menlo, Consolas, DejaVu Sans Mono, Liberation Mono, monospace";

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

    #region Panel Visibility (Extended)

    /// <summary>
    /// Gets or sets whether the search panel is visible.
    /// </summary>
    public bool SearchPanelVisible { get; set; } = false;

    /// <summary>
    /// Gets or sets whether the statistics panel is visible.
    /// </summary>
    public bool StatisticsPanelVisible { get; set; } = false;

    /// <summary>
    /// Gets or sets whether the color map panel is visible.
    /// </summary>
    public bool ColorMapPanelVisible { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the script panel is visible.
    /// </summary>
    public bool ScriptPanelVisible { get; set; } = false;

    #endregion

    #region Script Settings

    /// <summary>
    /// Gets or sets the default Lua script path.
    /// </summary>
    public string DefaultScriptPath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether to auto-save scripts.
    /// </summary>
    public bool AutoSaveScripts { get; set; } = false;

    /// <summary>
    /// Gets or sets whether to run scripts on load.
    /// </summary>
    public bool RunScriptOnLoad { get; set; } = false;

    /// <summary>
    /// Gets or sets the script editor font size.
    /// </summary>
    public int ScriptFontSize { get; set; } = 12;

    /// <summary>
    /// Gets or sets whether to show line numbers in script editor.
    /// </summary>
    public bool ScriptShowLineNumbers { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to enable syntax highlighting.
    /// </summary>
    public bool ScriptSyntaxHighlighting { get; set; } = true;

    #endregion

    #region Advanced Settings

    /// <summary>
    /// Gets or sets the maximum number of log messages to display.
    /// </summary>
    public int MaxLogMessages { get; set; } = 100000;

    /// <summary>
    /// Gets or sets whether to enable virtualization for large logs.
    /// </summary>
    public bool EnableVirtualization { get; set; } = true;

    /// <summary>
    /// Gets or sets the buffer size for network receivers.
    /// </summary>
    public int NetworkBufferSize { get; set; } = 65536;

    /// <summary>
    /// Gets or sets the file watcher polling interval in milliseconds.
    /// </summary>
    public int FileWatcherInterval { get; set; } = 500;

    /// <summary>
    /// Gets or sets whether to highlight search matches.
    /// </summary>
    public bool HighlightSearchMatches { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to enable word wrap in message view.
    /// </summary>
    public bool EnableWordWrap { get; set; } = false;

    #endregion
}
