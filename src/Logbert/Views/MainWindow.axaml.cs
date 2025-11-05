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
        var dialog = new NewLogSourceDialog();
        var result = await dialog.ShowDialog<bool>(this);

        if (result && DataContext is MainWindowViewModel viewModel)
        {
            var selectedReceiver = dialog.SelectedReceiverType;
            if (selectedReceiver != null)
            {
                // For now, just create a sample document
                // TODO: Create actual log provider based on selected receiver
                var newDoc = new LogDocumentViewModel
                {
                    Title = $"{selectedReceiver.Name} {viewModel.Documents.Count + 1}"
                };

                // Generate sample log messages for testing
                var sampleMessages = Couchcoding.Logbert.Logging.Sample.SampleLogGenerator.GenerateMessages(100);
                foreach (var message in sampleMessages)
                {
                    newDoc.Messages.Add(message);
                }

                viewModel.Documents.Add(newDoc);
                viewModel.DockFactory.AddDocument(newDoc);
                viewModel.ActiveDocument = newDoc;
            }
        }
    }
}
