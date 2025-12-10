using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Logbert.Logging;
using Logbert.ViewModels.Controls;

namespace Logbert.ViewModels.Docking;

/// <summary>
/// ViewModel for the dockable statistics panel.
/// </summary>
public partial class StatisticsPanelViewModel : ViewModelBase
{
    private IEnumerable<LogMessage>? _currentMessages;

    [ObservableProperty]
    private StatisticsViewModel _statisticsViewModel = new();

    [ObservableProperty]
    private ObservableCollection<KeyValuePair<string, int>> _topLoggers = new();

    /// <summary>
    /// Gets the formatted total messages count.
    /// </summary>
    public string TotalMessagesFormatted => $"{StatisticsViewModel.TotalMessages:N0}";

    /// <summary>
    /// Gets the formatted first message time.
    /// </summary>
    public string FirstMessageTimeFormatted => StatisticsViewModel.FirstMessageTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? "N/A";

    /// <summary>
    /// Gets the formatted last message time.
    /// </summary>
    public string LastMessageTimeFormatted => StatisticsViewModel.LastMessageTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? "N/A";

    /// <summary>
    /// Gets the formatted time range.
    /// </summary>
    public string TimeRangeFormatted
    {
        get
        {
            var range = StatisticsViewModel.TimeRange;
            if (range == TimeSpan.Zero)
                return "N/A";

            if (range.TotalDays >= 1)
                return $"{range.Days}d {range.Hours}h {range.Minutes}m";
            if (range.TotalHours >= 1)
                return $"{range.Hours}h {range.Minutes}m {range.Seconds}s";
            if (range.TotalMinutes >= 1)
                return $"{range.Minutes}m {range.Seconds}s";
            return $"{range.Seconds}.{range.Milliseconds}s";
        }
    }

    /// <summary>
    /// Gets the formatted messages per second.
    /// </summary>
    public string MessagesPerSecondFormatted => $"{StatisticsViewModel.MessagesPerSecond:F2}";

    /// <summary>
    /// Gets whether logger statistics are available.
    /// </summary>
    public bool HasLoggerStatistics => TopLoggers.Count > 0;

    public IRelayCommand RefreshCommand { get; }

    public StatisticsPanelViewModel()
    {
        RefreshCommand = new RelayCommand(OnRefresh);

        StatisticsViewModel.PropertyChanged += (s, e) =>
        {
            OnPropertyChanged(nameof(TotalMessagesFormatted));
            OnPropertyChanged(nameof(FirstMessageTimeFormatted));
            OnPropertyChanged(nameof(LastMessageTimeFormatted));
            OnPropertyChanged(nameof(TimeRangeFormatted));
            OnPropertyChanged(nameof(MessagesPerSecondFormatted));
        };
    }

    private void OnRefresh()
    {
        if (_currentMessages != null)
        {
            UpdateStatistics(_currentMessages);
        }
    }

    /// <summary>
    /// Updates statistics from a collection of log messages.
    /// </summary>
    public void UpdateStatistics(IEnumerable<LogMessage> messages)
    {
        _currentMessages = messages;
        var messageList = messages.ToList();

        // Update main statistics
        StatisticsViewModel.UpdateStatistics(messageList);

        // Calculate top loggers
        TopLoggers.Clear();
        var loggerGroups = messageList
            .Where(m => !string.IsNullOrEmpty(m.Logger))
            .GroupBy(m => m.Logger)
            .OrderByDescending(g => g.Count())
            .Take(10)
            .ToList();

        foreach (var group in loggerGroups)
        {
            TopLoggers.Add(new KeyValuePair<string, int>(group.Key ?? "Unknown", group.Count()));
        }

        OnPropertyChanged(nameof(HasLoggerStatistics));
    }
}
