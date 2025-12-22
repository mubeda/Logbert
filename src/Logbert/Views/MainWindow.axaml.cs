using System;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Logbert.Helper;
using Logbert.Interfaces;
using Logbert.Receiver.Log4NetFileReceiver;
using Logbert.Services;
using Logbert.ViewModels;
using Logbert.ViewModels.Dialogs;
using Logbert.ViewModels.Docking;
using Logbert.Views.Dialogs;
using Logbert.Views.Dialogs.ReceiverSettings;

namespace Logbert.Views;

public partial class MainWindow : Window
{
    private MainWindowViewModel? ViewModel => DataContext as MainWindowViewModel;

    public MainWindow()
    {
        InitializeComponent();

        // Initialize ViewModel
        DataContext = new MainWindowViewModel();

        // Wire up window events for settings persistence
        Loaded += OnWindowLoaded;
        Closing += OnWindowClosing;
    }

    private async void OnWindowLoaded(object? sender, RoutedEventArgs e)
    {
        // Load window state from settings
        var settings = SettingsService.Instance.Settings;

        // Restore window position and size
        if (settings.WindowX >= 0 && settings.WindowY >= 0)
        {
            Position = new Avalonia.PixelPoint(settings.WindowX, settings.WindowY);
        }

        Width = settings.WindowWidth;
        Height = settings.WindowHeight;

        // Restore window state
        WindowState = settings.WindowState switch
        {
            "Maximized" => WindowState.Maximized,
            "Minimized" => WindowState.Minimized,
            _ => WindowState.Normal
        };

        // Show Welcome dialog on first run or if enabled in settings
        if (settings.ShowWelcomeScreen)
        {
            var welcomeDialog = new WelcomeDialog();
            await welcomeDialog.ShowDialog(this);
        }
    }

    private void OnWindowClosing(object? sender, WindowClosingEventArgs e)
    {
        // Save window state to settings
        SettingsService.Instance.UpdateSettings(settings =>
        {
            settings.WindowX = Position.X;
            settings.WindowY = Position.Y;
            settings.WindowWidth = (int)Width;
            settings.WindowHeight = (int)Height;
            settings.WindowState = WindowState switch
            {
                WindowState.Maximized => "Maximized",
                WindowState.Minimized => "Minimized",
                _ => "Normal"
            };
        });

        // Save settings to disk
        SettingsService.Instance.Save();
    }

    public async void ShowAboutDialog(object? sender, RoutedEventArgs e)
    {
        var dialog = new AboutDialog();
        await dialog.ShowDialog(this);
    }

    public async void ShowOptionsDialog(object? sender, RoutedEventArgs e)
    {
        var dialog = new OptionsDialog();
        var result = await dialog.ShowDialog<bool>(this);

        // TODO: Apply settings if result is true
    }

    public async void ShowFindDialog(object? sender, RoutedEventArgs e)
    {
        var dialog = new SearchDialog();

        // Pass the active document to the search dialog
        if (ViewModel?.ActiveDocument != null)
        {
            dialog.ViewModel.SetSearchTarget(ViewModel.ActiveDocument);
        }

        await dialog.ShowDialog(this);
    }

