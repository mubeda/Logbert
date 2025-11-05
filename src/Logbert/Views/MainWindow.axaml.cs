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

        // TODO: MainWindowViewModel excluded from compilation, settings application disabled
    }

    public async void ShowFindDialog(object? sender, RoutedEventArgs e)
    {
        // TODO: MainWindowViewModel excluded from compilation, search functionality disabled for now
        await System.Threading.Tasks.Task.CompletedTask;
    }

    public async void ShowNewLogSourceDialog(object? sender, RoutedEventArgs e)
    {
        // TODO: This functionality is currently disabled as ReceiverConfigurationDialog
        // depends on receiver settings ViewModels that are excluded from compilation.
        // Will be re-implemented after completing Avalonia receiver migration.
        await System.Threading.Tasks.Task.CompletedTask;
    }

    public async void ShowScriptEditor(object? sender, RoutedEventArgs e)
    {
        var dialog = new ScriptEditorDialog();
        await dialog.ShowDialog(this);
    }

    public async void ShowStatisticsDialog(object? sender, RoutedEventArgs e)
    {
        // TODO: StatisticsDialog is currently excluded from compilation.
        // Will be re-implemented as an Avalonia dialog after migrations are complete.
        await System.Threading.Tasks.Task.CompletedTask;
    }
}
