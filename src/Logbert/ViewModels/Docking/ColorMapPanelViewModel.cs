using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using Logbert.Logging;
using Logbert.ViewModels.Controls;

namespace Logbert.ViewModels.Docking;

/// <summary>
/// ViewModel for the dockable color map panel.
/// </summary>
public partial class ColorMapPanelViewModel : ViewModelBase
{
    [ObservableProperty]
    private ColorMapViewModel _colorMapViewModel = new();

    /// <summary>
    /// Gets the total messages text.
    /// </summary>
    public string TotalMessagesText => $"Total: {ColorMapViewModel.TotalMessages:N0}";

    /// <summary>
    /// Gets the visible range text.
    /// </summary>
    public string VisibleRangeText =>
        $"Visible: {ColorMapViewModel.VisibleStart:N0} - {ColorMapViewModel.VisibleEnd:N0}";

    /// <summary>
    /// Event raised when a position is clicked in the color map.
    /// </summary>
    public event EventHandler<int>? NavigateToIndex;

    public ColorMapPanelViewModel()
    {
        ColorMapViewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(ColorMapViewModel.TotalMessages))
            {
                OnPropertyChanged(nameof(TotalMessagesText));
            }
            if (e.PropertyName == nameof(ColorMapViewModel.VisibleStart) ||
                e.PropertyName == nameof(ColorMapViewModel.VisibleEnd))
            {
                OnPropertyChanged(nameof(VisibleRangeText));
            }
        };
    }

    /// <summary>
    /// Updates the color map with log messages.
    /// </summary>
    public void UpdateMessages(IEnumerable<LogMessage> messages)
    {
        ColorMapViewModel.UpdateMessages(messages);
    }

    /// <summary>
    /// Updates the visible range of messages.
    /// </summary>
    public void UpdateVisibleRange(int start, int end)
    {
        ColorMapViewModel.UpdateVisibleRange(start, end);
    }

    /// <summary>
    /// Navigates to a specific index in the log viewer.
    /// </summary>
    public void RequestNavigateToIndex(int index)
    {
        NavigateToIndex?.Invoke(this, index);
    }
}
