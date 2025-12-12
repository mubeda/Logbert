using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using Logbert.Logging;

namespace Logbert.ViewModels.Controls.Details;

/// <summary>
/// ViewModel for the Custom log details view.
/// </summary>
public partial class CustomDetailsViewModel : ObservableObject
{
    [ObservableProperty]
    private LogMessageCustom? _message;

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
    /// <param name="message">The Custom message to display.</param>
    public void SetMessage(LogMessageCustom message)
    {
        Message = message;
        OnPropertyChanged(nameof(Level));
        OnPropertyChanged(nameof(FormattedTimestamp));
        OnPropertyChanged(nameof(IndexDisplay));
        OnPropertyChanged(nameof(Logger));
        OnPropertyChanged(nameof(MessageText));
        OnPropertyChanged(nameof(RawData));
        OnPropertyChanged(nameof(LevelBackground));
    }
}