    public async void ShowNewLogSourceDialog(object? sender, RoutedEventArgs e)
    {
        // Step 1: Show receiver type selection dialog
        var newLogSourceDialog = new NewLogSourceDialog();
        var selectedReceiver = await newLogSourceDialog.ShowDialog<LogReceiverType?>(this);

        if (selectedReceiver == null)
        {
            return; // User cancelled
        }

        // Step 2: Show receiver-specific configuration dialog
        ILogSettingsCtrl? receiverSettings = null;

        switch (selectedReceiver.Name)
        {
            case "Log4Net File":
                var log4NetDialog = new Log4NetFileReceiverSettingsView();
                receiverSettings = await log4NetDialog.ShowDialog<ILogSettingsCtrl?>(this);
                break;

            case "Log4Net Dir":
                var log4NetDirDialog = new Log4NetDirReceiverSettingsView();
                receiverSettings = await log4NetDirDialog.ShowDialog<ILogSettingsCtrl?>(this);
                break;

            case "NLog File":
                var nlogDialog = new NLogFileReceiverSettingsView();
                receiverSettings = await nlogDialog.ShowDialog<ILogSettingsCtrl?>(this);
                break;

            case "NLog Dir":
                var nlogDirDialog = new NLogDirReceiverSettingsView();
                receiverSettings = await nlogDirDialog.ShowDialog<ILogSettingsCtrl?>(this);
                break;

            case "Syslog File":
                var syslogDialog = new SyslogFileReceiverSettingsView();
                receiverSettings = await syslogDialog.ShowDialog<ILogSettingsCtrl?>(this);
                break;

            // Network receivers
            case "Log4Net UDP":
                var log4NetUdpDialog = new Log4NetUdpReceiverSettingsView();
                receiverSettings = await log4NetUdpDialog.ShowDialog<ILogSettingsCtrl?>(this);
                break;

            case "NLog UDP":
                var nlogUdpDialog = new NLogUdpReceiverSettingsView();
                receiverSettings = await nlogUdpDialog.ShowDialog<ILogSettingsCtrl?>(this);
                break;

            case "NLog TCP":
                var nlogTcpDialog = new NLogTcpReceiverSettingsView();
                receiverSettings = await nlogTcpDialog.ShowDialog<ILogSettingsCtrl?>(this);
                break;

            case "Syslog UDP":
                var syslogUdpDialog = new SyslogUdpReceiverSettingsView();
                receiverSettings = await syslogUdpDialog.ShowDialog<ILogSettingsCtrl?>(this);
                break;

            // System receivers
            case "Windows Event Log":
                var eventlogDialog = new EventlogReceiverSettingsView();
                receiverSettings = await eventlogDialog.ShowDialog<ILogSettingsCtrl?>(this);
                break;

            case "Windows Debug Output":
                var winDebugDialog = new WinDebugReceiverSettingsView();
                receiverSettings = await winDebugDialog.ShowDialog<ILogSettingsCtrl?>(this);
                break;

            // Custom receivers
            case "Custom File":
                var customFileDialog = new CustomFileReceiverSettingsView();
                receiverSettings = await customFileDialog.ShowDialog<ILogSettingsCtrl?>(this);
                break;

            case "Custom Dir":
                var customDirDialog = new CustomDirReceiverSettingsView();
                receiverSettings = await customDirDialog.ShowDialog<ILogSettingsCtrl?>(this);
                break;

            case "Custom UDP":
                var customUdpDialog = new CustomUdpReceiverSettingsView();
                receiverSettings = await customUdpDialog.ShowDialog<ILogSettingsCtrl?>(this);
                break;

            case "Custom TCP":
                var customTcpDialog = new CustomTcpReceiverSettingsView();
                receiverSettings = await customTcpDialog.ShowDialog<ILogSettingsCtrl?>(this);
                break;

            case "Custom HTTP":
                var customHttpDialog = new CustomHttpReceiverSettingsView();
                receiverSettings = await customHttpDialog.ShowDialog<ILogSettingsCtrl?>(this);
                break;

            default:
                // For now, fall back to creating a sample document
                if (ViewModel != null)
                {
                    ViewModel.NewDocumentCommand.Execute(null);
                }
                return;
        }

        // Step 3: Create receiver and add document if settings were confirmed
        if (receiverSettings != null && ViewModel != null)
        {
            try
            {
                // Get display info before creating the receiver instance
                string displayInfo = receiverSettings.GetDisplayInfo();
                var receiver = receiverSettings.GetConfiguredInstance();

                // Create a new document for this receiver
                var newDoc = new LogDocumentViewModel
                {
                    Title = $"{selectedReceiver.Name} - {displayInfo}"
                };

                // Set up the receiver to feed messages to the document
                // Initialize automatically starts the receiver
                receiver.Initialize(newDoc.LogViewerViewModel);

                // Register the provider for pause control
                newDoc.LogViewerViewModel.SetLogProvider(receiver);

                // Add document to main window
                ViewModel.Documents.Add(newDoc);
                ViewModel.ActiveDocument = newDoc;
            }
            catch (System.Exception ex)
            {
                // Show error dialog
                var errorDialog = new Window
                {
                    Title = "Error",
                    Width = 400,
                    Height = 150,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    Content = new StackPanel
                    {
                        Margin = new Avalonia.Thickness(20),
                        Spacing = 15,
                        Children =
                        {
                            new TextBlock
                            {
                                Text = $"Failed to create receiver: {ex.Message}",
                                TextWrapping = Avalonia.Media.TextWrapping.Wrap
                            }
                        }
                    }
                };
                await errorDialog.ShowDialog(this);
            }
        }
    }

