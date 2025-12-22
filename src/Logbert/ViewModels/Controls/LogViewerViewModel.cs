using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
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
    /// Gets or sets the logger filter path. Null or empty means no filter.
    /// </summary>
    [ObservableProperty]
    private string? _loggerFilterPath;

    /// <summary>
    /// Gets or sets whether the logger filter is recursive (includes child loggers).
    /// </summary>
    [ObservableProperty]
    private bool _loggerFilterRecursive = true;

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
    /// Reference to the log provider/receiver for pause control.
    /// </summary>
    private ILogProvider? _logProvider;

    /// <summary>
    /// Queue for batching incoming log messages.
    /// </summary>
    private readonly ConcurrentQueue<LogMessage> _messageQueue = new();

    /// <summary>
    /// Timer for processing batched messages.
    /// </summary>
    private DispatcherTimer? _batchTimer;

    /// <summary>
    /// Flag to track if a batch process is pending.
    /// </summary>
    private bool _batchProcessPending;

    /// <summary>
    /// Maximum number of messages to process in one batch.
    /// </summary>
    private const int MaxBatchSize = 100;

    /// <summary>
    /// Interval for batch processing in milliseconds.
    /// </summary>
    private const int BatchIntervalMs = 50;

    /// <summary>
    /// Gets or sets whether log receiving is paused.
    /// </summary>
    [ObservableProperty]
    private bool _isPaused;

    /// <summary>
    /// Gets the filtered messages based on log level visibility.
    /// </summary>
    public ObservableCollection<LogMessage> FilteredMessages { get; } = new();

    #region Column Grouping

    /// <summary>
    /// Gets or sets whether grouping is enabled.
    /// </summary>
    [ObservableProperty]
    private bool _isGroupingEnabled;

    /// <summary>
    /// Gets or sets the currently selected grouping column.
    /// </summary>
    [ObservableProperty]
    private GroupableColumn? _selectedGroupingColumn;

    /// <summary>
    /// Gets the list of columns that can be grouped.
    /// </summary>
    public ObservableCollection<GroupableColumn> GroupableColumns { get; } = new()
    {
        new GroupableColumn { Name = "Level", PropertyName = "Level" },
        new GroupableColumn { Name = "Logger", PropertyName = "Logger" },
        new GroupableColumn { Name = "Thread", PropertyName = "ThreadName" },
        new GroupableColumn { Name = "Machine", PropertyName = "MachineName" },
        new GroupableColumn { Name = "Process", PropertyName = "ProcessName" }
    };

    /// <summary>
    /// Gets the columns currently being used for grouping.
    /// </summary>
    public ObservableCollection<GroupableColumn> ActiveGroupingColumns { get; } = new();

    /// <summary>
    /// Gets the grouped messages when grouping is enabled.
    /// </summary>
    public ObservableCollection<LogMessageGroup> GroupedMessages { get; } = new();

    /// <summary>
    /// Gets the command to add a grouping column.
    /// </summary>
    public IRelayCommand<GroupableColumn> AddGroupingColumnCommand { get; } = null!;

    /// <summary>
    /// Gets the command to remove a grouping column.
    /// </summary>
    public IRelayCommand<GroupableColumn> RemoveGroupingColumnCommand { get; } = null!;

    /// <summary>
    /// Gets the command to clear all grouping.
    /// </summary>
    public IRelayCommand ClearGroupingCommand { get; } = null!;

    /// <summary>
    /// Gets the command to toggle a group's expanded state.
    /// </summary>
    public IRelayCommand<LogMessageGroup> ToggleGroupExpandedCommand { get; } = null!;

    #endregion

    #region Column Visibility

    /// <summary>
    /// Gets the column configurations for visibility control.
    /// </summary>
    public ObservableCollection<ColumnConfig> ColumnConfigs { get; } = new();

    /// <summary>
    /// Gets the command to toggle a column's visibility.
    /// </summary>
    public IRelayCommand<ColumnConfig> ToggleColumnVisibilityCommand { get; } = null!;

    #endregion

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
    /// Gets the command to clear all messages.
    /// </summary>
    public IRelayCommand ClearMessagesCommand { get; } = null!;

    /// <summary>
    /// Gets the command to toggle pause/resume log receiving.
    /// </summary>
    public IRelayCommand TogglePauseCommand { get; } = null!;

    /// <summary>
    /// Initializes a new instance of the <see cref="LogViewerViewModel"/> class.
    /// </summary>
    public LogViewerViewModel()
    {
        ZoomInCommand = new RelayCommand(OnZoomIn, CanZoomIn);
        ZoomOutCommand = new RelayCommand(OnZoomOut, CanZoomOut);
        CopyMessageCommand = new RelayCommand(OnCopyMessage, CanCopyMessage);
        DismissErrorPanelCommand = new RelayCommand(OnDismissErrorPanel);
        ClearMessagesCommand = new RelayCommand(OnClearMessages, CanClearMessages);
        TogglePauseCommand = new RelayCommand(OnTogglePause, CanTogglePause);

        // Column grouping commands
        AddGroupingColumnCommand = new RelayCommand<GroupableColumn>(OnAddGroupingColumn);
        RemoveGroupingColumnCommand = new RelayCommand<GroupableColumn>(OnRemoveGroupingColumn);
        ClearGroupingCommand = new RelayCommand(OnClearGrouping);
        ToggleGroupExpandedCommand = new RelayCommand<LogMessageGroup>(OnToggleGroupExpanded);

        // Column visibility command
        ToggleColumnVisibilityCommand = new RelayCommand<ColumnConfig>(OnToggleColumnVisibility);

        // Initialize column configurations
        InitializeColumnConfigs();

        // Load saved grouping settings
        LoadGroupingSettings();

        // Listen for changes to log level filters and logger filters
        PropertyChanged += (s, e) =>
        {
            if (e.PropertyName?.StartsWith("Show") == true ||
                e.PropertyName == nameof(LoggerFilterPath) ||
                e.PropertyName == nameof(LoggerFilterRecursive))
            {
                UpdateFilteredMessages();
            }
        };

        // Initialize batch processing timer
        _batchTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(BatchIntervalMs)
        };
        _batchTimer.Tick += (s, e) => ProcessMessageBatch();
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
    /// Returns whether the clear messages command can execute.
    /// </summary>
    private bool CanClearMessages() => Messages.Count > 0;

    /// <summary>
    /// Clears all messages from the log viewer.
    /// </summary>
    private void OnClearMessages()
    {
        // Clear the message queue first
        while (_messageQueue.TryDequeue(out _)) { }

        // Stop batch processing
        _batchTimer?.Stop();
        _batchProcessPending = false;

        Messages.Clear();
        FilteredMessages.Clear();
        GroupedMessages.Clear();
        SelectedMessage = null;
        ErrorPanelVisible = false;
        _errorCount = 0;
        _lastErrorMessage = string.Empty;

        // Update command state
        ((RelayCommand)ClearMessagesCommand).NotifyCanExecuteChanged();
    }

    /// <summary>
    /// Returns whether the toggle pause command can execute.
    /// </summary>
    private bool CanTogglePause() => _logProvider != null;

    /// <summary>
    /// Toggles pause/resume for log receiving.
    /// </summary>
    private void OnTogglePause()
    {
        if (_logProvider != null)
        {
            IsPaused = !IsPaused;
            _logProvider.IsActive = !IsPaused;
        }
    }

    /// <summary>
    /// Sets the log provider for pause control.
    /// </summary>
    public void SetLogProvider(ILogProvider provider)
    {
        _logProvider = provider;
        IsPaused = !provider.IsActive;
        ((RelayCommand)TogglePauseCommand).NotifyCanExecuteChanged();
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

        // Update grouped messages if grouping is enabled
        if (IsGroupingEnabled)
        {
            UpdateGroupedMessages();
        }
    }

    /// <summary>
    /// Determines if a message should be shown based on filter settings.
    /// </summary>
    private bool ShouldShowMessage(LogMessage message)
    {
        // Check log level filter first
        bool levelMatch = message.Level switch
        {
            LogLevel.Trace => ShowTrace,
            LogLevel.Debug => ShowDebug,
            LogLevel.Info => ShowInfo,
            LogLevel.Warning => ShowWarning,
            LogLevel.Error => ShowError,
            LogLevel.Fatal => ShowFatal,
            _ => true
        };

        if (!levelMatch)
        {
            return false;
        }

        // Check logger filter
        if (!string.IsNullOrEmpty(LoggerFilterPath))
        {
            var messageLogger = message.Logger ?? string.Empty;
            if (LoggerFilterRecursive)
            {
                // Recursive: logger must start with the filter path
                if (!messageLogger.StartsWith(LoggerFilterPath, StringComparison.Ordinal))
                {
                    return false;
                }
            }
            else
            {
                // Exact match: logger must equal the filter path
                if (!messageLogger.Equals(LoggerFilterPath, StringComparison.Ordinal))
                {
                    return false;
                }
            }
        }

        return true;
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

    private async void OnCopyMessage()
    {
        if (SelectedMessage == null) return;

        try
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var mainWindow = desktop.MainWindow;
                if (mainWindow?.Clipboard != null)
                {
                    await mainWindow.Clipboard.SetTextAsync(SelectedMessage.Message);
                }
            }
        }
        catch
        {
            // Ignore clipboard errors
        }
    }

    /// <summary>
    /// Handles a single log message from the receiver.
    /// Uses batching to prevent UI thread flooding.
    /// </summary>
    public void HandleMessage(LogMessage logMsg)
    {
        // Queue the message for batch processing
        _messageQueue.Enqueue(logMsg);

        // Start the batch timer if not already running
        if (!_batchProcessPending)
        {
            _batchProcessPending = true;
            Dispatcher.UIThread.Post(StartBatchTimer, DispatcherPriority.Background);
        }
    }

    /// <summary>
    /// Handles multiple log messages from the receiver.
    /// Uses batching to prevent UI thread flooding.
    /// </summary>
    public void HandleMessage(LogMessage[] logMsgs)
    {
        // Queue all messages for batch processing
        foreach (var msg in logMsgs)
        {
            _messageQueue.Enqueue(msg);
        }

        // Start the batch timer if not already running
        if (!_batchProcessPending)
        {
            _batchProcessPending = true;
            Dispatcher.UIThread.Post(StartBatchTimer, DispatcherPriority.Background);
        }
    }

    /// <summary>
    /// Starts the batch processing timer on the UI thread.
    /// </summary>
    private void StartBatchTimer()
    {
        if (_batchTimer != null && !_batchTimer.IsEnabled)
        {
            _batchTimer.Start();
        }
    }

    /// <summary>
    /// Processes a batch of queued messages.
    /// </summary>
    private void ProcessMessageBatch()
    {
        var processedMessages = new List<LogMessage>();
        int count = 0;

        // Dequeue up to MaxBatchSize messages
        while (count < MaxBatchSize && _messageQueue.TryDequeue(out var msg))
        {
            Messages.Add(msg);

            if (ShouldShowMessage(msg))
            {
                FilteredMessages.Add(msg);
            }

            processedMessages.Add(msg);
            count++;
        }

        // If we processed any messages, notify once
        if (processedMessages.Count > 0)
        {
            // Notify subscribers that messages were updated (single notification for batch)
            MessagesUpdated?.Invoke(this, new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Add,
                processedMessages));

            // Update command state once per batch
            ((RelayCommand)ClearMessagesCommand).NotifyCanExecuteChanged();
        }

        // Stop timer if queue is empty
        if (_messageQueue.IsEmpty)
        {
            _batchTimer?.Stop();
            _batchProcessPending = false;
        }
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

    #region Column Grouping Methods

    /// <summary>
    /// Adds a column to the grouping.
    /// </summary>
    private void OnAddGroupingColumn(GroupableColumn? column)
    {
        if (column == null || ActiveGroupingColumns.Contains(column))
        {
            return;
        }

        column.IsGrouped = true;
        ActiveGroupingColumns.Add(column);
        IsGroupingEnabled = ActiveGroupingColumns.Count > 0;
        UpdateGroupedMessages();
        SaveGroupingSettings();
    }

    /// <summary>
    /// Removes a column from the grouping.
    /// </summary>
    private void OnRemoveGroupingColumn(GroupableColumn? column)
    {
        if (column == null || !ActiveGroupingColumns.Contains(column))
        {
            return;
        }

        column.IsGrouped = false;
        ActiveGroupingColumns.Remove(column);
        IsGroupingEnabled = ActiveGroupingColumns.Count > 0;
        UpdateGroupedMessages();
        SaveGroupingSettings();
    }

    /// <summary>
    /// Clears all grouping.
    /// </summary>
    private void OnClearGrouping()
    {
        foreach (var column in ActiveGroupingColumns)
        {
            column.IsGrouped = false;
        }

        ActiveGroupingColumns.Clear();
        GroupedMessages.Clear();
        IsGroupingEnabled = false;
        SaveGroupingSettings();
    }

    /// <summary>
    /// Toggles a group's expanded state.
    /// </summary>
    private void OnToggleGroupExpanded(LogMessageGroup? group)
    {
        if (group != null)
        {
            group.IsExpanded = !group.IsExpanded;
        }
    }

    /// <summary>
    /// Updates the grouped messages based on active grouping columns.
    /// </summary>
    private void UpdateGroupedMessages()
    {
        GroupedMessages.Clear();

        if (!IsGroupingEnabled || ActiveGroupingColumns.Count == 0)
        {
            return;
        }

        // Get the first grouping column
        var groupingColumn = ActiveGroupingColumns[0];

        // Group the filtered messages by the selected column
        var groups = FilteredMessages
            .GroupBy(msg => GetPropertyValue(msg, groupingColumn.PropertyName))
            .OrderBy(g => g.Key);

        foreach (var group in groups)
        {
            var messageGroup = new LogMessageGroup(
                groupingColumn.Name,
                group.Key ?? "Unknown",
                group);
            GroupedMessages.Add(messageGroup);
        }
    }

    /// <summary>
    /// Gets the value of a property from a log message.
    /// </summary>
    private static string? GetPropertyValue(LogMessage message, string propertyName)
    {
        return propertyName switch
        {
            "Level" => message.Level.ToString(),
            "Logger" => message.Logger,
            "Message" => message.Message,
            "ThreadName" => message.ThreadName ?? "(none)",
            "MachineName" => message.MachineName ?? "(none)",
            "ProcessName" => message.ProcessName ?? "(none)",
            _ => null
        };
    }

    /// <summary>
    /// Called when IsGroupingEnabled changes.
    /// </summary>
    partial void OnIsGroupingEnabledChanged(bool value)
    {
        if (!value)
        {
            GroupedMessages.Clear();
        }
        else
        {
            UpdateGroupedMessages();
        }
    }

    #endregion

    #region Column Visibility Methods

    /// <summary>
    /// Initializes column configurations from settings.
    /// </summary>
    private void InitializeColumnConfigs()
    {
        var settings = SettingsService.Instance.Settings;
        var columns = new[]
        {
            new { Name = "Number", Header = "#", Binding = "Index", DefaultWidth = 60.0 },
            new { Name = "Timestamp", Header = "Timestamp", Binding = "Timestamp", DefaultWidth = 180.0 },
            new { Name = "Level", Header = "Level", Binding = "Level", DefaultWidth = 80.0 },
            new { Name = "Logger", Header = "Logger", Binding = "Logger", DefaultWidth = 200.0 },
            new { Name = "Thread", Header = "Thread", Binding = "ThreadName", DefaultWidth = 80.0 },
            new { Name = "Machine", Header = "Machine", Binding = "MachineName", DefaultWidth = 100.0 },
            new { Name = "Process", Header = "Process", Binding = "ProcessName", DefaultWidth = 100.0 },
            new { Name = "Message", Header = "Message", Binding = "Message", DefaultWidth = 400.0 }
        };

        for (int i = 0; i < columns.Length; i++)
        {
            var col = columns[i];
            var isVisible = settings.ColumnVisibility.TryGetValue(col.Name, out var visible) ? visible : true;
            var width = settings.ColumnWidths.TryGetValue(col.Name, out var w) ? w : col.DefaultWidth;
            var order = settings.ColumnOrder.TryGetValue(col.Name, out var o) ? o : i;

            var config = new ColumnConfig
            {
                Name = col.Name,
                Header = col.Header,
                BindingProperty = col.Binding,
                IsVisible = isVisible,
                Width = width,
                DisplayOrder = order
            };

            // Subscribe to visibility changes
            config.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ColumnConfig.IsVisible))
                {
                    SaveColumnVisibilitySettings();
                }
            };

            ColumnConfigs.Add(config);
        }
    }

    /// <summary>
    /// Toggles a column's visibility.
    /// </summary>
    private void OnToggleColumnVisibility(ColumnConfig? column)
    {
        if (column != null)
        {
            column.IsVisible = !column.IsVisible;
        }
    }

    /// <summary>
    /// Saves column visibility settings.
    /// </summary>
    private void SaveColumnVisibilitySettings()
    {
        SettingsService.Instance.UpdateSettings(settings =>
        {
            foreach (var col in ColumnConfigs)
            {
                settings.ColumnVisibility[col.Name] = col.IsVisible;
            }
        });
        SettingsService.Instance.SaveIfDirty();
    }

    /// <summary>
    /// Loads grouping settings from storage.
    /// </summary>
    private void LoadGroupingSettings()
    {
        var settings = SettingsService.Instance.Settings;
        if (settings.ActiveGroupingColumns.Count > 0)
        {
            foreach (var columnName in settings.ActiveGroupingColumns)
            {
                var column = GroupableColumns.FirstOrDefault(c => c.Name == columnName);
                if (column != null)
                {
                    OnAddGroupingColumn(column);
                }
            }
        }
    }

    /// <summary>
    /// Saves grouping settings to storage.
    /// </summary>
    private void SaveGroupingSettings()
    {
        SettingsService.Instance.UpdateSettings(settings =>
        {
            settings.ActiveGroupingColumns.Clear();
            foreach (var col in ActiveGroupingColumns)
            {
                settings.ActiveGroupingColumns.Add(col.Name);
            }
        });
        SettingsService.Instance.SaveIfDirty();
    }

    #endregion
}
