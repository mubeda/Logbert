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
        // TODO: NewLogSourceDialog needs to be recreated
        // For now, the ViewModel will create a sample document
        if (DataContext is MainWindowViewModel viewModel)
        {
            viewModel.NewDocumentCommand.Execute(null);
        }
        await System.Threading.Tasks.Task.CompletedTask;
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
