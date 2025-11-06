using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Couchcoding.Logbert.ViewModels.Dialogs.ReceiverSettings;

namespace Couchcoding.Logbert.Views.Dialogs.ReceiverSettings;

public partial class Log4NetDirReceiverSettingsView : Window
{
    public Log4NetDirReceiverSettingsViewModel? ViewModel => DataContext as Log4NetDirReceiverSettingsViewModel;

    public Log4NetDirReceiverSettingsView()
    {
        InitializeComponent();
        DataContext = new Log4NetDirReceiverSettingsViewModel();
    }

    private async void OnBrowseClick(object? sender, RoutedEventArgs e)
    {
        var storageProvider = StorageProvider;
        if (storageProvider == null) return;

        var result = await storageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Select Log Directory",
            AllowMultiple = false
        });

        if (result != null && result.Count > 0 && ViewModel != null)
        {
            ViewModel.DirectoryPath = result[0].Path.LocalPath;
        }
    }

    private void OnOkClick(object? sender, RoutedEventArgs e)
    {
        if (ViewModel != null)
        {
            var validation = ViewModel.ValidateSettings();
            if (validation.IsSuccess)
            {
                Close(ViewModel);
            }
            else
            {
                // Show error message
                var messageBox = new Window
                {
                    Title = "Validation Error",
                    Width = 400,
                    Height = 150,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    Content = new StackPanel
                    {
                        Margin = new Avalonia.Thickness(20),
                        Spacing = 15,
                        Children =
                        {
                            new TextBlock { Text = validation.ErrorMsg, TextWrapping = Avalonia.Media.TextWrapping.Wrap },
                            new Button
                            {
                                Content = "OK",
                                Width = 80,
                                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
                                Command = new Avalonia.ReactiveUI.ReactiveCommand<object>(_ => { })
                            }
                        }
                    }
                };
                messageBox.ShowDialog(this);
            }
        }
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e)
    {
        Close(null);
    }
}
