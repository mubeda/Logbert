using Avalonia.Controls;
using Avalonia.Interactivity;
using Couchcoding.Logbert.Interfaces;
using Couchcoding.Logbert.ViewModels;
using Couchcoding.Logbert.ViewModels.Dialogs;
using Couchcoding.Logbert.Views.Dialogs;
using Couchcoding.Logbert.Views.Dialogs.ReceiverSettings;

namespace Couchcoding.Logbert.Views;

public partial class MainWindow : Window
{
    private MainWindowViewModel? ViewModel => DataContext as MainWindowViewModel;

    public MainWindow()
    {
        InitializeComponent();

        // Initialize ViewModel
        DataContext = new MainWindowViewModel();
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
        // TODO: SearchDialog needs to be recreated
        await System.Threading.Tasks.Task.CompletedTask;
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

            case "NLog File":
                var nlogDialog = new NLogFileReceiverSettingsView();
                receiverSettings = await nlogDialog.ShowDialog<ILogSettingsCtrl?>(this);
                break;

            // TODO: Add more receiver types here

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
                var receiver = receiverSettings.GetConfiguredInstance();

                // Create a new document for this receiver
                var newDoc = new LogDocumentViewModel
                {
                    Title = $"{selectedReceiver.Name} - {System.IO.Path.GetFileName(receiver.Settings?.ToString() ?? "New")}"
                };

                // Set up the receiver to feed messages to the document
                receiver.Initialize(newDoc.LogViewerViewModel);

                // Add document to main window
                ViewModel.Documents.Add(newDoc);
                ViewModel.ActiveDocument = newDoc;

                // Start receiving
                receiver.Start();
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

    public async void ShowScriptEditor(object? sender, RoutedEventArgs e)
    {
        var dialog = new ScriptEditorDialog();
        await dialog.ShowDialog(this);
    }

    public async void ShowStatisticsDialog(object? sender, RoutedEventArgs e)
    {
        // TODO: StatisticsDialog needs to be recreated
        await System.Threading.Tasks.Task.CompletedTask;
    }
}
