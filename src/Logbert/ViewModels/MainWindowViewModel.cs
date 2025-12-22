using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Logbert.Helper;
using Logbert.Logging;
using Logbert.Logging.Sample;
using Logbert.Services;
using Logbert.ViewModels.Docking;

namespace Logbert.ViewModels;

/// <summary>
/// ViewModel for the main application window.
/// </summary>
public partial class MainWindowViewModel : ViewModelBase
{
    /// <summary>
    /// Gets or sets the collection of open log documents.
    /// </summary>
    public ObservableCollection<LogDocumentViewModel> Documents { get; } = new();

    /// <summary>
    /// Gets or sets the currently active document.
    /// </summary>
    [ObservableProperty]
    private LogDocumentViewModel? _activeDocument;

    #region Panel ViewModels

    /// <summary>
    /// Gets the filter panel view model.
    /// </summary>
    public FilterPanelViewModel FilterPanel { get; } = new();

    /// <summary>
    /// Gets the logger tree view model.
    /// </summary>
    public LoggerTreeViewModel LoggerTree { get; } = new();

    /// <summary>
    /// Gets the bookmarks panel view model.
    /// </summary>
    public BookmarksPanelViewModel BookmarksPanel { get; } = new();

    /// <summary>
    /// Gets the search panel view model.
    /// </summary>
    public SearchPanelViewModel SearchPanel { get; } = new();

    /// <summary>
    /// Gets the details panel view model.
    /// </summary>
    public DetailsPanelViewModel DetailsPanel { get; } = new();

    /// <summary>
    /// Gets the color map panel view model.
    /// </summary>
    public ColorMapPanelViewModel ColorMapPanel { get; } = new();

    /// <summary>
    /// Gets the statistics panel view model.
    /// </summary>
    public StatisticsPanelViewModel StatisticsPanel { get; } = new();

    /// <summary>
    /// Gets the script panel view model.
    /// </summary>
    public ScriptPanelViewModel ScriptPanel { get; } = new();

    #endregion

    #region Panel Visibility

    [ObservableProperty]
    private bool _filterPanelVisible = true;

    [ObservableProperty]
    private bool _loggerTreeVisible = true;

    [ObservableProperty]
    private bool _bookmarksPanelVisible = true;

    [ObservableProperty]
    private bool _searchPanelVisible = false;

    [ObservableProperty]
    private bool _detailsPanelVisible = true;

    [ObservableProperty]
    private bool _colorMapPanelVisible = true;

    [ObservableProperty]
    private bool _statisticsPanelVisible = false;

    [ObservableProperty]
    private bool _scriptPanelVisible = false;

    #endregion

    /// <summary>
    /// Gets or sets whether the welcome screen is visible.
    /// </summary>
    [ObservableProperty]
    private bool _isWelcomeScreenVisible = true;

    /// <summary>
    /// Gets the collection of recently opened files.
    /// </summary>
    public ObservableCollection<string> RecentFiles { get; } = new();

    /// <summary>
    /// Gets the command to create a new log document.
    /// </summary>
    public ICommand NewDocumentCommand { get; } = null!;

    /// <summary>
    /// Gets the command to open a recent file.
    /// </summary>
    public ICommand OpenRecentFileCommand { get; } = null!;

    /// <summary>
    /// Gets the command to open an existing log file.
    /// </summary>
    public ICommand OpenFileCommand { get; } = null!;

    /// <summary>
    /// Gets the command to close the active document.
    /// </summary>
    public ICommand CloseDocumentCommand { get; } = null!;

    /// <summary>
    /// Gets the command to exit the application.
    /// </summary>
    public ICommand ExitCommand { get; } = null!;

    /// <summary>
    /// Gets the command to show the About dialog.
    /// </summary>
    public ICommand ShowAboutCommand { get; } = null!;

    /// <summary>
    /// Gets the command to show the Options dialog.
    /// </summary>
    public ICommand ShowOptionsCommand { get; } = null!;

    /// <summary>
    /// Gets the command to show the Find dialog.
    /// </summary>
    public ICommand ShowFindCommand { get; } = null!;

    /// <summary>
    /// Gets the command to export log messages.
    /// </summary>
    public ICommand ExportCommand { get; } = null!;

    #region Panel Toggle Commands

    /// <summary>
    /// Gets the command to toggle the filter panel visibility.
    /// </summary>
    public ICommand ToggleFilterPanelCommand { get; } = null!;

