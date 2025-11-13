using Avalonia.Controls;
using Avalonia.Interactivity;
using Couchcoding.Logbert.Interfaces;
using Couchcoding.Logbert.ViewModels.Dialogs.ReceiverSettings;

namespace Couchcoding.Logbert.Views.Dialogs.ReceiverSettings;

public partial class CustomUdpReceiverSettingsView : Window
{
    private CustomUdpReceiverSettingsViewModel? ViewModel => DataContext as CustomUdpReceiverSettingsViewModel;

    public CustomUdpReceiverSettingsView()
    {
        InitializeComponent();
        DataContext = new CustomUdpReceiverSettingsViewModel();
    }

    private void OnOkClick(object? sender, RoutedEventArgs e)
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
