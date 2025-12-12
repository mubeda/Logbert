using Avalonia.Controls;
using Avalonia.Interactivity;
using Logbert.Interfaces;
using Logbert.Services;
using Logbert.ViewModels.Dialogs.ReceiverSettings;

namespace Logbert.Views.Dialogs.ReceiverSettings;

public partial class CustomHttpReceiverSettingsView : Window
{
    private CustomHttpReceiverSettingsViewModel? ViewModel => DataContext as CustomHttpReceiverSettingsViewModel;

    public CustomHttpReceiverSettingsView()
    {
        InitializeComponent();
        DataContext = new CustomHttpReceiverSettingsViewModel();
    }

    private async void OnOkClick(object? sender, RoutedEventArgs e)
    {
        if (ViewModel == null)
        {
            Close(null);
            return;
        }

        // Set the columnizer from the editor control
        if (ColumnizerEditor?.ViewModel != null)
        {
            ViewModel.Columnizer = ColumnizerEditor.ViewModel.GetColumnizer();
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
