using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Logbert.Logging;

namespace Logbert.ViewModels.Controls;

/// <summary>
/// Represents a group of log messages for column grouping functionality.
/// </summary>
public partial class LogMessageGroup : ObservableObject
{
    /// <summary>
    /// Gets or sets the group key (the value being grouped by).
    /// </summary>
    [ObservableProperty]
    private string _groupKey = string.Empty;

    /// <summary>
    /// Gets or sets the column name used for grouping.
    /// </summary>
    [ObservableProperty]
    private string _columnName = string.Empty;

    /// <summary>
    /// Gets or sets whether the group is expanded.
    /// </summary>
    [ObservableProperty]
    private bool _isExpanded = true;

    /// <summary>
    /// Gets the messages in this group.
    /// </summary>
    public ObservableCollection<LogMessage> Messages { get; } = new();

    /// <summary>
    /// Gets the count of messages in this group.
    /// </summary>
    public int Count => Messages.Count;

    /// <summary>
    /// Gets the display text for the group header.
    /// </summary>
    public string DisplayText => $"{ColumnName}: {GroupKey} ({Count} items)";

    /// <summary>
    /// Initializes a new instance of the <see cref="LogMessageGroup"/> class.
    /// </summary>
    public LogMessageGroup()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LogMessageGroup"/> class.
    /// </summary>
    /// <param name="columnName">The column name used for grouping.</param>
    /// <param name="groupKey">The group key value.</param>
    /// <param name="messages">The messages in this group.</param>
    public LogMessageGroup(string columnName, string groupKey, IEnumerable<LogMessage> messages)
    {
        ColumnName = columnName;
        GroupKey = groupKey;

        foreach (var msg in messages)
        {
            Messages.Add(msg);
        }
    }
}

/// <summary>
/// Represents a column that can be used for grouping.
/// </summary>
public class GroupableColumn
{
    /// <summary>
    /// Gets or sets the column name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the property name to group by.
    /// </summary>
    public string PropertyName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether this column is currently used for grouping.
    /// </summary>
    public bool IsGrouped { get; set; }

    public override string ToString() => Name;
}

/// <summary>
/// Represents a column configuration for visibility and display.
/// </summary>
public partial class ColumnConfig : ObservableObject
{
    /// <summary>
    /// Gets or sets the column name (used as key).
    /// </summary>
    [ObservableProperty]
    private string _name = string.Empty;

    /// <summary>
    /// Gets or sets the display header text.
    /// </summary>
    [ObservableProperty]
    private string _header = string.Empty;

    /// <summary>
    /// Gets or sets whether the column is visible.
    /// </summary>
    [ObservableProperty]
    private bool _isVisible = true;

    /// <summary>
    /// Gets or sets the column width.
    /// </summary>
    [ObservableProperty]
    private double _width = 100;

    /// <summary>
    /// Gets or sets the display order.
    /// </summary>
    [ObservableProperty]
    private int _displayOrder;

    /// <summary>
    /// Gets or sets the binding property name.
    /// </summary>
    public string BindingProperty { get; set; } = string.Empty;
}
