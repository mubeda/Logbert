using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Couchcoding.Logbert.ViewModels.Dialogs;

/// <summary>
/// ViewModel for the Options dialog.
/// </summary>
public partial class OptionsDialogViewModel : ViewModelBase
{
    [ObservableProperty]
    private bool _alwaysOnTop = false;

    [ObservableProperty]
    private bool _minimizeToTray = false;

    [ObservableProperty]
    private bool _showWelcomeScreen = true;

    [ObservableProperty]
    private int _maxRecentFiles = 10;

    [ObservableProperty]
    private string _selectedTheme = "Light";

    [ObservableProperty]
    private bool _enableLogging = false;

    [ObservableProperty]
    private int _logRetentionDays = 7;

    [ObservableProperty]
    private string _defaultFontFamily = "Consolas";

    [ObservableProperty]
    private int _defaultFontSize = 11;

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
        // TODO: Load from application settings
        // For now, using defaults
    }

    private void SaveSettings()
    {
        // TODO: Save to application settings
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
        AlwaysOnTop = false;
        MinimizeToTray = false;
        ShowWelcomeScreen = true;
        MaxRecentFiles = 10;
        SelectedTheme = "Light";
        EnableLogging = false;
        LogRetentionDays = 7;
        DefaultFontFamily = "Consolas";
        DefaultFontSize = 11;
    }
}
