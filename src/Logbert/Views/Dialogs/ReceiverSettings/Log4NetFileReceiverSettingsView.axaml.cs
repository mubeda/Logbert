using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Couchcoding.Logbert.ViewModels.Dialogs.ReceiverSettings;
using System.Linq;

namespace Couchcoding.Logbert.Views.Dialogs.ReceiverSettings
{
    public partial class Log4NetFileReceiverSettingsView : UserControl
    {
        public Log4NetFileReceiverSettingsView()
        {
            InitializeComponent();
        }

        public async void BrowseFile(object? sender, RoutedEventArgs e)
        {
            if (DataContext is not Log4NetFileReceiverSettingsViewModel viewModel)
                return;

            var topLevel = TopLevel.GetTopLevel(this);
            if (topLevel == null)
                return;

            var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Select Log4Net XML File",
                AllowMultiple = false,
                FileTypeFilter = new[]
                {
                    new FilePickerFileType("Log Files")
                    {
                        Patterns = new[] { "*.log", "*.xml", "*.txt" }
                    },
                    new FilePickerFileType("All Files")
                    {
                        Patterns = new[] { "*.*" }
                    }
                }
            });

            if (files.Count > 0)
            {
                viewModel.FilePath = files[0].Path.LocalPath;
            }
        }
    }
}
