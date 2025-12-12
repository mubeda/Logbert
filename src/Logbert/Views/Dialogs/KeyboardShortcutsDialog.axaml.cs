using Avalonia.Controls;
using Avalonia.Interactivity;
using Logbert.ViewModels.Dialogs;

namespace Logbert.Views.Dialogs;

/// <summary>
/// Dialog showing all keyboard shortcuts.
/// </summary>
public partial class KeyboardShortcutsDialog : Window
{
    public KeyboardShortcutsDialog()
    {
        InitializeComponent();
        DataContext = new KeyboardShortcutsDialogViewModel();
    }

    private void OnCloseClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}
