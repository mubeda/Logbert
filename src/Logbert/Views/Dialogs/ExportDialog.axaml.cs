using System;
using System.Collections.Generic;
using System.Threading;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Logbert.Logging;
using Logbert.Services;
using Logbert.ViewModels.Dialogs;

namespace Logbert.Views.Dialogs;

public partial class ExportDialog : Window
{
    private CancellationTokenSource? _cancellationTokenSource;

    /// <summary>
    /// Gets the view model.
    /// </summary>
    public ExportDialogViewModel ViewModel { get; }

    public ExportDialog()
    {
        InitializeComponent();

        ViewModel = new ExportDialogViewModel();
        DataContext = ViewModel;
    }

    /// <summary>
    /// Sets the messages to export.
    /// </summary>
    /// <param name="allMessages">All messages.</param>
    /// <param name="filteredMessages">Filtered messages (optional).</param>
    public void SetMessages(IReadOnlyList<LogMessage> allMessages, IReadOnlyList<LogMessage>? filteredMessages = null)
    {
        ViewModel.SetMessages(allMessages, filteredMessages);
    }

    private async void OnBrowseClick(object? sender, RoutedEventArgs e)
    {
        var topLevel = GetTopLevel(this);
        if (topLevel == null) return;

        var extension = ViewModel.GetDefaultExtension();
        var formatName = ViewModel.GetExportFormat() == ExportService.ExportFormat.Csv
            ? "CSV Files"
            : "Text Files";

        var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Export Log Messages",
            DefaultExtension = extension.TrimStart('.'),
            SuggestedFileName = $"logbert_export{extension}",
            FileTypeChoices = new[]
            {
                new FilePickerFileType(formatName)
                {
                    Patterns = new[] { $"*{extension}" }
                },
                new FilePickerFileType("All Files")
                {
                    Patterns = new[] { "*.*" }
                }
            }
        });

        if (file != null)
        {
            ViewModel.FilePath = file.Path.LocalPath;
        }
    }

    private async void OnExportClick(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(ViewModel.FilePath))
        {
            return;
        }

        var messages = ViewModel.GetMessagesToExport();
        if (messages.Count == 0)
        {
            return;
        }

        ViewModel.IsExporting = true;
        ViewModel.ProgressPercentage = 0;
        ViewModel.ProgressStatus = "Starting export...";

        _cancellationTokenSource = new CancellationTokenSource();

        try
        {
            var exportService = new ExportService();

            // Subscribe to progress events
            exportService.ProgressChanged += (s, args) =>
            {
                // Update UI on the UI thread
                Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                {
                    ViewModel.ProgressPercentage = args.Percentage;
                    ViewModel.ProgressStatus = $"Exporting... {args.Current:N0} of {args.Total:N0} messages ({args.Percentage:F1}%)";
                });
            };

            var encoding = ViewModel.SelectedEncoding?.Encoding ?? System.Text.Encoding.UTF8;

            if (ViewModel.GetExportFormat() == ExportService.ExportFormat.Csv)
            {
                await exportService.ExportToCsvAsync(
                    messages,
                    ViewModel.FilePath,
                    encoding,
                    ViewModel.IncludeHeaders,
                    ',',
                    _cancellationTokenSource.Token);
            }
            else
            {
                await exportService.ExportToTextAsync(
                    messages,
                    ViewModel.FilePath,
                    encoding,
                    _cancellationTokenSource.Token);
            }

            ViewModel.ProgressStatus = $"Export complete! {messages.Count:N0} messages exported.";

            // Close dialog after successful export
            await System.Threading.Tasks.Task.Delay(1000);
            Close(true);
        }
        catch (OperationCanceledException)
        {
            ViewModel.ProgressStatus = "Export cancelled.";
        }
        catch (Exception ex)
        {
            ViewModel.ProgressStatus = $"Export failed: {ex.Message}";

            // Show error dialog
            var errorDialog = new Window
            {
                Title = "Export Error",
                Width = 400,
                Height = 150,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Content = new StackPanel
                {
                    Margin = new Avalonia.Thickness(20),
                    Spacing = 15,
                    Children =
                    {
                        new TextBlock
                        {
                            Text = $"Failed to export: {ex.Message}",
                            TextWrapping = Avalonia.Media.TextWrapping.Wrap
                        },
                        new Button
                        {
                            Content = "OK",
                            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                            Padding = new Avalonia.Thickness(30, 8)
                        }
                    }
                }
            };
            await errorDialog.ShowDialog(this);
        }
        finally
        {
            ViewModel.IsExporting = false;
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e)
    {
        if (ViewModel.IsExporting)
        {
            _cancellationTokenSource?.Cancel();
        }
        else
        {
            Close(false);
        }
    }
}
