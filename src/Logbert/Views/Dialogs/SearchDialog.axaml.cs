using Avalonia.Controls;
using Avalonia.Interactivity;
using Couchcoding.Logbert.ViewModels.Dialogs;

namespace Couchcoding.Logbert.Views.Dialogs;

public partial class SearchDialog : Window
{
    public SearchDialogViewModel ViewModel { get; }

    public SearchDialog()
    {
        InitializeComponent();
        ViewModel = new SearchDialogViewModel();
        DataContext = ViewModel;
    }

    private void OnCloseClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}
