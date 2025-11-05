using System.Collections.ObjectModel;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using Couchcoding.Logbert.Logging;

namespace Couchcoding.Logbert.ViewModels.Controls;

/// <summary>
/// ViewModel for the ColorMap control.
/// </summary>
public partial class ColorMapViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<ColorMapItem> _items = new();

    [ObservableProperty]
    private int _totalMessages;

    [ObservableProperty]
    private int _visibleStart;

    [ObservableProperty]
    private int _visibleEnd;

    public ColorMapViewModel()
    {
    }

    /// <summary>
    /// Updates the color map with log messages.
    /// </summary>
    public void UpdateMessages(IEnumerable<LogMessage> messages)
    {
        Items.Clear();

        var messageList = messages.ToList();
        TotalMessages = messageList.Count;

        if (TotalMessages == 0)
            return;

        // Create color map items for each message
        for (int i = 0; i < messageList.Count; i++)
        {
            var message = messageList[i];
            Items.Add(new ColorMapItem
            {
                Index = i,
                Position = (double)i / TotalMessages,
                Color = GetColorForLevel(message.Level),
                Message = message
            });
        }
    }

    /// <summary>
    /// Updates the visible range of messages.
    /// </summary>
    public void UpdateVisibleRange(int start, int end)
    {
        VisibleStart = start;
        VisibleEnd = end;
    }

    private Color GetColorForLevel(LogLevel level)
    {
        return level switch
        {
            LogLevel.Trace => Color.FromRgb(128, 128, 128),
            LogLevel.Debug => Color.FromRgb(0, 0, 255),
            LogLevel.Info => Color.FromRgb(0, 128, 0),
            LogLevel.Warning => Color.FromRgb(255, 165, 0),
            LogLevel.Error => Color.FromRgb(255, 0, 0),
            LogLevel.Fatal => Color.FromRgb(139, 0, 0),
            _ => Colors.Black
        };
    }
}

/// <summary>
/// Represents an item in the color map.
/// </summary>
public partial class ColorMapItem : ObservableObject
{
    [ObservableProperty]
    private int _index;

    [ObservableProperty]
    private double _position; // 0.0 to 1.0

    [ObservableProperty]
    private Color _color;

    [ObservableProperty]
    private LogMessage? _message;
}
