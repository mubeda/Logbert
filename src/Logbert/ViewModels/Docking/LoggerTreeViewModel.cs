using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Dock.Model.Mvvm.Controls;

namespace Couchcoding.Logbert.ViewModels.Docking;

/// <summary>
/// ViewModel for the logger tree tool window.
/// </summary>
public partial class LoggerTreeViewModel : Tool
{
    [ObservableProperty]
    private ObservableCollection<LoggerTreeNode> _loggers = new();

    [ObservableProperty]
    private LoggerTreeNode? _selectedLogger;

    public LoggerTreeViewModel()
    {
    }

    /// <summary>
    /// Updates the logger tree from log messages.
    /// </summary>
    public void UpdateLoggers(IEnumerable<string> loggerNames)
    {
        Loggers.Clear();

        var rootNodes = new Dictionary<string, LoggerTreeNode>();

        foreach (var loggerName in loggerNames.Distinct().OrderBy(n => n))
        {
            var parts = loggerName.Split('.');
            var currentLevel = rootNodes;
            string currentPath = string.Empty;

            for (int i = 0; i < parts.Length; i++)
            {
                var part = parts[i];
                currentPath = string.IsNullOrEmpty(currentPath) ? part : $"{currentPath}.{part}";

                if (!currentLevel.ContainsKey(part))
                {
                    var node = new LoggerTreeNode
                    {
                        Name = part,
                        FullPath = currentPath,
                        IsExpanded = i < 2 // Auto-expand first two levels
                    };

                    currentLevel[part] = node;

                    if (i == 0)
                    {
                        Loggers.Add(node);
                    }
                }

                if (i < parts.Length - 1)
                {
                    currentLevel = currentLevel[part].ChildrenDict;
                }
            }
        }
    }
}

/// <summary>
/// Represents a node in the logger tree.
/// </summary>
public partial class LoggerTreeNode : ObservableObject
{
    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _fullPath = string.Empty;

    [ObservableProperty]
    private bool _isExpanded;

    [ObservableProperty]
    private bool _isSelected;

    [ObservableProperty]
    private ObservableCollection<LoggerTreeNode> _children = new();

    internal Dictionary<string, LoggerTreeNode> ChildrenDict { get; } = new();

    partial void OnChildrenChanged(ObservableCollection<LoggerTreeNode> value)
    {
        // Keep dict in sync
        ChildrenDict.Clear();
        foreach (var child in value)
        {
            ChildrenDict[child.Name] = child;
        }
    }
}
