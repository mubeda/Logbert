using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Logbert.ViewModels.Docking;

namespace Logbert.Views.Docking;

/// <summary>
/// Dockable search panel for searching log messages.
/// </summary>
public partial class SearchPanelView : UserControl
{
    public SearchPanelView()
    {
        InitializeComponent();
    }

    private void OnSearchTextKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && DataContext is SearchPanelViewModel vm)
        {
            if (vm.FindNextCommand.CanExecute(null))
            {
                vm.FindNextCommand.Execute(null);
            }
        }
    }

    private void OnResultDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (DataContext is SearchPanelViewModel vm)
        {
            vm.NavigateToSelectedResult();
        }
    }
}
