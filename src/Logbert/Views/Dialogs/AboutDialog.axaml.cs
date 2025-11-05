using Avalonia.Controls;
using Avalonia.Interactivity;
using Couchcoding.Logbert.ViewModels.Dialogs;

namespace Couchcoding.Logbert.Views.Dialogs;

public partial class AboutDialog : Window
{
    public AboutDialog()
    {
        InitializeComponent();
        DataContext = new AboutDialogViewModel();
    }

    private void OnCloseClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}
