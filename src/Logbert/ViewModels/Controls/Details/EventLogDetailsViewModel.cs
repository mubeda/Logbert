using System;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using Logbert.Logging;

namespace Logbert.ViewModels.Controls.Details;

/// <summary>
/// ViewModel for the Windows Event Log details view.
/// </summary>
public partial class EventLogDetailsViewModel : ObservableObject
{
    [ObservableProperty]
    private LogMessageEventlog? _message;

    /// <summary>
    /// Gets the log level.
    /// </summary>
    public string Level => Message?.Level.ToString() ?? string.Empty;

    /// <summary>
    /// Gets the event source.
    /// </summary>
    public string Source => Message?.Logger ?? string.Empty;

    /// <summary>
    /// Gets the formatted timestamp.
    /// </summary>
    public string FormattedTimestamp => Message?.Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff") ?? string.Empty;

    /// <summary>
    /// Gets the index display string.
    /// </summary>
    public string IndexDisplay => Message != null ? $"Message #{Message.Index}" : string.Empty;

    /// <summary>
    /// Gets the event instance ID.
    /// </summary>
    public string InstanceId => Message?.InstanceId.ToString() ?? string.Empty;

    /// <summary>
    /// Gets the event category.
    /// </summary>
    public string Category => Message?.Category ?? string.Empty;

    /// <summary>
    /// Gets the username.
    /// </summary>
    public string Username => Message?.Username ?? string.Empty;

    /// <summary>
    /// Gets the log message text.
    /// </summary>
    public string MessageText => Message?.Message ?? string.Empty;

    /// <summary>
    /// Gets whether binary data is available.
    /// </summary>
    public bool HasBinaryData => Message?.Data != null && Message.Data.Length > 0;

    /// <summary>
    /// Gets the binary data size description.
    /// </summary>
    public string BinaryDataSize
    {
        get
        {
            if (Message?.Data == null || Message.Data.Length == 0)
                return "No binary data";
            return $"{Message.Data.Length} bytes";
        }
    }

    /// <summary>
    /// Gets the binary data as hex string.
    /// </summary>
    public string BinaryDataHex
    {
        get
        {
            if (Message?.Data == null || Message.Data.Length == 0)
                return string.Empty;

            return BitConverter.ToString(Message.Data).Replace("-", " ");
        }
    }

    /// <summary>
    /// Gets the raw event data.
    /// </summary>
    public string RawData => Message?.ToString() ?? string.Empty;

    /// <summary>
    /// Gets a description for the current log level.
    /// </summary>
    public string LevelDescription
    {
        get
        {
            if (Message == null) return string.Empty;

            return Message.Level switch
            {
                LogLevel.Info => "Information event",
                LogLevel.Warning => "Warning event",
                LogLevel.Error => "Error or Audit Failure event",
                _ => string.Empty
            };
        }
    }

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
    /// <param name="message">The EventLog message to display.</param>
    public void SetMessage(LogMessageEventlog message)
    {
        Message = message;
        OnPropertyChanged(nameof(Level));
        OnPropertyChanged(nameof(Source));
        OnPropertyChanged(nameof(FormattedTimestamp));
        OnPropertyChanged(nameof(IndexDisplay));
        OnPropertyChanged(nameof(InstanceId));
        OnPropertyChanged(nameof(Category));
        OnPropertyChanged(nameof(Username));
        OnPropertyChanged(nameof(MessageText));
        OnPropertyChanged(nameof(HasBinaryData));
        OnPropertyChanged(nameof(BinaryDataSize));
        OnPropertyChanged(nameof(BinaryDataHex));
        OnPropertyChanged(nameof(RawData));
        OnPropertyChanged(nameof(LevelDescription));
        OnPropertyChanged(nameof(LevelBackground));
    }
}
