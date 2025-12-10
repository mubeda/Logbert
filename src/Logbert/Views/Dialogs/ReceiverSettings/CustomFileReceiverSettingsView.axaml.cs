using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Couchcoding.Logbert.Interfaces;
using Couchcoding.Logbert.Services;
using Couchcoding.Logbert.ViewModels.Dialogs.ReceiverSettings;
using System.Linq;

namespace Couchcoding.Logbert.Views.Dialogs.ReceiverSettings;

public partial class CustomFileReceiverSettingsView : Window
{
    private CustomFileReceiverSettingsViewModel? ViewModel => DataContext as CustomFileReceiverSettingsViewModel;

    public CustomFileReceiverSettingsView()
    {
        InitializeComponent();
        DataContext = new CustomFileReceiverSettingsViewModel();
    }

    private async void OnBrowseClick(object? sender, RoutedEventArgs e)
    {
        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select Log File",
            AllowMultiple = false,
            FileTypeFilter = new[]
            {
                new FilePickerFileType("Log Files") { Patterns = new[] { "*.log", "*.txt" } },
                new FilePickerFileType("All Files") { Patterns = new[] { "*" } }
            }
        });

        if (files != null && files.Count > 0 && ViewModel != null)
        {
            ViewModel.FilePath = files[0].Path.LocalPath;
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
