using Avalonia.Controls;
using Avalonia.Interactivity;
using Logbert.ViewModels.Dialogs;

namespace Logbert.Views.Dialogs;

/// <summary>
/// Dialog for configuring DataGrid column order and visibility.
/// </summary>
public partial class ColumnReorderDialog : Window
{
    public ColumnReorderDialog()
    {
        InitializeComponent();
        DataContext = new ColumnReorderDialogViewModel();
    }

    /// <summary>
    /// Gets the dialog result.
    /// </summary>
    public bool DialogResult => (DataContext as ColumnReorderDialogViewModel)?.DialogResult ?? false;

    /// <summary>
    /// Gets the view model.
    /// </summary>
    public ColumnReorderDialogViewModel? ViewModel => DataContext as ColumnReorderDialogViewModel;

    private void OnOkClick(object? sender, RoutedEventArgs e)
    {
        var vm = DataContext as ColumnReorderDialogViewModel;
        vm?.OkCommand.Execute(null);
        Close();
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e)
    {
        var vm = DataContext as ColumnReorderDialogViewModel;
        vm?.CancelCommand.Execute(null);
        Close();
    }
}
