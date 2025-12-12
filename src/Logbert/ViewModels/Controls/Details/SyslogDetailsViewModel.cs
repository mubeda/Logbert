using System;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using Logbert.Logging;

namespace Logbert.ViewModels.Controls.Details;

/// <summary>
/// ViewModel for the Syslog details view.
/// </summary>
public partial class SyslogDetailsViewModel : ObservableObject
{
    [ObservableProperty]
    private LogMessageSyslog? _message;

    /// <summary>
    /// Gets the severity level.
    /// </summary>
    public string Severity => Message?.Level.ToString() ?? string.Empty;

    /// <summary>
    /// Gets the facility.
    /// </summary>
    public string Facility => Message?.LogFacility.ToString() ?? string.Empty;

    /// <summary>
    /// Gets the formatted timestamp.
    /// </summary>
    public string FormattedTimestamp => Message?.Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff") ?? string.Empty;

    /// <summary>
    /// Gets the formatted local timestamp.
    /// </summary>
    public string FormattedLocalTimestamp => Message?.LocalTimestamp.ToString("yyyy-MM-dd HH:mm:ss.fff") ?? string.Empty;

    /// <summary>
    /// Gets the index display string.
    /// </summary>
    public string IndexDisplay => Message != null ? $"Message #{Message.Index}" : string.Empty;

    /// <summary>
    /// Gets the logger/host name.
    /// </summary>
    public string Logger => Message?.Logger ?? string.Empty;

    /// <summary>
    /// Gets the log message text.
    /// </summary>
    public string MessageText => Message?.Message ?? string.Empty;

    /// <summary>
    /// Gets the raw syslog data.
    /// </summary>
    public string RawData => Message?.RawData?.ToString() ?? string.Empty;

    /// <summary>
    /// Gets a description for the current facility.
    /// </summary>
    public string FacilityDescription
    {
        get
        {
            if (Message == null) return string.Empty;

            return Message.LogFacility switch
            {
                LogMessageSyslog.Facility.Kernel => "(Kernel messages)",
                LogMessageSyslog.Facility.UserLevel => "(User-level messages)",
                LogMessageSyslog.Facility.MailSystem => "(Mail system)",
                LogMessageSyslog.Facility.SystemDaemons => "(System daemons)",
                LogMessageSyslog.Facility.Security => "(Security/Authorization)",
                LogMessageSyslog.Facility.Internal => "(Syslogd internal)",
                LogMessageSyslog.Facility.LinePrinter => "(Line printer)",
                LogMessageSyslog.Facility.NetworkNews => "(Network news)",
                LogMessageSyslog.Facility.UUCP => "(UUCP subsystem)",
                LogMessageSyslog.Facility.ClockDaemeon => "(Clock daemon)",
                LogMessageSyslog.Facility.Security2 => "(Security/Authorization)",
                LogMessageSyslog.Facility.FTPDaemon => "(FTP daemon)",
                LogMessageSyslog.Facility.NTP => "(NTP subsystem)",
                LogMessageSyslog.Facility.LogAudit => "(Log audit)",
                LogMessageSyslog.Facility.LogAlert => "(Log alert)",
                LogMessageSyslog.Facility.Clock => "(Clock daemon)",
                LogMessageSyslog.Facility.Local0 => "(Local use 0)",
                LogMessageSyslog.Facility.Local1 => "(Local use 1)",
                LogMessageSyslog.Facility.Local2 => "(Local use 2)",
                LogMessageSyslog.Facility.Local3 => "(Local use 3)",
                LogMessageSyslog.Facility.Local4 => "(Local use 4)",
                LogMessageSyslog.Facility.Local5 => "(Local use 5)",
                LogMessageSyslog.Facility.Local6 => "(Local use 6)",
                LogMessageSyslog.Facility.Local7 => "(Local use 7)",
                _ => string.Empty
            };
        }
    }

    /// <summary>
    /// Gets a description for the current severity.
    /// </summary>
    public string SeverityDescription
    {
        get
        {
            if (Message == null) return string.Empty;

            return Message.Level switch
            {
                LogLevel.Trace => "(Notice/Debug)",
                LogLevel.Debug => "(Debug-level messages)",
                LogLevel.Info => "(Informational)",
                LogLevel.Warning => "(Warning conditions)",
                LogLevel.Error => "(Error/Alert conditions)",
                LogLevel.Fatal => "(Critical/Emergency)",
                _ => string.Empty
            };
        }
    }

    /// <summary>
    /// Gets the background color for the severity badge.
    /// </summary>
    public IBrush SeverityBackground
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
    /// Gets the background color for the facility badge.
    /// </summary>
    public IBrush FacilityBackground
    {
        get
        {
            if (Message == null) return Brushes.Gray;

            // Color code by facility category
            return Message.LogFacility switch
            {
                LogMessageSyslog.Facility.Kernel => Brushes.DarkSlateBlue,
                LogMessageSyslog.Facility.UserLevel => Brushes.SteelBlue,
                LogMessageSyslog.Facility.MailSystem => Brushes.Teal,
                LogMessageSyslog.Facility.SystemDaemons => Brushes.DarkCyan,
                LogMessageSyslog.Facility.Security or LogMessageSyslog.Facility.Security2 => Brushes.Maroon,
                LogMessageSyslog.Facility.Internal => Brushes.SlateGray,
                LogMessageSyslog.Facility.LinePrinter => Brushes.DarkOliveGreen,
                LogMessageSyslog.Facility.NetworkNews => Brushes.DarkGoldenrod,
                LogMessageSyslog.Facility.UUCP => Brushes.SaddleBrown,
                LogMessageSyslog.Facility.ClockDaemeon or LogMessageSyslog.Facility.Clock => Brushes.Purple,
                LogMessageSyslog.Facility.FTPDaemon => Brushes.DarkGreen,
                LogMessageSyslog.Facility.NTP => Brushes.Indigo,
                LogMessageSyslog.Facility.LogAudit or LogMessageSyslog.Facility.LogAlert => Brushes.DarkRed,
                _ => Brushes.CadetBlue // Local facilities
            };
        }
    }

    /// <summary>
    /// Updates the view model with a new log message.
    /// </summary>
    /// <param name="message">The Syslog message to display.</param>
    public void SetMessage(LogMessageSyslog message)
    {
        Message = message;
        OnPropertyChanged(nameof(Severity));
        OnPropertyChanged(nameof(Facility));
        OnPropertyChanged(nameof(FormattedTimestamp));
        OnPropertyChanged(nameof(FormattedLocalTimestamp));
        OnPropertyChanged(nameof(IndexDisplay));
        OnPropertyChanged(nameof(Logger));
        OnPropertyChanged(nameof(MessageText));
        OnPropertyChanged(nameof(RawData));
        OnPropertyChanged(nameof(FacilityDescription));
        OnPropertyChanged(nameof(SeverityDescription));
        OnPropertyChanged(nameof(SeverityBackground));
        OnPropertyChanged(nameof(FacilityBackground));
    }
}
