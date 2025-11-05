using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Couchcoding.Logbert.Logging;
using Couchcoding.Logbert.Logging.Sample;
using Couchcoding.Logbert.ViewModels.Docking;

namespace Couchcoding.Logbert.ViewModels;

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
    /// Gets or sets whether the welcome screen is visible.
    /// </summary>
    [ObservableProperty]
    private bool _isWelcomeScreenVisible = true;

    /// <summary>
    /// Gets the command to create a new log document.
    /// </summary>
    public ICommand NewDocumentCommand { get; } = null!;

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
    /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
    /// </summary>
    public MainWindowViewModel()
    {
        NewDocumentCommand = new RelayCommand(OnNewDocument);
        OpenFileCommand = new RelayCommand(OnOpenFile);
        CloseDocumentCommand = new RelayCommand(OnCloseDocument, CanCloseDocument);
        ExitCommand = new RelayCommand(OnExit);
        ShowAboutCommand = new RelayCommand(OnShowAbout);
        ShowOptionsCommand = new RelayCommand(OnShowOptions);
        ShowFindCommand = new RelayCommand(OnShowFind, CanShowFind);

        // Listen for document changes
        Documents.CollectionChanged += (s, e) =>
        {
            ((RelayCommand)CloseDocumentCommand).NotifyCanExecuteChanged();
            ((RelayCommand)ShowFindCommand).NotifyCanExecuteChanged();
            UpdateWelcomeScreenVisibility();
        };
    }

    partial void OnActiveDocumentChanged(LogDocumentViewModel? value)
    {
        // Sync filter panel with active document
        if (value != null)
        {
            // Update logger tree with loggers from the document
            var loggerNames = value.Messages.Select(m => m.Logger ?? "Unknown").Distinct();
            LoggerTree.UpdateLoggers(loggerNames);
        }
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
}
