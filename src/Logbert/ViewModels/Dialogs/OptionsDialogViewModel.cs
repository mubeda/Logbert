using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Logbert.Services;

namespace Logbert.ViewModels.Dialogs;

/// <summary>
/// ViewModel for the Options dialog.
/// </summary>
public partial class OptionsDialogViewModel : ViewModelBase
{
    #region General Settings

    [ObservableProperty]
    private bool _alwaysOnTop = false;

    [ObservableProperty]
    private bool _minimizeToTray = false;

    [ObservableProperty]
    private bool _showWelcomeScreen = true;

    [ObservableProperty]
    private int _maxRecentFiles = 10;

    #endregion

    #region Appearance Settings

    [ObservableProperty]
    private string _selectedTheme = "Light";

    [ObservableProperty]
    private string _defaultFontFamily = "Consolas";

    [ObservableProperty]
    private int _defaultFontSize = 11;

    #endregion

    #region Behavior Settings

    [ObservableProperty]
    private bool _autoScroll = true;

    [ObservableProperty]
    private bool _showTimestamps = true;

    [ObservableProperty]
    private string _timestampFormat = "yyyy-MM-dd HH:mm:ss.fff";

    [ObservableProperty]
    private bool _highlightSearchMatches = true;

    [ObservableProperty]
    private bool _enableWordWrap = false;

    #endregion

    #region Script Settings

    [ObservableProperty]
    private string _defaultScriptPath = string.Empty;

    [ObservableProperty]
    private bool _autoSaveScripts = false;

    [ObservableProperty]
    private bool _runScriptOnLoad = false;

    [ObservableProperty]
    private int _scriptFontSize = 12;

    [ObservableProperty]
    private bool _scriptShowLineNumbers = true;

    [ObservableProperty]
    private bool _scriptSyntaxHighlighting = true;

    #endregion

    #region Advanced Settings

    [ObservableProperty]
    private int _maxLogMessages = 100000;

    [ObservableProperty]
    private bool _enableVirtualization = true;

    [ObservableProperty]
    private int _networkBufferSize = 65536;

    [ObservableProperty]
    private int _fileWatcherInterval = 500;

    #endregion

    #region Logging Settings

    [ObservableProperty]
    private bool _enableLogging = false;

    [ObservableProperty]
    private int _logRetentionDays = 7;

    #endregion

    public ObservableCollection<string> AvailableThemes { get; } = new()
    {
        "Light",
        "Dark",
        "System"
    };

    public ObservableCollection<string> AvailableFonts { get; } = new()
    {
        "Consolas",
        "Courier New",
        "Monospace",
        "Source Code Pro",
        "Fira Code"
    };

    public ObservableCollection<string> TimestampFormats { get; } = new()
    {
        "yyyy-MM-dd HH:mm:ss.fff",
        "yyyy-MM-ddTHH:mm:ss.fffZ",
        "MM/dd/yyyy HH:mm:ss",
        "dd/MM/yyyy HH:mm:ss",
        "HH:mm:ss.fff",
        "HH:mm:ss"
    };

    public IRelayCommand OkCommand { get; } = null!;
    public IRelayCommand CancelCommand { get; } = null!;
    public IRelayCommand ResetCommand { get; } = null!;

    public bool DialogResult { get; private set; }

    public OptionsDialogViewModel()
    {
        OkCommand = new RelayCommand(OnOk);
        CancelCommand = new RelayCommand(OnCancel);
        ResetCommand = new RelayCommand(OnReset);

        LoadSettings();
    }

