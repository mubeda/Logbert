using Avalonia.Controls;
using Avalonia.Interactivity;
using Couchcoding.Logbert.ViewModels.Dialogs;

namespace Couchcoding.Logbert.Views.Dialogs;

public partial class NewLogSourceDialog : Window
{
    public NewLogSourceDialogViewModel ViewModel { get; }

    public NewLogSourceDialog()
    {
        InitializeComponent();
        ViewModel = new NewLogSourceDialogViewModel();
        DataContext = ViewModel;
    }

    private void OnOkClick(object? sender, RoutedEventArgs e)
    {
        if (ViewModel.SelectedReceiver != null)
        {
            Close(ViewModel.SelectedReceiver);
        }
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e)
    {
        Close(null);
    }
}
