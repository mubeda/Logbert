using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Logbert.ViewModels.Dialogs;

/// <summary>
/// Represents a keyboard shortcut.
/// </summary>
public class ShortcutItem
{
    public string Key { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
}

/// <summary>
/// Represents a category of shortcuts.
/// </summary>
public class ShortcutCategory
{
    public string Name { get; set; } = string.Empty;
    public ObservableCollection<ShortcutItem> Shortcuts { get; } = new();
}

/// <summary>
/// ViewModel for the Keyboard Shortcuts dialog.
/// </summary>
public partial class KeyboardShortcutsDialogViewModel : ViewModelBase
{
    /// <summary>
    /// Gets the collection of shortcut categories.
    /// </summary>
    public ObservableCollection<ShortcutCategory> Categories { get; } = new();

    /// <summary>
    /// Gets or sets the search filter text.
    /// </summary>
    [ObservableProperty]
    private string _searchFilter = string.Empty;

    public KeyboardShortcutsDialogViewModel()
    {
        LoadShortcuts();
    }

    private void LoadShortcuts()
    {
        // File operations
        var fileCategory = new ShortcutCategory { Name = "File Operations" };
        fileCategory.Shortcuts.Add(new ShortcutItem { Key = "Ctrl+N", Description = "New log source", Category = "File" });
        fileCategory.Shortcuts.Add(new ShortcutItem { Key = "Ctrl+O", Description = "Open log file", Category = "File" });
        fileCategory.Shortcuts.Add(new ShortcutItem { Key = "Ctrl+W", Description = "Close current document", Category = "File" });
        fileCategory.Shortcuts.Add(new ShortcutItem { Key = "Ctrl+E", Description = "Export log messages", Category = "File" });
        fileCategory.Shortcuts.Add(new ShortcutItem { Key = "Ctrl+Q", Description = "Exit application", Category = "File" });
        Categories.Add(fileCategory);

        // Edit operations
        var editCategory = new ShortcutCategory { Name = "Edit" };
        editCategory.Shortcuts.Add(new ShortcutItem { Key = "Ctrl+C", Description = "Copy selected messages", Category = "Edit" });
        editCategory.Shortcuts.Add(new ShortcutItem { Key = "Ctrl+A", Description = "Select all messages", Category = "Edit" });
        editCategory.Shortcuts.Add(new ShortcutItem { Key = "Delete", Description = "Clear selected messages", Category = "Edit" });
        Categories.Add(editCategory);

        // Search and Filter
        var searchCategory = new ShortcutCategory { Name = "Search & Filter" };
        searchCategory.Shortcuts.Add(new ShortcutItem { Key = "Ctrl+F", Description = "Open search dialog", Category = "Search" });
        searchCategory.Shortcuts.Add(new ShortcutItem { Key = "F3", Description = "Find next match", Category = "Search" });
        searchCategory.Shortcuts.Add(new ShortcutItem { Key = "Shift+F3", Description = "Find previous match", Category = "Search" });
        searchCategory.Shortcuts.Add(new ShortcutItem { Key = "Ctrl+G", Description = "Go to line number", Category = "Search" });
        searchCategory.Shortcuts.Add(new ShortcutItem { Key = "Escape", Description = "Clear search / Close dialog", Category = "Search" });
        Categories.Add(searchCategory);

        // Navigation
        var navCategory = new ShortcutCategory { Name = "Navigation" };
        navCategory.Shortcuts.Add(new ShortcutItem { Key = "Home", Description = "Go to first message", Category = "Navigation" });
        navCategory.Shortcuts.Add(new ShortcutItem { Key = "End", Description = "Go to last message", Category = "Navigation" });
        navCategory.Shortcuts.Add(new ShortcutItem { Key = "Page Up", Description = "Scroll up one page", Category = "Navigation" });
        navCategory.Shortcuts.Add(new ShortcutItem { Key = "Page Down", Description = "Scroll down one page", Category = "Navigation" });
        navCategory.Shortcuts.Add(new ShortcutItem { Key = "Ctrl+Home", Description = "Go to beginning of log", Category = "Navigation" });
        navCategory.Shortcuts.Add(new ShortcutItem { Key = "Ctrl+End", Description = "Go to end of log", Category = "Navigation" });
        Categories.Add(navCategory);

        // Bookmarks
        var bookmarkCategory = new ShortcutCategory { Name = "Bookmarks" };
        bookmarkCategory.Shortcuts.Add(new ShortcutItem { Key = "Ctrl+B", Description = "Toggle bookmark on current line", Category = "Bookmarks" });
        bookmarkCategory.Shortcuts.Add(new ShortcutItem { Key = "Ctrl+Shift+B", Description = "Clear all bookmarks", Category = "Bookmarks" });
        bookmarkCategory.Shortcuts.Add(new ShortcutItem { Key = "F2", Description = "Go to next bookmark", Category = "Bookmarks" });
        bookmarkCategory.Shortcuts.Add(new ShortcutItem { Key = "Shift+F2", Description = "Go to previous bookmark", Category = "Bookmarks" });
        Categories.Add(bookmarkCategory);

        // View
        var viewCategory = new ShortcutCategory { Name = "View" };
        viewCategory.Shortcuts.Add(new ShortcutItem { Key = "F11", Description = "Toggle full screen", Category = "View" });
        viewCategory.Shortcuts.Add(new ShortcutItem { Key = "Ctrl++", Description = "Zoom in", Category = "View" });
        viewCategory.Shortcuts.Add(new ShortcutItem { Key = "Ctrl+-", Description = "Zoom out", Category = "View" });
        viewCategory.Shortcuts.Add(new ShortcutItem { Key = "Ctrl+0", Description = "Reset zoom", Category = "View" });
        Categories.Add(viewCategory);

        // Script
        var scriptCategory = new ShortcutCategory { Name = "Script" };
        scriptCategory.Shortcuts.Add(new ShortcutItem { Key = "F5", Description = "Run script", Category = "Script" });
        scriptCategory.Shortcuts.Add(new ShortcutItem { Key = "Ctrl+S", Description = "Save script", Category = "Script" });
        scriptCategory.Shortcuts.Add(new ShortcutItem { Key = "Ctrl+Shift+S", Description = "Save script as...", Category = "Script" });
        Categories.Add(scriptCategory);

        // Help
        var helpCategory = new ShortcutCategory { Name = "Help" };
        helpCategory.Shortcuts.Add(new ShortcutItem { Key = "F1", Description = "Open help / documentation", Category = "Help" });
        helpCategory.Shortcuts.Add(new ShortcutItem { Key = "Ctrl+,", Description = "Open options", Category = "Help" });
        Categories.Add(helpCategory);
    }

    partial void OnSearchFilterChanged(string value)
    {
        // Filter shortcuts based on search text
        // In a full implementation, this would filter the visible shortcuts
    }
}
