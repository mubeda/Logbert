using Avalonia.Controls;
using Avalonia.Interactivity;
using Logbert.ViewModels.Dialogs;

namespace Logbert.Views.Dialogs;

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
