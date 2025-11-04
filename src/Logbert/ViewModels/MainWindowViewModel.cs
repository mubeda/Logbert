using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Couchcoding.Logbert.Logging;
using Couchcoding.Logbert.Logging.Sample;

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
    public LogDocumentViewModel? ActiveDocument { get; set; }

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
            Documents.Remove(ActiveDocument);
            ActiveDocument = Documents.Count > 0 ? Documents[^1] : null;
        }
    }

    private void OnExit()
    {
        // TODO: Cleanup and exit
    }
}
