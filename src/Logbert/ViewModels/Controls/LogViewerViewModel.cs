using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Logbert.Logging;
using Logbert.Interfaces;
using Logbert.Services;
using Avalonia.Threading;
using Avalonia.Media;

namespace Logbert.ViewModels.Controls;

/// <summary>
/// ViewModel for the log viewer control.
/// </summary>
public partial class LogViewerViewModel : ViewModelBase, ILogHandler
{
    /// <summary>
    /// Event raised when messages are added to or removed from the viewer.
    /// </summary>
    public event EventHandler<NotifyCollectionChangedEventArgs>? MessagesUpdated;

    [ObservableProperty]
    private ObservableCollection<LogMessage> _messages = new();

    [ObservableProperty]
    private LogMessage? _selectedMessage;

    [ObservableProperty]
    private double _fontSize = 11;

    [ObservableProperty]
    private bool _showTrace = true;

    [ObservableProperty]
    private bool _showDebug = true;

    [ObservableProperty]
    private bool _showInfo = true;

    [ObservableProperty]
    private bool _showWarning = true;

    [ObservableProperty]
    private bool _showError = true;

    [ObservableProperty]
    private bool _showFatal = true;

    /// <summary>
    /// Gets or sets whether the error panel is visible.
    /// </summary>
    [ObservableProperty]
    private bool _errorPanelVisible;

    /// <summary>
    /// Gets or sets the error panel title.
    /// </summary>
    [ObservableProperty]
    private string _errorPanelTitle = string.Empty;

    /// <summary>
    /// Gets or sets the error panel message.
    /// </summary>
    [ObservableProperty]
    private string _errorPanelMessage = string.Empty;

    /// <summary>
    /// Gets or sets the error panel background brush.
    /// </summary>
    [ObservableProperty]
    private IBrush _errorPanelBackgroundBrush = new SolidColorBrush(Color.FromRgb(255, 165, 0));

    /// <summary>
    /// Gets or sets the error panel foreground brush.
    /// </summary>
    [ObservableProperty]
    private IBrush _errorPanelForegroundBrush = new SolidColorBrush(Colors.Black);

    /// <summary>
    /// Tracks the count of suppressed errors for the same message.
    /// </summary>
    private int _errorCount;

    /// <summary>
    /// Tracks the last error message for deduplication.
    /// </summary>
    private string _lastErrorMessage = string.Empty;

    /// <summary>
    /// Gets the filtered messages based on log level visibility.
    /// </summary>
    public ObservableCollection<LogMessage> FilteredMessages { get; } = new();

    /// <summary>
    /// Gets the command to zoom in.
    /// </summary>
    public IRelayCommand ZoomInCommand { get; } = null!;

    /// <summary>
    /// Gets the command to zoom out.
    /// </summary>
    public IRelayCommand ZoomOutCommand { get; } = null!;

    /// <summary>
    /// Gets the command to copy selected message.
    /// </summary>
    public IRelayCommand CopyMessageCommand { get; } = null!;

    /// <summary>
    /// Gets the command to dismiss the error panel.
    /// </summary>
    public IRelayCommand DismissErrorPanelCommand { get; } = null!;

    /// <summary>
    /// Initializes a new instance of the <see cref="LogViewerViewModel"/> class.
    /// </summary>
    public LogViewerViewModel()
    {
        ZoomInCommand = new RelayCommand(OnZoomIn, CanZoomIn);
        ZoomOutCommand = new RelayCommand(OnZoomOut, CanZoomOut);
        CopyMessageCommand = new RelayCommand(OnCopyMessage, CanCopyMessage);
        DismissErrorPanelCommand = new RelayCommand(OnDismissErrorPanel);

        // Listen for changes to log level filters
        PropertyChanged += (s, e) =>
        {
            if (e.PropertyName?.StartsWith("Show") == true)
            {
                UpdateFilteredMessages();
            }
        };
    }

    /// <summary>
    /// Dismisses the error panel.
    /// </summary>
    private void OnDismissErrorPanel()
    {
        ErrorPanelVisible = false;
        _errorCount = 0;
        _lastErrorMessage = string.Empty;
    }