    /// <summary>
    /// Gets the command to toggle the logger tree visibility.
    /// </summary>
    public ICommand ToggleLoggerTreeCommand { get; } = null!;

    /// <summary>
    /// Gets the command to toggle the bookmarks panel visibility.
    /// </summary>
    public ICommand ToggleBookmarksPanelCommand { get; } = null!;

    /// <summary>
    /// Gets the command to toggle the search panel visibility.
    /// </summary>
    public ICommand ToggleSearchPanelCommand { get; } = null!;

    /// <summary>
    /// Gets the command to toggle the details panel visibility.
    /// </summary>
    public ICommand ToggleDetailsPanelCommand { get; } = null!;

    /// <summary>
    /// Gets the command to toggle the color map panel visibility.
    /// </summary>
    public ICommand ToggleColorMapPanelCommand { get; } = null!;

    /// <summary>
    /// Gets the command to toggle the statistics panel visibility.
    /// </summary>
    public ICommand ToggleStatisticsPanelCommand { get; } = null!;

    /// <summary>
    /// Gets the command to toggle the script panel visibility.
    /// </summary>
    public ICommand ToggleScriptPanelCommand { get; } = null!;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
    /// </summary>
    public MainWindowViewModel()
    {
        NewDocumentCommand = new RelayCommand(OnNewDocument);
        OpenFileCommand = new RelayCommand(OnOpenFile);
        OpenRecentFileCommand = new RelayCommand<string>(OnOpenRecentFile);
        CloseDocumentCommand = new RelayCommand(OnCloseDocument, CanCloseDocument);
        ExitCommand = new RelayCommand(OnExit);
        ShowAboutCommand = new RelayCommand(OnShowAbout);
        ShowOptionsCommand = new RelayCommand(OnShowOptions);
        ShowFindCommand = new RelayCommand(OnShowFind, CanShowFind);
        ExportCommand = new RelayCommand(OnExport, CanExport);

        // Panel toggle commands
        ToggleFilterPanelCommand = new RelayCommand(() => FilterPanelVisible = !FilterPanelVisible);
        ToggleLoggerTreeCommand = new RelayCommand(() => LoggerTreeVisible = !LoggerTreeVisible);
        ToggleBookmarksPanelCommand = new RelayCommand(() => BookmarksPanelVisible = !BookmarksPanelVisible);
        ToggleSearchPanelCommand = new RelayCommand(() => SearchPanelVisible = !SearchPanelVisible);
        ToggleDetailsPanelCommand = new RelayCommand(() => DetailsPanelVisible = !DetailsPanelVisible);
        ToggleColorMapPanelCommand = new RelayCommand(() => ColorMapPanelVisible = !ColorMapPanelVisible);
        ToggleStatisticsPanelCommand = new RelayCommand(() => StatisticsPanelVisible = !StatisticsPanelVisible);
        ToggleScriptPanelCommand = new RelayCommand(() => ScriptPanelVisible = !ScriptPanelVisible);

        // Load panel visibility from settings
        LoadPanelVisibilityFromSettings();

        // Listen for document changes
        Documents.CollectionChanged += (s, e) =>
        {
            ((RelayCommand)CloseDocumentCommand).NotifyCanExecuteChanged();
            ((RelayCommand)ShowFindCommand).NotifyCanExecuteChanged();
            ((RelayCommand)ExportCommand).NotifyCanExecuteChanged();
            UpdateWelcomeScreenVisibility();
        };

        // Initialize recent files from MruManager
        RefreshRecentFiles();

        // Listen for MRU list changes
        MruManager.MruListChanged += OnMruListChanged;

        // Listen for logger tree filter changes
        LoggerTree.LoggerFilterChanged += OnLoggerFilterChanged;

        // Listen for sync to message request
        LoggerTree.SyncToMessageRequested += OnSyncToMessageRequested;
    }

    /// <summary>
    /// Handles sync to message request from the Logger Tree panel.
    /// </summary>
    private void OnSyncToMessageRequested(object? sender, EventArgs e)
    {
        if (ActiveDocument?.SelectedMessage != null)
        {
            LoggerTree.SynchronizeToMessage(ActiveDocument.SelectedMessage);
        }
    }

