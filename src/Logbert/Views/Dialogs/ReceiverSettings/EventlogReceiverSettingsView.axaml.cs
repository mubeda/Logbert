using Avalonia.Controls;
using Avalonia.Interactivity;
using Couchcoding.Logbert.Interfaces;
using Couchcoding.Logbert.ViewModels.Dialogs.ReceiverSettings;

namespace Couchcoding.Logbert.Views.Dialogs.ReceiverSettings;

public partial class EventlogReceiverSettingsView : Window
{
    private EventlogReceiverSettingsViewModel? ViewModel => DataContext as EventlogReceiverSettingsViewModel;

    public EventlogReceiverSettingsView()
    {
        InitializeComponent();
        DataContext = new EventlogReceiverSettingsViewModel();
    }

    private void OnOkClick(object? sender, RoutedEventArgs e)
    {
        if (ViewModel == null)
        {
            Close(null);
            return;
        }

        var validationResult = ViewModel.ValidateSettings();
        if (!validationResult.IsSuccess)
        {
            // TODO: Show validation error dialog
            // For now, just close with null
            Close(null);
            return;
        }

        Close(ViewModel);
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e)
    {
        Close(null);
    }
}
