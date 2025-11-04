using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Couchcoding.Logbert.Docking;
using Couchcoding.Logbert.Logging;
using Couchcoding.Logbert.Logging.Sample;
using Dock.Model.Core;

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
    /// Gets the dock factory for managing docking layout.
    /// </summary>
    public DockFactory DockFactory { get; }

    /// <summary>
    /// Gets the dock layout manager for persistence.
    /// </summary>
    public DockLayoutManager LayoutManager { get; }

    /// <summary>
    /// Gets the root dock layout.
    /// </summary>
    [ObservableProperty]
    private IDock? _layout;

    /// <summary>
    /// Gets the command to create a new log document.
    /// </summary>
    public ICommand NewDocumentCommand { get; }

    /// <summary>
    /// Gets the command to open an existing log file.
    /// </summary>
    public ICommand OpenFileCommand { get; }

    /// <summary>
    /// Gets the command to close the active document.
    /// </summary>
    public ICommand CloseDocumentCommand { get; }

    /// <summary>
    /// Gets the command to exit the application.
    /// </summary>
    public ICommand ExitCommand { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
    /// </summary>
    public MainWindowViewModel()
    {
        NewDocumentCommand = new RelayCommand(OnNewDocument);
        OpenFileCommand = new RelayCommand(OnOpenFile);
        CloseDocumentCommand = new RelayCommand(OnCloseDocument, CanCloseDocument);
        ExitCommand = new RelayCommand(OnExit);

        // Initialize docking
        DockFactory = new DockFactory(this);
        LayoutManager = new DockLayoutManager();

        // Try to load saved layout, otherwise create default
        Layout = LayoutManager.LoadLayout(DockFactory);
        if (Layout == null)
        {
            Layout = DockFactory.CreateLayout();
        }
        DockFactory.InitLayout(Layout);

        // Listen for document changes
        Documents.CollectionChanged += (s, e) =>
        {
            ((RelayCommand)CloseDocumentCommand).NotifyCanExecuteChanged();
        };
    }

    partial void OnActiveDocumentChanged(LogDocumentViewModel? value)
    {
        DockFactory.SetActiveDocument(value);
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
        DockFactory.AddDocument(newDoc);
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
            DockFactory.RemoveDocument(docToClose);
            ActiveDocument = Documents.Count > 0 ? Documents[^1] : null;
        }
    }

    private void OnExit()
    {
        // Save layout before exit
        LayoutManager.SaveLayout(Layout);

        // Close the application
        if (Avalonia.Application.Current?.ApplicationLifetime
            is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Shutdown();
        }
    }
}
