using Avalonia.Controls;
using Avalonia.Interactivity;
using Logbert.Interfaces;
using Logbert.Services;
using Logbert.ViewModels.Dialogs.ReceiverSettings;

namespace Logbert.Views.Dialogs.ReceiverSettings;

public partial class WinDebugReceiverSettingsView : Window
{
    private WinDebugReceiverSettingsViewModel? ViewModel => DataContext as WinDebugReceiverSettingsViewModel;

    public WinDebugReceiverSettingsView()
    {
        InitializeComponent();
        DataContext = new WinDebugReceiverSettingsViewModel();
    }

    private async void OnOkClick(object? sender, RoutedEventArgs e)
    {
        if (ViewModel == null)
        {
            Close(null);
            return;
        }

        var validationResult = ViewModel.ValidateSettings();
        if (!validationResult.IsSuccess)
        {
            await NotificationService.Instance.ShowValidationErrorAsync(
                validationResult.ErrorMsg ?? "Please check your settings and try again.");
            return;
        }

        Close(ViewModel);
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e)
    {
        Close(null);
    }
}
