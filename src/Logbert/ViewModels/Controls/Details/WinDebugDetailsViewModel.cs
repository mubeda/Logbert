using System;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using Logbert.Logging;

namespace Logbert.ViewModels.Controls.Details;

/// <summary>
/// ViewModel for the Windows Debug details view.
/// </summary>
public partial class WinDebugDetailsViewModel : ObservableObject
{
    [ObservableProperty]
    private LogMessageWinDebug? _message;

    /// <summary>
    /// Gets the log level (always Debug for WinDebug messages).
    /// </summary>
    public string Level => Message?.Level.ToString() ?? string.Empty;

    /// <summary>
    /// Gets the formatted timestamp.
    /// </summary>
    public string FormattedTimestamp => Message?.Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff") ?? string.Empty;

    /// <summary>
    /// Gets the index display string.
    /// </summary>
    public string IndexDisplay => Message != null ? $"Message #{Message.Index}" : string.Empty;

    /// <summary>
    /// Gets the process ID.
    /// </summary>
    public string ProcessId => Message?.ProcessId.ToString() ?? string.Empty;

    /// <summary>
    /// Gets the log message text.
    /// </summary>
    public string MessageText => Message?.Message ?? string.Empty;

    /// <summary>
    /// Gets the raw data.
    /// </summary>
    public string RawData => Message?.RawData?.ToString() ?? string.Empty;

    /// <summary>
    /// Gets the background color for the log level badge.
    /// </summary>
    public IBrush LevelBackground => Brushes.DodgerBlue; // Always Debug level

    /// <summary>
    /// Updates the view model with a new log message.
    /// </summary>
    /// <param name="message">The Windows Debug message to display.</param>
    public void SetMessage(LogMessageWinDebug message)
    {
        Message = message;
        OnPropertyChanged(nameof(Level));
        OnPropertyChanged(nameof(FormattedTimestamp));
        OnPropertyChanged(nameof(IndexDisplay));
        OnPropertyChanged(nameof(ProcessId));
        OnPropertyChanged(nameof(MessageText));
        OnPropertyChanged(nameof(RawData));
        OnPropertyChanged(nameof(LevelBackground));
    }
}