    public async void ShowOpenFileDialog(object? sender, RoutedEventArgs e)
    {
        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open Log File",
            AllowMultiple = false,
            FileTypeFilter = new[]
            {
                new FilePickerFileType("Log Files")
                {
                    Patterns = new[] { "*.log", "*.txt", "*.xml" }
                },
                new FilePickerFileType("All Files")
                {
                    Patterns = new[] { "*.*" }
                }
            }
        });

        if (files.Count == 0 || ViewModel == null)
        {
            return;
        }

        var filePath = files[0].Path.LocalPath;
        var fileName = Path.GetFileName(filePath);

        try
        {
            // Create a Log4Net file receiver for the selected file
            // Use UTF-8 encoding (65001) and start from beginning
            var receiver = new Log4NetFileReceiver(filePath, true, 65001);

            // Create a new document for this receiver
            var newDoc = new LogDocumentViewModel
            {
                Title = fileName
            };

            // Initialize the receiver with the document's log handler
            receiver.Initialize(newDoc.LogViewerViewModel);

            // Register the provider for pause control
            newDoc.LogViewerViewModel.SetLogProvider(receiver);

            // Add document to main window
            ViewModel.Documents.Add(newDoc);
            ViewModel.ActiveDocument = newDoc;

            // Add to recent files
            MruManager.AddFile(filePath);
        }
        catch (Exception ex)
        {
            // Show error dialog
            var errorDialog = new Window
            {
                Title = "Error Opening File",
                Width = 400,
                Height = 150,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Content = new StackPanel
                {
                    Margin = new Avalonia.Thickness(20),
                    Spacing = 15,
                    Children =
                    {
                        new TextBlock
                        {
                            Text = $"Failed to open file: {ex.Message}",
                            TextWrapping = Avalonia.Media.TextWrapping.Wrap
                        }
                    }
                }
            };
            await errorDialog.ShowDialog(this);
        }
    }

    public async void ShowScriptEditor(object? sender, RoutedEventArgs e)
    {
        var dialog = new ScriptEditorDialog();
        await dialog.ShowDialog(this);
    }

    public async void ShowStatisticsDialog(object? sender, RoutedEventArgs e)
    {
        if (ViewModel?.ActiveDocument == null)
        {
            return; // No active document with messages
        }

        var dialog = new StatisticsDialog(ViewModel.ActiveDocument.Messages);
        await dialog.ShowDialog(this);
    }

    public async void ShowExportDialog(object? sender, RoutedEventArgs e)
    {
        if (ViewModel?.ActiveDocument == null || ViewModel.ActiveDocument.Messages.Count == 0)
        {
            return; // No active document with messages
        }

        var dialog = new ExportDialog();

        // Pass all messages and filtered messages to the dialog
        var allMessages = ViewModel.ActiveDocument.Messages.ToList();
        var filteredMessages = ViewModel.ActiveDocument.LogViewerViewModel?.FilteredMessages?.ToList();

        dialog.SetMessages(allMessages, filteredMessages);

        await dialog.ShowDialog(this);
    }

    public async void ShowWelcomeDialog(object? sender, RoutedEventArgs e)
    {
        var dialog = new WelcomeDialog();
        await dialog.ShowDialog(this);
    }

    public async void ShowKeyboardShortcutsDialog(object? sender, RoutedEventArgs e)
    {
        var dialog = new KeyboardShortcutsDialog();
        await dialog.ShowDialog(this);
    }

    public async void ShowColumnConfigDialog(object? sender, RoutedEventArgs e)
    {
        var dialog = new ColumnReorderDialog();
        await dialog.ShowDialog(this);

        if (dialog.DialogResult)
        {
            // Save column configuration to settings
            dialog.ViewModel?.SaveToSettings();

            // Notify active document to refresh column configuration
            if (ViewModel?.ActiveDocument?.LogViewerViewModel != null)
            {
                ViewModel.ActiveDocument.LogViewerViewModel.RefreshColumnConfiguration();
            }
        }
    }
}