    /// <summary>
    /// Handles logger filter changes from the Logger Tree panel.
    /// </summary>
    private void OnLoggerFilterChanged(object? sender, LoggerFilterChangedEventArgs e)
    {
        if (ActiveDocument == null)
        {
            return;
        }

        // Apply the logger filter to the active document
        ActiveDocument.LogViewerViewModel.LoggerFilterPath = e.LoggerPath;
        ActiveDocument.LogViewerViewModel.LoggerFilterRecursive = e.IsRecursive;
    }

    private void RefreshRecentFiles()
    {
        RecentFiles.Clear();
        foreach (var file in MruManager.MruFiles)
        {
            RecentFiles.Add(file);
        }
    }

    private void OnMruListChanged(object? sender, EventArgs e)
    {
        RefreshRecentFiles();
    }

    private void UpdateWelcomeScreenVisibility()
    {
        IsWelcomeScreenVisible = Documents.Count == 0;
    }

    private void OnNewDocument()
    {
        // TODO: Show new log source dialog
        var newDoc = new LogDocumentViewModel
        {
            Title = $"Sample Log {Documents.Count + 1}"
        };

        // Generate sample log messages for testing
        var sampleMessages = SampleLogGenerator.GenerateMessages(100);
        foreach (var message in sampleMessages)
        {
            newDoc.Messages.Add(message);
        }

        Documents.Add(newDoc);
        ActiveDocument = newDoc;
    }

    private void OnOpenFile()
    {
        // TODO: Show file open dialog
    }

