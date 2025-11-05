using Avalonia.Controls;
using Avalonia.Interactivity;
using Couchcoding.Logbert.ViewModels.Dialogs;

namespace Couchcoding.Logbert.Views.Dialogs;

public partial class SearchDialog : Window
{
    public SearchDialog()
    {
        InitializeComponent();
        DataContext = new SearchDialogViewModel();
    }

    public SearchDialog(SearchDialogViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }

    private void OnCloseClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}
