using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Logbert.Logging;

namespace Logbert.ViewModels.Docking;

/// <summary>
/// ViewModel for the logger tree tool window.
/// </summary>
public partial class LoggerTreeViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<LoggerTreeNode> _loggers = new();

    [ObservableProperty]
    private LoggerTreeNode? _selectedLogger;

    /// <summary>
    /// Gets or sets whether filtering is recursive (includes child loggers).
    /// </summary>
    [ObservableProperty]
    private bool _isRecursive = true;

    /// <summary>
    /// Gets the currently selected logger path for filtering.
    /// Returns null when root is selected or no selection.
    /// </summary>
    public string? SelectedLoggerPath => SelectedLogger?.FullPath;

    /// <summary>
    /// Event raised when the logger filter changes.
    /// </summary>
    public event EventHandler<LoggerFilterChangedEventArgs>? LoggerFilterChanged;

    /// <summary>
    /// Command to toggle recursive filtering.
    /// </summary>
    public IRelayCommand ToggleRecursiveCommand { get; }

    /// <summary>
    /// Command to clear the logger filter (select root).
    /// </summary>
    public IRelayCommand ClearFilterCommand { get; }

    /// <summary>
    /// Command to sync tree to current message.
    /// </summary>
    public IRelayCommand SyncToMessageCommand { get; }

    /// <summary>
    /// Event raised when sync to message is requested.
    /// </summary>
    public event EventHandler? SyncToMessageRequested;

    public LoggerTreeViewModel()
    {
        ToggleRecursiveCommand = new RelayCommand(OnToggleRecursive);
        ClearFilterCommand = new RelayCommand(OnClearFilter);
        SyncToMessageCommand = new RelayCommand(OnSyncToMessage);
    }

    private void OnSyncToMessage()
    {
        SyncToMessageRequested?.Invoke(this, EventArgs.Empty);
    }

    partial void OnSelectedLoggerChanged(LoggerTreeNode? value)
    {
        OnPropertyChanged(nameof(SelectedLoggerPath));
        RaiseLoggerFilterChanged();
    }

    partial void OnIsRecursiveChanged(bool value)
    {
        RaiseLoggerFilterChanged();
    }

    private void OnToggleRecursive()
    {
        IsRecursive = !IsRecursive;
    }

    private void OnClearFilter()
    {
        SelectedLogger = Loggers.Count > 0 ? Loggers[0] : null;
    }

    private void RaiseLoggerFilterChanged()
    {
        // Root node or no selection means no filter
        bool isFiltering = SelectedLogger != null && SelectedLogger.FullPath != "Root";
        string? filterPath = isFiltering ? SelectedLogger!.FullPath : null;

        LoggerFilterChanged?.Invoke(this, new LoggerFilterChangedEventArgs(filterPath, IsRecursive));
    }

    /// <summary>
    /// Synchronizes the tree selection to match the specified log message's logger.
    /// </summary>
    /// <param name="message">The log message to sync to.</param>
    public void SynchronizeToMessage(LogMessage? message)
    {
        if (message == null || string.IsNullOrEmpty(message.Logger))
        {
            return;
        }

        var targetPath = message.Logger;
        var node = FindNodeByPath(Loggers, targetPath);
        if (node != null)
        {
            // Expand parent nodes and select the target
            ExpandPathToNode(node);
            SelectedLogger = node;
        }
    }

    private LoggerTreeNode? FindNodeByPath(IEnumerable<LoggerTreeNode> nodes, string targetPath)
    {
        foreach (var node in nodes)
        {
            if (node.FullPath == targetPath)
            {
                return node;
            }

            var found = FindNodeByPath(node.Children, targetPath);
            if (found != null)
            {
                return found;
            }
        }
        return null;
    }

    private void ExpandPathToNode(LoggerTreeNode node)
    {
        // Find the path of parent nodes and expand them
        var parts = node.FullPath.Split('.');
        string currentPath = "";

        foreach (var part in parts.Take(parts.Length - 1))
        {
            currentPath = string.IsNullOrEmpty(currentPath) ? part : $"{currentPath}.{part}";
            var parentNode = FindNodeByPath(Loggers, currentPath);
            if (parentNode != null)
            {
                parentNode.IsExpanded = true;
            }
        }
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

/// <summary>
/// Event arguments for logger filter changes.
/// </summary>
public class LoggerFilterChangedEventArgs : EventArgs
{
    /// <summary>
    /// Gets the logger path to filter by. Null means no filter (show all).
    /// </summary>
    public string? LoggerPath { get; }

    /// <summary>
    /// Gets whether the filter should include child loggers.
    /// </summary>
    public bool IsRecursive { get; }

    public LoggerFilterChangedEventArgs(string? loggerPath, bool isRecursive)
    {
        LoggerPath = loggerPath;
        IsRecursive = isRecursive;
    }
}