    /// <summary>
    /// Updates the messages collection.
    /// </summary>
    public void UpdateMessages(ObservableCollection<LogMessage> messages)
    {
        Messages = messages;
        UpdateFilteredMessages();
    }

    /// <summary>
    /// Updates the filtered messages based on current filter settings.
    /// </summary>
    private void UpdateFilteredMessages()
    {
        FilteredMessages.Clear();

        foreach (LogMessage message in Messages)
        {
            if (ShouldShowMessage(message))
            {
                FilteredMessages.Add(message);
            }
        }
    }

    /// <summary>
    /// Determines if a message should be shown based on filter settings.
    /// </summary>
    private bool ShouldShowMessage(LogMessage message)
    {
        return message.Level switch
        {
            LogLevel.Trace => ShowTrace,
            LogLevel.Debug => ShowDebug,
            LogLevel.Info => ShowInfo,
            LogLevel.Warning => ShowWarning,
            LogLevel.Error => ShowError,
            LogLevel.Fatal => ShowFatal,
            _ => true
        };
    }

    private bool CanZoomIn() => FontSize < 60;

    private void OnZoomIn()
    {
        if (FontSize < 60)
        {
            FontSize += 2;
        }
    }

    private bool CanZoomOut() => FontSize > 6;

    private void OnZoomOut()
    {
        if (FontSize > 6)
        {
            FontSize -= 2;
        }
    }

    private bool CanCopyMessage() => SelectedMessage != null;

    private void OnCopyMessage()
    {
        // TODO: Implement clipboard copy
    }

    /// <summary>
    /// Handles a single log message from the receiver.
    /// </summary>
    public void HandleMessage(LogMessage logMsg)
    {
        Dispatcher.UIThread.Post(() =>
        {
            Messages.Add(logMsg);

            if (ShouldShowMessage(logMsg))
            {
                FilteredMessages.Add(logMsg);
            }

            // Notify subscribers that messages were updated
            MessagesUpdated?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, logMsg));
        });
    }

    /// <summary>
    /// Handles multiple log messages from the receiver.
    /// </summary>
    public void HandleMessage(LogMessage[] logMsgs)
    {
        Dispatcher.UIThread.Post(() =>
        {
            foreach (LogMessage msg in logMsgs)
            {
                Messages.Add(msg);

                if (ShouldShowMessage(msg))
                {
                    FilteredMessages.Add(msg);
                }
            }

            // Notify subscribers that messages were updated
            MessagesUpdated?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, logMsgs));
        });
    }

    /// <summary>
    /// Handles an error from the receiver by showing a non-blocking error panel.
    /// </summary>
    public void HandleError(LogError error)
    {
        Dispatcher.UIThread.Post(() =>
        {

            // Check if this is the same error message (for deduplication)
            if (error.Message == _lastErrorMessage)
            {
                _errorCount++;
                // Update message with count if there are repeated errors
                ErrorPanelMessage = _errorCount > 0
                    ? $"{error.Message}{(error.Message.EndsWith(".") ? string.Empty : ".")} (+{_errorCount} similar)"
                    : $"{error.Message}{(error.Message.EndsWith(".") ? string.Empty : ".")}";
            }
            else
            {
                // New error message
                _lastErrorMessage = error.Message;
                _errorCount = 0;
                ErrorPanelMessage = $"{error.Message}{(error.Message.EndsWith(".") ? string.Empty : ".")}";
            }

            // Set panel properties based on error type
            ErrorPanelTitle = error.Title;

            // Convert System.Drawing.Color to Avalonia brushes
            ErrorPanelBackgroundBrush = new SolidColorBrush(Color.FromRgb(error.BackColor.R, error.BackColor.G, error.BackColor.B));
            ErrorPanelForegroundBrush = new SolidColorBrush(Color.FromRgb(error.ForeColor.R, error.ForeColor.G, error.ForeColor.B));

            // Show the panel (non-blocking)
            ErrorPanelVisible = true;
        });
    }

    /// <summary>
    /// Event raised when column configuration changes.
    /// </summary>
    public event System.EventHandler? ColumnConfigurationChanged;

    /// <summary>
    /// Refreshes the column configuration from settings.
    /// </summary>
    public void RefreshColumnConfiguration()
    {
        ColumnConfigurationChanged?.Invoke(this, System.EventArgs.Empty);
    }
}
