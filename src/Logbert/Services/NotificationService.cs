using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;

namespace Couchcoding.Logbert.Services;

/// <summary>
/// Service for displaying notifications and error dialogs to users.
/// </summary>
public class NotificationService
{
    private static NotificationService? _instance;
    private static readonly object _lock = new();

    /// <summary>
    /// Gets the singleton instance of the NotificationService.
    /// </summary>
    public static NotificationService Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    _instance ??= new NotificationService();
                }
            }
            return _instance;
        }
    }

    /// <summary>
    /// Notification severity levels.
    /// </summary>
    public enum NotificationLevel
    {
        Info,
        Warning,
        Error
    }

    private NotificationService() { }

    /// <summary>
    /// Gets the main window of the application.
    /// </summary>
    private static Window? GetMainWindow()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            return desktop.MainWindow;
        }
        return null;
    }

    /// <summary>
    /// Shows an information message dialog.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="title">The dialog title.</param>
    public async Task ShowInfoAsync(string message, string title = "Information")
    {
        await ShowMessageAsync(message, title, NotificationLevel.Info);
    }

    /// <summary>
    /// Shows a warning message dialog.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="title">The dialog title.</param>
    public async Task ShowWarningAsync(string message, string title = "Warning")
    {
        await ShowMessageAsync(message, title, NotificationLevel.Warning);
    }

    /// <summary>
    /// Shows an error message dialog.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="title">The dialog title.</param>
    public async Task ShowErrorAsync(string message, string title = "Error")
    {
        await ShowMessageAsync(message, title, NotificationLevel.Error);
    }

    /// <summary>
    /// Shows an error message dialog with exception details.
    /// </summary>
    /// <param name="message">The user-friendly message to display.</param>
    /// <param name="exception">The exception with details.</param>
    /// <param name="title">The dialog title.</param>
    public async Task ShowErrorAsync(string message, Exception exception, string title = "Error")
    {
        await ShowErrorWithDetailsAsync(message, exception.ToString(), title);
    }

    /// <summary>
    /// Shows a validation error message dialog.
    /// </summary>
    /// <param name="message">The validation error message.</param>
    /// <param name="title">The dialog title.</param>
    public async Task ShowValidationErrorAsync(string message, string title = "Validation Error")
    {
        await ShowMessageAsync(message, title, NotificationLevel.Warning);
    }

    /// <summary>
    /// Shows a message dialog with the specified level.
    /// </summary>
    private async Task ShowMessageAsync(string message, string title, NotificationLevel level)
    {
        await Dispatcher.UIThread.InvokeAsync(async () =>
        {
            var mainWindow = GetMainWindow();
            if (mainWindow == null) return;

            var iconColor = level switch
            {
                NotificationLevel.Info => Brushes.DodgerBlue,
                NotificationLevel.Warning => Brushes.Orange,
                NotificationLevel.Error => Brushes.Red,
                _ => Brushes.Gray
            };

            var iconData = level switch
            {
                NotificationLevel.Info => "M13,9H11V7H13M13,17H11V11H13M12,2A10,10 0 0,0 2,12A10,10 0 0,0 12,22A10,10 0 0,0 22,12A10,10 0 0,0 12,2Z",
                NotificationLevel.Warning => "M13,14H11V10H13M13,18H11V16H13M1,21H23L12,2L1,21Z",
                NotificationLevel.Error => "M12,2C17.53,2 22,6.47 22,12C22,17.53 17.53,22 12,22C6.47,22 2,17.53 2,12C2,6.47 6.47,2 12,2M15.59,7L12,10.59L8.41,7L7,8.41L10.59,12L7,15.59L8.41,17L12,13.41L15.59,17L17,15.59L13.41,12L17,8.41L15.59,7Z",
                _ => "M13,9H11V7H13M13,17H11V11H13M12,2A10,10 0 0,0 2,12A10,10 0 0,0 12,22A10,10 0 0,0 22,12A10,10 0 0,0 12,2Z"
            };

            var dialog = new Window
            {
                Title = title,
                Width = 420,
                Height = 180,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                CanResize = false,
                Content = new DockPanel
                {
                    Margin = new Thickness(20),
                    Children =
                    {
                        CreateButton("OK", true),
                        CreateIconAndMessage(iconData, iconColor, message)
                    }
                }
            };

            // Set DockPanel.Dock for button
            DockPanel.SetDock((Control)((DockPanel)dialog.Content).Children[0], Avalonia.Controls.Dock.Bottom);

            await dialog.ShowDialog(mainWindow);
        });
    }

    /// <summary>
    /// Shows an error dialog with expandable details section.
    /// </summary>
    private async Task ShowErrorWithDetailsAsync(string message, string details, string title)
    {
        await Dispatcher.UIThread.InvokeAsync(async () =>
        {
            var mainWindow = GetMainWindow();
            if (mainWindow == null) return;

            var detailsExpander = new Expander
            {
                Header = "Details",
                Margin = new Thickness(0, 10, 0, 0),
                Content = new ScrollViewer
                {
                    MaxHeight = 150,
                    Content = new TextBox
                    {
                        Text = details,
                        IsReadOnly = true,
                        TextWrapping = TextWrapping.Wrap,
                        FontFamily = new FontFamily("Consolas,Courier New,monospace"),
                        FontSize = 11,
                        AcceptsReturn = true
                    }
                }
            };

            var dialog = new Window
            {
                Title = title,
                Width = 500,
                MinHeight = 200,
                MaxHeight = 450,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                SizeToContent = SizeToContent.Height,
                CanResize = false,
                Content = new DockPanel
                {
                    Margin = new Thickness(20),
                    Children =
                    {
                        CreateButton("OK", true),
                        new StackPanel
                        {
                            Children =
                            {
                                CreateIconAndMessage(
                                    "M12,2C17.53,2 22,6.47 22,12C22,17.53 17.53,22 12,22C6.47,22 2,17.53 2,12C2,6.47 6.47,2 12,2M15.59,7L12,10.59L8.41,7L7,8.41L10.59,12L7,15.59L8.41,17L12,13.41L15.59,17L17,15.59L13.41,12L17,8.41L15.59,7Z",
                                    Brushes.Red,
                                    message),
                                detailsExpander
                            }
                        }
                    }
                }
            };

            // Set DockPanel.Dock for button
            DockPanel.SetDock((Control)((DockPanel)dialog.Content).Children[0], Avalonia.Controls.Dock.Bottom);

            await dialog.ShowDialog(mainWindow);
        });
    }

    /// <summary>
    /// Shows a confirmation dialog with Yes/No buttons.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="title">The dialog title.</param>
    /// <returns>True if user clicked Yes, false otherwise.</returns>
    public async Task<bool> ShowConfirmationAsync(string message, string title = "Confirm")
    {
        return await Dispatcher.UIThread.InvokeAsync(async () =>
        {
            var mainWindow = GetMainWindow();
            if (mainWindow == null) return false;

            bool result = false;

            var yesButton = new Button
            {
                Content = "Yes",
                Padding = new Thickness(25, 8),
                Margin = new Thickness(0, 0, 10, 0)
            };

            var noButton = new Button
            {
                Content = "No",
                Padding = new Thickness(25, 8)
            };

            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 15, 0, 0),
                Children = { yesButton, noButton }
            };

            Window? dialog = null;

            yesButton.Click += (s, e) =>
            {
                result = true;
                dialog?.Close();
            };

            noButton.Click += (s, e) =>
            {
                result = false;
                dialog?.Close();
            };

            dialog = new Window
            {
                Title = title,
                Width = 400,
                Height = 170,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                CanResize = false,
                Content = new DockPanel
                {
                    Margin = new Thickness(20),
                    Children =
                    {
                        buttonPanel,
                        CreateIconAndMessage(
                            "M10,19H13V22H10V19M12,2C17.35,2.22 19.68,7.62 16.5,11.67C15.67,12.67 14.33,13.33 13.67,14.17C13,15 13,16 13,17H10C10,15.33 10,13.92 10.67,12.92C11.33,11.92 12.67,11.33 13.5,10.67C15.92,8.43 15.32,5.26 12,5A3,3 0 0,0 9,8H6A6,6 0 0,1 12,2Z",
                            Brushes.DodgerBlue,
                            message)
                    }
                }
            };

            DockPanel.SetDock(buttonPanel, Avalonia.Controls.Dock.Bottom);

            await dialog.ShowDialog(mainWindow);
            return result;
        });
    }

    private static StackPanel CreateIconAndMessage(string iconData, IBrush iconColor, string message)
    {
        return new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Children =
            {
                new PathIcon
                {
                    Data = StreamGeometry.Parse(iconData),
                    Foreground = iconColor,
                    Width = 32,
                    Height = 32,
                    Margin = new Thickness(0, 0, 15, 0),
                    VerticalAlignment = VerticalAlignment.Top
                },
                new TextBlock
                {
                    Text = message,
                    TextWrapping = TextWrapping.Wrap,
                    VerticalAlignment = VerticalAlignment.Center,
                    MaxWidth = 320
                }
            }
        };
    }

    private static Button CreateButton(string text, bool isDefault)
    {
        var button = new Button
        {
            Content = text,
            Padding = new Thickness(30, 8),
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(0, 15, 0, 0),
            IsDefault = isDefault
        };

        button.Click += (s, e) =>
        {
            if (button.Parent is DockPanel panel && panel.Parent is Window window)
            {
                window.Close();
            }
        };

        return button;
    }
}
