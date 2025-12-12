using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Logbert.ViewModels.Dialogs;

/// <summary>
/// ViewModel for the About dialog.
/// </summary>
public partial class AboutDialogViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _applicationName = "Logbert";

    [ObservableProperty]
    private string _version = string.Empty;

    [ObservableProperty]
    private string _copyright = "Copyright © 2024 Logbert Contributors";

    [ObservableProperty]
    private string _description = "Cross-platform log file viewer for various log formats";

    [ObservableProperty]
    private string _licenseText = string.Empty;

    #region System Information

    [ObservableProperty]
    private string _operatingSystem = string.Empty;

    [ObservableProperty]
    private string _dotNetVersion = string.Empty;

    [ObservableProperty]
    private string _architecture = string.Empty;

    [ObservableProperty]
    private string _avaloniaVersion = string.Empty;

    #endregion

    #region Commands

    public IRelayCommand OpenGitHubCommand { get; }
    public IRelayCommand OpenIssuesCommand { get; }
    public IRelayCommand OpenDocumentationCommand { get; }
    public IRelayCommand CopySystemInfoCommand { get; }

    #endregion

    public AboutDialogViewModel()
    {
        // Get version from assembly
        var assembly = Assembly.GetExecutingAssembly();
        var versionInfo = assembly.GetName().Version;
        Version = versionInfo != null
            ? $"Version {versionInfo.Major}.{versionInfo.Minor}.{versionInfo.Build}"
            : "Version 2.0.0";

        // Get system information
        OperatingSystem = $"{RuntimeInformation.OSDescription}";
        DotNetVersion = $".NET {Environment.Version}";
        Architecture = RuntimeInformation.ProcessArchitecture.ToString();

        // Get Avalonia version
        var avaloniaAssembly = typeof(Avalonia.Application).Assembly;
        var avaloniaVersionInfo = avaloniaAssembly.GetName().Version;
        AvaloniaVersion = avaloniaVersionInfo != null
            ? $"Avalonia {avaloniaVersionInfo.Major}.{avaloniaVersionInfo.Minor}.{avaloniaVersionInfo.Build}"
            : "Avalonia 11.x";

        // Initialize commands
        OpenGitHubCommand = new RelayCommand(() => OpenUrl("https://github.com/logbert/logbert"));
        OpenIssuesCommand = new RelayCommand(() => OpenUrl("https://github.com/logbert/logbert/issues"));
        OpenDocumentationCommand = new RelayCommand(() => OpenUrl("https://github.com/logbert/logbert/wiki"));
        CopySystemInfoCommand = new RelayCommand(OnCopySystemInfo);

        // Load MIT license text
        LicenseText = @"The MIT License (MIT)

Copyright (c) 2024 Logbert Contributors

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the ""Software""), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.";
    }

    private void OpenUrl(string url)
    {
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

    private async void OnCopySystemInfo()
    {
        var info = $"""
            Logbert {Version}
            OS: {OperatingSystem}
            Runtime: {DotNetVersion}
            Architecture: {Architecture}
            UI Framework: {AvaloniaVersion}
            """;

        try
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var mainWindow = desktop.MainWindow;
                if (mainWindow != null)
                {
                    var clipboard = mainWindow.Clipboard;
                    if (clipboard != null)
                    {
                        await clipboard.SetTextAsync(info);
                    }
                }
            }
        }
        catch
        {
            // Ignore clipboard errors
        }
    }
}
