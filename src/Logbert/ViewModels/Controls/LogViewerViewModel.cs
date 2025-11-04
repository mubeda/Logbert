using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Couchcoding.Logbert.Logging;

namespace Couchcoding.Logbert.ViewModels.Controls;

/// <summary>
/// ViewModel for the log viewer control.
/// </summary>
public partial class LogViewerViewModel : ViewModelBase
{
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
    /// Gets the filtered messages based on log level visibility.
    /// </summary>
    public ObservableCollection<LogMessage> FilteredMessages { get; } = new();

    /// <summary>
    /// Gets the command to zoom in.
    /// </summary>
    public IRelayCommand ZoomInCommand { get; }

    /// <summary>
    /// Gets the command to zoom out.
    /// </summary>
    public IRelayCommand ZoomOutCommand { get; }

    /// <summary>
    /// Gets the command to copy selected message.
    /// </summary>
    public IRelayCommand CopyMessageCommand { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LogViewerViewModel"/> class.
    /// </summary>
    public LogViewerViewModel()
    {
        ZoomInCommand = new RelayCommand(OnZoomIn, CanZoomIn);
        ZoomOutCommand = new RelayCommand(OnZoomOut, CanZoomOut);
        CopyMessageCommand = new RelayCommand(OnCopyMessage, CanCopyMessage);

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

        foreach (var message in Messages)
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
}
