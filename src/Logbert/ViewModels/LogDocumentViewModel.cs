using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Couchcoding.Logbert.Logging;
using Couchcoding.Logbert.ViewModels.Controls;

namespace Couchcoding.Logbert.ViewModels;

/// <summary>
/// ViewModel for a single log document/tab.
/// </summary>
public partial class LogDocumentViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _title = "Untitled";

    [ObservableProperty]
    private bool _isActive;

    /// <summary>
    /// Gets the collection of log messages in this document.
    /// </summary>
    public ObservableCollection<LogMessage> Messages { get; } = new();

    /// <summary>
    /// Gets or sets the selected log message.
    /// </summary>
    [ObservableProperty]
    private LogMessage? _selectedMessage;

    /// <summary>
    /// Gets or sets the filter text.
    /// </summary>
    [ObservableProperty]
    private string _filterText = string.Empty;

    /// <summary>
    /// Gets or sets whether trace level messages are visible.
    /// </summary>
    [ObservableProperty]
    private bool _showTrace = true;

    /// <summary>
    /// Gets or sets whether debug level messages are visible.
    /// </summary>
    [ObservableProperty]
    private bool _showDebug = true;

    /// <summary>
    /// Gets or sets whether info level messages are visible.
    /// </summary>
    [ObservableProperty]
    private bool _showInfo = true;

    /// <summary>
    /// Gets or sets whether warning level messages are visible.
    /// </summary>
    [ObservableProperty]
    private bool _showWarning = true;

    /// <summary>
    /// Gets or sets whether error level messages are visible.
    /// </summary>
    [ObservableProperty]
    private bool _showError = true;

    /// <summary>
    /// Gets or sets whether fatal level messages are visible.
    /// </summary>
    [ObservableProperty]
    private bool _showFatal = true;

    /// <summary>
    /// Gets the log viewer view model.
    /// </summary>
    public LogViewerViewModel LogViewerViewModel { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LogDocumentViewModel"/> class.
    /// </summary>
    public LogDocumentViewModel()
    {
        LogViewerViewModel = new LogViewerViewModel();

        // Sync messages to viewer
        Messages.CollectionChanged += (s, e) =>
        {
            LogViewerViewModel.UpdateMessages(Messages);
        };

        // Sync selected message
        LogViewerViewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(LogViewerViewModel.SelectedMessage))
            {
                SelectedMessage = LogViewerViewModel.SelectedMessage;
            }
        };

        PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(SelectedMessage))
            {
                LogViewerViewModel.SelectedMessage = SelectedMessage;
            }
        };
    }
}
