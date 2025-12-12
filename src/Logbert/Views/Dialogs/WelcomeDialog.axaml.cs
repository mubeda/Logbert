using Avalonia.Controls;
using Avalonia.Interactivity;
using Logbert.ViewModels.Dialogs;

namespace Logbert.Views.Dialogs;

/// <summary>
/// Welcome dialog shown on first run or startup.
/// </summary>
public partial class WelcomeDialog : Window
{
    public WelcomeDialog()
    {
        InitializeComponent();
        DataContext = new WelcomeDialogViewModel();
    }

    private void OnGetStartedClick(object? sender, RoutedEventArgs e)
    {
        var vm = DataContext as WelcomeDialogViewModel;
        vm?.SavePreferences();
        Close();
    }
}
