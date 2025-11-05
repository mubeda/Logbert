using Avalonia.Controls;
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
}
