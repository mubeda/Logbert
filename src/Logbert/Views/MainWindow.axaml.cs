using Avalonia.Controls;
using Avalonia.Interactivity;
using Couchcoding.Logbert.ViewModels;
using Couchcoding.Logbert.Views.Dialogs;

namespace Couchcoding.Logbert.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
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

        if (result && DataContext is MainWindowViewModel viewModel)
        {
            // Apply any settings changes
            // TODO: Implement settings application
        }
    }

    public async void ShowFindDialog(object? sender, RoutedEventArgs e)
    {
        if (DataContext is MainWindowViewModel viewModel && viewModel.ActiveDocument != null)
        {
            var searchViewModel = new Couchcoding.Logbert.ViewModels.Dialogs.SearchDialogViewModel();
            searchViewModel.SetSearchTarget(viewModel.ActiveDocument);

            var dialog = new SearchDialog(searchViewModel);
            await dialog.ShowDialog(this);
        }
    }

    public async void ShowNewLogSourceDialog(object? sender, RoutedEventArgs e)
    {
        // Step 1: Let user select receiver type
        var typeDialog = new NewLogSourceDialog();
        var typeResult = await typeDialog.ShowDialog<bool>(this);

        if (!typeResult || typeDialog.SelectedReceiverType == null)
            return;

        var receiverType = typeDialog.SelectedReceiverType.Name;

        // Step 2: Configure the selected receiver
        var configDialog = new ReceiverConfigurationDialog(receiverType);
        var configResult = await configDialog.ShowDialog<bool>(this);

        if (!configResult || configDialog.ConfiguredReceiver == null)
            return;

        // Step 3: Create a new log document with the configured receiver
        if (DataContext is MainWindowViewModel viewModel)
        {
            var receiver = configDialog.ConfiguredReceiver;

            // Create a new document
            var newDocument = new LogDocumentViewModel
            {
                Title = receiver.Description,
                FilePath = receiver.Tooltip
            };

            // Initialize the receiver with a log handler
            // The document's LogViewerViewModel will act as the log handler
            receiver.Initialize(newDocument.LogViewerViewModel);

            // Store the receiver in the document
            newDocument.LogProvider = receiver;

            // Add to documents collection and docking system
            viewModel.Documents.Add(newDocument);
            viewModel.DockFactory.AddDocument(newDocument);
            viewModel.ActiveDocument = newDocument;
        }
    }

    public async void ShowScriptEditor(object? sender, RoutedEventArgs e)
    {
        var dialog = new ScriptEditorDialog();
        await dialog.ShowDialog(this);
    }

    public async void ShowStatisticsDialog(object? sender, RoutedEventArgs e)
    {
        if (DataContext is MainWindowViewModel viewModel && viewModel.ActiveDocument != null)
        {
            var dialog = new StatisticsDialog(viewModel.ActiveDocument.Messages);
            await dialog.ShowDialog(this);
        }
    }
}
