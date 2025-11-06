using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Couchcoding.Logbert.Interfaces;
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
        if (!validationResult.Success)
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
