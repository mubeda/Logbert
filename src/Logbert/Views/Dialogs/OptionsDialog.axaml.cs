using Avalonia.Controls;
using Avalonia.Interactivity;
using Couchcoding.Logbert.ViewModels.Dialogs;

namespace Couchcoding.Logbert.Views.Dialogs;

public partial class OptionsDialog : Window
{
    public OptionsDialog()
    {
        InitializeComponent();
        DataContext = new OptionsDialogViewModel();
    }

    private void OnOkClick(object? sender, RoutedEventArgs e)
    {
        Close(true);
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e)
    {
        Close(false);
    }
}
