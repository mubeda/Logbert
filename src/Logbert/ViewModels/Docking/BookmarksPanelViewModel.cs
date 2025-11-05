using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Couchcoding.Logbert.Logging;

namespace Couchcoding.Logbert.ViewModels.Docking;

/// <summary>
/// ViewModel for the bookmarks panel tool window.
/// </summary>
public partial class BookmarksPanelViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<BookmarkItem> _bookmarks = new();

    [ObservableProperty]
    private BookmarkItem? _selectedBookmark;

    public ICommand ClearBookmarksCommand { get; } = null!;
    public ICommand RemoveBookmarkCommand { get; } = null!;

    public BookmarksPanelViewModel()
    {
        ClearBookmarksCommand = new RelayCommand(OnClearBookmarks, CanClearBookmarks);
        RemoveBookmarkCommand = new RelayCommand<BookmarkItem>(OnRemoveBookmark);

        Bookmarks.CollectionChanged += (s, e) =>
        {
            ((RelayCommand)ClearBookmarksCommand).NotifyCanExecuteChanged();
        };
    }

    private bool CanClearBookmarks()
    {
        return Bookmarks.Count > 0;
    }

    private void OnClearBookmarks()
    {
        Bookmarks.Clear();
    }

    private void OnRemoveBookmark(BookmarkItem? bookmark)
    {
        if (bookmark != null)
        {
            Bookmarks.Remove(bookmark);
        }
    }

    /// <summary>
    /// Adds a bookmark for a log message.
    /// </summary>
    public void AddBookmark(LogMessage message, string? comment = null)
    {
        var bookmark = new BookmarkItem
        {
            Message = message,
            Comment = comment ?? string.Empty,
            CreatedAt = DateTime.Now
        };

        Bookmarks.Add(bookmark);
    }

    /// <summary>
    /// Toggles a bookmark for a log message.
    /// </summary>
    public void ToggleBookmark(LogMessage message)
    {
        var existing = Bookmarks.FirstOrDefault(b => b.Message == message);
        if (existing != null)
        {
            Bookmarks.Remove(existing);
        }
        else
        {
            AddBookmark(message);
        }
    }
}

/// <summary>
/// Represents a bookmarked log message.
/// </summary>
public partial class BookmarkItem : ObservableObject
{
    [ObservableProperty]
    private LogMessage _message = null!;

    [ObservableProperty]
    private string _comment = string.Empty;

    [ObservableProperty]
    private DateTime _createdAt;
}
