using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Couchcoding.Logbert.Logging;

namespace Couchcoding.Logbert.ViewModels.Controls;

/// <summary>
/// ViewModel for statistics visualization.
/// </summary>
public partial class StatisticsViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<LogLevelStatistic> _statistics = new();

    [ObservableProperty]
    private int _totalMessages;

    [ObservableProperty]
    private DateTime? _firstMessageTime;

    [ObservableProperty]
    private DateTime? _lastMessageTime;

    [ObservableProperty]
    private TimeSpan _timeRange;

    [ObservableProperty]
    private double _messagesPerSecond;

    public StatisticsViewModel()
    {
    }

    /// <summary>
    /// Updates statistics from a collection of log messages.
    /// </summary>
    public void UpdateStatistics(IEnumerable<LogMessage> messages)
    {
        var messageList = messages.ToList();
        TotalMessages = messageList.Count;

        if (TotalMessages == 0)
        {
            Statistics.Clear();
            FirstMessageTime = null;
            LastMessageTime = null;
            TimeRange = TimeSpan.Zero;
            MessagesPerSecond = 0;
            return;
        }

        // Calculate time range
        FirstMessageTime = messageList.Min(m => m.Timestamp);
        LastMessageTime = messageList.Max(m => m.Timestamp);
        TimeRange = LastMessageTime!.Value - FirstMessageTime!.Value;
        MessagesPerSecond = TimeRange.TotalSeconds > 0
            ? TotalMessages / TimeRange.TotalSeconds
            : 0;

        // Group by log level and count
        var levelGroups = messageList
            .GroupBy(m => m.Level)
            .OrderByDescending(g => g.Count())
            .ToList();

        Statistics.Clear();

        foreach (var group in levelGroups)
        {
            var count = group.Count();
            var percentage = (double)count / TotalMessages * 100;

            Statistics.Add(new LogLevelStatistic
            {
                Level = group.Key,
                Count = count,
                Percentage = percentage,
                Color = GetColorForLevel(group.Key)
            });
        }
    }

    private string GetColorForLevel(LogLevel level)
    {
        return level switch
        {
            LogLevel.Trace => "#808080",
            LogLevel.Debug => "#0000FF",
            LogLevel.Info => "#008000",
            LogLevel.Warning => "#FFA500",
            LogLevel.Error => "#FF0000",
            LogLevel.Fatal => "#8B0000",
            _ => "#000000"
        };
    }
}

/// <summary>
/// Represents statistics for a specific log level.
/// </summary>
public partial class LogLevelStatistic : ObservableObject
{
    [ObservableProperty]
    private LogLevel _level;

    [ObservableProperty]
    private int _count;

    [ObservableProperty]
    private double _percentage;

    [ObservableProperty]
    private string _color = "#000000";
}
