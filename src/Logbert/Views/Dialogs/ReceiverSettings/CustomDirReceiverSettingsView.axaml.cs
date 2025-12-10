using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Couchcoding.Logbert.Interfaces;
using Couchcoding.Logbert.Services;
using Couchcoding.Logbert.ViewModels.Dialogs.ReceiverSettings;

namespace Couchcoding.Logbert.Views.Dialogs.ReceiverSettings;

public partial class CustomDirReceiverSettingsView : Window
{
    private CustomDirReceiverSettingsViewModel? ViewModel => DataContext as CustomDirReceiverSettingsViewModel;

    public CustomDirReceiverSettingsView()
    {
        InitializeComponent();
        DataContext = new CustomDirReceiverSettingsViewModel();
    }

    private async void OnBrowseClick(object? sender, RoutedEventArgs e)
    {
        var folders = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Select Log Directory",
            AllowMultiple = false
        });

        if (folders != null && folders.Count > 0 && ViewModel != null)
        {
            ViewModel.DirectoryPath = folders[0].Path.LocalPath;
        }
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
                validationResult.Message ?? "Please check your settings and try again.");
            return;
        }

        Close(ViewModel);
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e)
    {
        Close(null);
    }
}