    private void LoadSettings()
    {
        var settings = SettingsService.Instance.Settings;

        // General
        AlwaysOnTop = settings.AlwaysOnTop;
        MinimizeToTray = settings.MinimizeToTray;
        ShowWelcomeScreen = settings.ShowWelcomeScreen;
        MaxRecentFiles = settings.MaxRecentFiles;

        // Appearance
        SelectedTheme = settings.SelectedTheme;
        DefaultFontFamily = settings.DefaultFontFamily;
        DefaultFontSize = settings.DefaultFontSize;

        // Behavior
        AutoScroll = settings.AutoScroll;
        ShowTimestamps = settings.ShowTimestamps;
        TimestampFormat = settings.TimestampFormat;
        HighlightSearchMatches = settings.HighlightSearchMatches;
        EnableWordWrap = settings.EnableWordWrap;

        // Script
        DefaultScriptPath = settings.DefaultScriptPath;
        AutoSaveScripts = settings.AutoSaveScripts;
        RunScriptOnLoad = settings.RunScriptOnLoad;
        ScriptFontSize = settings.ScriptFontSize;
        ScriptShowLineNumbers = settings.ScriptShowLineNumbers;
        ScriptSyntaxHighlighting = settings.ScriptSyntaxHighlighting;

        // Advanced
        MaxLogMessages = settings.MaxLogMessages;
        EnableVirtualization = settings.EnableVirtualization;
        NetworkBufferSize = settings.NetworkBufferSize;
        FileWatcherInterval = settings.FileWatcherInterval;

        // Logging
        EnableLogging = settings.EnableLogging;
        LogRetentionDays = settings.LogRetentionDays;
    }

    private void SaveSettings()
    {
        SettingsService.Instance.UpdateSettings(settings =>
        {
            // General
            settings.AlwaysOnTop = AlwaysOnTop;
            settings.MinimizeToTray = MinimizeToTray;
            settings.ShowWelcomeScreen = ShowWelcomeScreen;
            settings.MaxRecentFiles = MaxRecentFiles;

            // Appearance
            settings.SelectedTheme = SelectedTheme;
            settings.DefaultFontFamily = DefaultFontFamily;
            settings.DefaultFontSize = DefaultFontSize;

            // Behavior
            settings.AutoScroll = AutoScroll;
            settings.ShowTimestamps = ShowTimestamps;
            settings.TimestampFormat = TimestampFormat;
            settings.HighlightSearchMatches = HighlightSearchMatches;
            settings.EnableWordWrap = EnableWordWrap;

            // Script
            settings.DefaultScriptPath = DefaultScriptPath;
            settings.AutoSaveScripts = AutoSaveScripts;
            settings.RunScriptOnLoad = RunScriptOnLoad;
            settings.ScriptFontSize = ScriptFontSize;
            settings.ScriptShowLineNumbers = ScriptShowLineNumbers;
            settings.ScriptSyntaxHighlighting = ScriptSyntaxHighlighting;

            // Advanced
            settings.MaxLogMessages = MaxLogMessages;
            settings.EnableVirtualization = EnableVirtualization;
            settings.NetworkBufferSize = NetworkBufferSize;
            settings.FileWatcherInterval = FileWatcherInterval;

            // Logging
            settings.EnableLogging = EnableLogging;
            settings.LogRetentionDays = LogRetentionDays;
        });

        SettingsService.Instance.Save();
    }

    private void OnOk()
    {
        SaveSettings();
        DialogResult = true;
    }

    private void OnCancel()
    {
        DialogResult = false;
    }

    private void OnReset()
    {
        // General
        AlwaysOnTop = false;
        MinimizeToTray = false;
        ShowWelcomeScreen = true;
        MaxRecentFiles = 10;

        // Appearance
        SelectedTheme = "System";
        DefaultFontFamily = "Consolas";
        DefaultFontSize = 11;

        // Behavior
        AutoScroll = true;
        ShowTimestamps = true;
        TimestampFormat = "yyyy-MM-dd HH:mm:ss.fff";
        HighlightSearchMatches = true;
        EnableWordWrap = false;

        // Script
        DefaultScriptPath = string.Empty;
        AutoSaveScripts = false;
        RunScriptOnLoad = false;
        ScriptFontSize = 12;
        ScriptShowLineNumbers = true;
        ScriptSyntaxHighlighting = true;

        // Advanced
        MaxLogMessages = 100000;
        EnableVirtualization = true;
        NetworkBufferSize = 65536;
        FileWatcherInterval = 500;

        // Logging
        EnableLogging = false;
        LogRetentionDays = 7;
    }
}
