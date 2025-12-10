using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using Logbert.Logging;

namespace Logbert.ViewModels.Controls.Details;

/// <summary>
/// ViewModel for the Log4Net details view.
/// </summary>
public partial class Log4NetDetailsViewModel : ObservableObject
{
    [ObservableProperty]
    private LogMessageLog4Net? _message;

    /// <summary>
    /// Gets the log level.
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
    /// Gets the logger name.
    /// </summary>
    public string Logger => Message?.Logger ?? string.Empty;

    /// <summary>
    /// Gets the thread name.
    /// </summary>
    public string Thread => Message?.Thread ?? string.Empty;

    /// <summary>
    /// Gets the file name from location info.
    /// </summary>
    public string FileName => Message?.Location.FileName ?? string.Empty;

    /// <summary>
    /// Gets the class name from location info.
    /// </summary>
    public string ClassName => Message?.Location.ClassName ?? string.Empty;

    /// <summary>
    /// Gets the method name from location info.
    /// </summary>
    public string MethodName => Message?.Location.MethodName ?? string.Empty;

    /// <summary>
    /// Gets whether location info is available.
    /// </summary>
    public bool HasLocation => !string.IsNullOrEmpty(FileName) ||
                                !string.IsNullOrEmpty(ClassName) ||
                                !string.IsNullOrEmpty(MethodName);

    /// <summary>
    /// Gets the log message text.
    /// </summary>
    public string MessageText => Message?.Message ?? string.Empty;

    /// <summary>
    /// Gets whether custom properties are available.
    /// </summary>
    public bool HasCustomProperties => Message?.CustomProperties?.Count > 0;

    /// <summary>
    /// Gets the custom properties.
    /// </summary>
    public IEnumerable<KeyValuePair<string, string>> CustomProperties =>
        Message?.CustomProperties ?? Enumerable.Empty<KeyValuePair<string, string>>();

    /// <summary>
    /// Gets the raw XML data.
    /// </summary>
    public string RawData => Message?.RawData?.ToString() ?? string.Empty;

    /// <summary>
    /// Gets the background color for the log level badge.
    /// </summary>
    public IBrush LevelBackground
    {
        get
        {
            if (Message == null) return Brushes.Gray;

            return Message.Level switch
            {
                LogLevel.Trace => Brushes.LightGray,
                LogLevel.Debug => Brushes.DodgerBlue,
                LogLevel.Info => Brushes.Green,
                LogLevel.Warning => Brushes.Orange,
                LogLevel.Error => Brushes.Red,
                LogLevel.Fatal => Brushes.DarkRed,
                _ => Brushes.Gray
            };
        }
    }

    /// <summary>
    /// Updates the view model with a new log message.
    /// </summary>
    /// <param name="message">The Log4Net message to display.</param>
    public void SetMessage(LogMessageLog4Net message)
    {
        Message = message;
        OnPropertyChanged(nameof(Level));
        OnPropertyChanged(nameof(FormattedTimestamp));
        OnPropertyChanged(nameof(IndexDisplay));
        OnPropertyChanged(nameof(Logger));
        OnPropertyChanged(nameof(Thread));
        OnPropertyChanged(nameof(FileName));
        OnPropertyChanged(nameof(ClassName));
        OnPropertyChanged(nameof(MethodName));
        OnPropertyChanged(nameof(HasLocation));
        OnPropertyChanged(nameof(MessageText));
        OnPropertyChanged(nameof(HasCustomProperties));
        OnPropertyChanged(nameof(CustomProperties));
        OnPropertyChanged(nameof(RawData));
        OnPropertyChanged(nameof(LevelBackground));
    }
}
