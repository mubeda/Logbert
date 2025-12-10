using Avalonia.Controls;
using Avalonia.Interactivity;
using Logbert.ViewModels.Dialogs;

namespace Logbert.Views.Dialogs;

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
