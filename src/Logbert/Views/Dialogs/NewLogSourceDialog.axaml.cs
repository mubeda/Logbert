using Avalonia.Controls;
using Avalonia.Interactivity;
using Couchcoding.Logbert.ViewModels.Dialogs;
using System.Linq;

namespace Couchcoding.Logbert.Views.Dialogs;

public partial class NewLogSourceDialog : Window
{
    public NewLogSourceDialog()
    {
        InitializeComponent();
        DataContext = new NewLogSourceDialogViewModel();
    }

    public LogReceiverType? SelectedReceiverType
    {
        get
        {
            if (DataContext is NewLogSourceDialogViewModel vm)
            {
                return vm.SelectedReceiver;
            }
            return null;
        }
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