    private void OnOpenRecentFile(string? filePath)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            return;
        }

        // TODO: Open the file and create a document
        // For now, just add it as a sample document
        var newDoc = new LogDocumentViewModel
        {
            Title = System.IO.Path.GetFileName(filePath)
        };

        // Generate sample log messages for testing
        var sampleMessages = SampleLogGenerator.GenerateMessages(50);
        foreach (var message in sampleMessages)
        {
            newDoc.Messages.Add(message);
        }

        Documents.Add(newDoc);
        ActiveDocument = newDoc;
    }

    private bool CanCloseDocument()
    {
        return ActiveDocument != null;
    }

    private void OnCloseDocument()
    {
        if (ActiveDocument != null)
        {
            var docToClose = ActiveDocument;
            Documents.Remove(docToClose);
            ActiveDocument = Documents.Count > 0 ? Documents[^1] : null;
        }
    }

    private void OnExit()
    {
        // Close the application
        if (Avalonia.Application.Current?.ApplicationLifetime
            is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Shutdown();
        }
    }

    private void OnShowAbout()
    {
        // Will be handled by MainWindow code-behind
    }

    private void OnShowOptions()
    {
        // Will be handled by MainWindow code-behind
    }

    private bool CanShowFind()
    {
        return ActiveDocument != null;
    }

    private void OnShowFind()
    {
        // Will be handled by MainWindow code-behind
    }

    private bool CanExport()
    {
        return ActiveDocument != null && ActiveDocument.Messages.Count > 0;
    }

    private void OnExport()
    {
        // Will be handled by MainWindow code-behind
    }

    #region Panel Visibility Settings

    private void LoadPanelVisibilityFromSettings()
    {
        var settings = SettingsService.Instance.Settings;

        FilterPanelVisible = settings.FilterVisible;
        LoggerTreeVisible = settings.LoggerTreeVisible;
        BookmarksPanelVisible = settings.BookmarksVisible;
        DetailsPanelVisible = settings.DetailsVisible;
        SearchPanelVisible = settings.SearchPanelVisible;
        ColorMapPanelVisible = settings.ColorMapPanelVisible;
        StatisticsPanelVisible = settings.StatisticsPanelVisible;
        ScriptPanelVisible = settings.ScriptPanelVisible;
    }

    /// <summary>
    /// Saves the current panel visibility to settings.
    /// </summary>
    public void SavePanelVisibilityToSettings()
    {
        SettingsService.Instance.UpdateSettings(settings =>
        {
            settings.FilterVisible = FilterPanelVisible;
            settings.LoggerTreeVisible = LoggerTreeVisible;
            settings.BookmarksVisible = BookmarksPanelVisible;
            settings.DetailsVisible = DetailsPanelVisible;
            settings.SearchPanelVisible = SearchPanelVisible;
            settings.ColorMapPanelVisible = ColorMapPanelVisible;
            settings.StatisticsPanelVisible = StatisticsPanelVisible;
            settings.ScriptPanelVisible = ScriptPanelVisible;
        });

        SettingsService.Instance.Save();
    }

    #endregion

    #region Panel Synchronization

    partial void OnActiveDocumentChanged(LogDocumentViewModel? oldValue, LogDocumentViewModel? newValue)
    {
        // Update command states based on active document
        ((RelayCommand)CloseDocumentCommand).NotifyCanExecuteChanged();
        ((RelayCommand)ShowFindCommand).NotifyCanExecuteChanged();
        ((RelayCommand)ExportCommand).NotifyCanExecuteChanged();

        // Unsubscribe from old document's events
        if (oldValue != null)
        {
            oldValue.MessagesUpdated -= OnActiveDocumentMessagesUpdated;
            oldValue.PropertyChanged -= OnActiveDocumentPropertyChanged;
        }

        // Sync filter panel with active document
        if (newValue != null)
        {
            // Subscribe to new document's events
            newValue.MessagesUpdated += OnActiveDocumentMessagesUpdated;
            newValue.PropertyChanged += OnActiveDocumentPropertyChanged;

            // Use LogViewerViewModel.Messages since that's where the receiver adds messages
            ObservableCollection<LogMessage> messages = newValue.LogViewerViewModel.Messages;

            // Update logger tree with loggers from the document
            System.Collections.Generic.IEnumerable<string> loggerNames = messages.Select(m => m.Logger ?? "Unknown").Distinct();
            LoggerTree.UpdateLoggers(loggerNames);

            // Update search panel target
            SearchPanel.SetSearchTarget(newValue);

            // Update details panel with currently selected message
            DetailsPanel.SetMessage(newValue.SelectedMessage);

            // Update statistics and color map
            StatisticsPanel.UpdateStatistics(messages);
            ColorMapPanel.UpdateMessages(messages);
        }
        else
        {
            SearchPanel.SetSearchTarget(null);
            DetailsPanel.SetMessage(null);
        }
    }

    /// <summary>
    /// Handles property changes on the active document (e.g., SelectedMessage).
    /// </summary>
    private void OnActiveDocumentPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(LogDocumentViewModel.SelectedMessage) && sender is LogDocumentViewModel doc)
        {
            // Update details panel when selected message changes
            DetailsPanel.SetMessage(doc.SelectedMessage);
        }
    }

    /// <summary>
    /// Throttle timer for panel updates to avoid excessive refreshes.
    /// </summary>
    private System.Threading.Timer? _panelUpdateThrottleTimer;

    /// <summary>
    /// Tracks if a panel update is pending.
    /// </summary>
    private bool _panelUpdatePending;

    /// <summary>
    /// Handles messages being added to or removed from the active document.
    /// </summary>
    private void OnActiveDocumentMessagesUpdated(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (ActiveDocument == null)
        {
            return;
        }

        // Throttle panel updates to avoid excessive refreshes (max once per 500ms)
        if (_panelUpdateThrottleTimer == null)
        {
            _panelUpdateThrottleTimer = new System.Threading.Timer(OnPanelUpdateTimerTick, null, 500, System.Threading.Timeout.Infinite);
            _panelUpdatePending = true;
        }
        else
        {
            _panelUpdatePending = true;
        }
    }

    /// <summary>
    /// Timer callback for throttled panel updates.
    /// </summary>
    private void OnPanelUpdateTimerTick(object? state)
    {
        if (!_panelUpdatePending || ActiveDocument == null)
        {
            // Reset timer for next update
            _panelUpdateThrottleTimer?.Change(500, System.Threading.Timeout.Infinite);
            return;
        }

        _panelUpdatePending = false;

        // Marshal to UI thread
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            if (ActiveDocument == null)
            {
                return;
            }

            // Use LogViewerViewModel.Messages since that's where the receiver adds messages
            ObservableCollection<LogMessage> messages = ActiveDocument.LogViewerViewModel.Messages;

            // Update statistics with all messages
            StatisticsPanel.UpdateStatistics(messages);

            // Update logger tree with distinct loggers
            System.Collections.Generic.IEnumerable<string> loggerNames = messages.Select(m => m.Logger ?? "Unknown").Distinct();
            LoggerTree.UpdateLoggers(loggerNames);

            // Update color map
            ColorMapPanel.UpdateMessages(messages);
        });

        // Reset timer for next update
        _panelUpdateThrottleTimer?.Change(500, System.Threading.Timeout.Infinite);
    }

    /// <summary>
    /// Updates the details panel with the selected message.
    /// </summary>
    public void UpdateSelectedMessage(LogMessage? message)
    {
        DetailsPanel.SetMessage(message);
    }

    #endregion
}
