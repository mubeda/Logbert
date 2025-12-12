using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Logbert.Services;

namespace Logbert.ViewModels.Dialogs;

/// <summary>
/// ViewModel for the Welcome dialog.
/// </summary>
public partial class WelcomeDialogViewModel : ViewModelBase
{
    /// <summary>
    /// Gets the application version.
    /// </summary>
    public string Version { get; }

    /// <summary>
    /// Gets or sets whether to show this dialog on startup.
    /// </summary>
    [ObservableProperty]
    private bool _dontShowAgain;

    /// <summary>
    /// Gets the command to open documentation.
    /// </summary>
    public IRelayCommand OpenDocumentationCommand { get; }

    public WelcomeDialogViewModel()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var version = assembly.GetName().Version;
        Version = version != null ? $"Version {version.Major}.{version.Minor}.{version.Build}" : "Version 2.0.0";

        DontShowAgain = !SettingsService.Instance.Settings.ShowWelcomeScreen;
        OpenDocumentationCommand = new RelayCommand(OnOpenDocumentation);
    }

    /// <summary>
    /// Saves the user's preferences.
    /// </summary>
    public void SavePreferences()
    {
        SettingsService.Instance.UpdateSettings(settings =>
        {
            settings.ShowWelcomeScreen = !DontShowAgain;
        });
        SettingsService.Instance.Save();
    }

    private void OnOpenDocumentation()
    {
        var url = "https://github.com/logbert/logbert/wiki";

        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", url);
            }
        }
        catch
        {
            // Ignore errors opening browser
        }
    }
}
