using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Logbert.Logging;

namespace Logbert.ViewModels.Dialogs;

/// <summary>
/// ViewModel for the Search dialog.
/// </summary>
public partial class SearchDialogViewModel : ViewModelBase
{
    private LogDocumentViewModel? _targetDocument;
    private int _currentSearchIndex = -1;
    private int _lastStartIndex = -1;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private bool _matchCase = false;

    [ObservableProperty]
    private bool _matchWholeWord = false;

    [ObservableProperty]
    private bool _useRegularExpression = false;

    [ObservableProperty]
    private bool _searchBackward = false;

    [ObservableProperty]
    private ObservableCollection<string> _searchHistory = new();

    [ObservableProperty]
    private string? _statusMessage;

    public IRelayCommand FindNextCommand { get; } = null!;
    public IRelayCommand FindPreviousCommand { get; } = null!;

    public SearchDialogViewModel()
    {
        FindNextCommand = new RelayCommand(OnFindNext, CanFind);
        FindPreviousCommand = new RelayCommand(OnFindPrevious, CanFind);

        PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(SearchText))
            {
                ((RelayCommand)FindNextCommand).NotifyCanExecuteChanged();
                ((RelayCommand)FindPreviousCommand).NotifyCanExecuteChanged();
                _currentSearchIndex = -1; // Reset search when text changes
            }
        };
    }

    private bool CanFind()
    {
        return !string.IsNullOrWhiteSpace(SearchText) && _targetDocument != null;
    }

    private void OnFindNext()
    {
        SearchBackward = false;
        PerformSearch();
    }

    private void OnFindPrevious()
    {
        SearchBackward = true;
        PerformSearch();
    }

    private void PerformSearch()
    {
        if (_targetDocument == null || string.IsNullOrWhiteSpace(SearchText))
        {
            StatusMessage = "No document selected or empty search text";
            return;
        }

        // Add to search history if not already present
        if (!SearchHistory.Contains(SearchText))
        {
            SearchHistory.Insert(0, SearchText);

            // Limit history to 10 items
            while (SearchHistory.Count > 10)
            {
                SearchHistory.RemoveAt(SearchHistory.Count - 1);
            }
        }

        var messages = _targetDocument.LogViewerViewModel.FilteredMessages;
        if (messages.Count == 0)
        {
            StatusMessage = "No messages to search";
            return;
        }

        // Determine starting index
        int startIndex;
        if (_currentSearchIndex == -1)
        {
            // First search or text changed
            var selectedIndex = messages.IndexOf(_targetDocument.SelectedMessage);
            startIndex = selectedIndex >= 0 ? selectedIndex : 0;
            _lastStartIndex = startIndex;
        }
        else
        {
            // Continue from last found position
            startIndex = _currentSearchIndex;
        }

        // Search for next match
        int foundIndex = SearchBackward
            ? FindPreviousMatch(messages, startIndex)
            : FindNextMatch(messages, startIndex);

        if (foundIndex >= 0)
        {
            _currentSearchIndex = foundIndex;
            _targetDocument.SelectedMessage = messages[foundIndex];
            _targetDocument.LogViewerViewModel.SelectedMessage = messages[foundIndex];

            int matchNumber = CountMatchesUpTo(messages, foundIndex) + 1;
            int totalMatches = CountTotalMatches(messages);
            StatusMessage = $"Found match {matchNumber} of {totalMatches}";
        }
        else
        {
            // Wrap around search
            if (SearchBackward && startIndex > 0)
            {
                foundIndex = FindPreviousMatch(messages, messages.Count - 1);
            }
            else if (!SearchBackward && startIndex < messages.Count - 1)
            {
                foundIndex = FindNextMatch(messages, -1);
            }

            if (foundIndex >= 0)
            {
                _currentSearchIndex = foundIndex;
                _targetDocument.SelectedMessage = messages[foundIndex];
                _targetDocument.LogViewerViewModel.SelectedMessage = messages[foundIndex];

                int matchNumber = CountMatchesUpTo(messages, foundIndex) + 1;
                int totalMatches = CountTotalMatches(messages);
                StatusMessage = $"Found match {matchNumber} of {totalMatches} (wrapped)";
            }
            else
            {
                StatusMessage = $"'{SearchText}' not found";
            }
        }
    }

    private int FindNextMatch(ObservableCollection<LogMessage> messages, int startIndex)
    {
        for (int i = startIndex + 1; i < messages.Count; i++)
        {
            if (IsMatch(messages[i]))
            {
                return i;
            }
        }
        return -1;
    }

    private int FindPreviousMatch(ObservableCollection<LogMessage> messages, int startIndex)
    {
        for (int i = startIndex - 1; i >= 0; i--)
        {
            if (IsMatch(messages[i]))
            {
                return i;
            }
        }
        return -1;
    }

    private bool IsMatch(LogMessage message)
    {
        var searchIn = message.Message ?? string.Empty;

        if (UseRegularExpression)
        {
            try
            {
                var options = MatchCase ? RegexOptions.None : RegexOptions.IgnoreCase;
                return Regex.IsMatch(searchIn, SearchText, options);
            }
            catch (ArgumentException)
            {
                // Invalid regex pattern
                StatusMessage = "Invalid regular expression";
                return false;
            }
        }
        else
        {
            var comparison = MatchCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

            if (MatchWholeWord)
            {
                var words = searchIn.Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                return words.Any(w => string.Equals(w, SearchText, comparison));
            }
            else
            {
                return searchIn.Contains(SearchText, comparison);
            }
        }
    }

    private int CountMatchesUpTo(ObservableCollection<LogMessage> messages, int index)
    {
        int count = 0;
        for (int i = 0; i <= index; i++)
        {
            if (IsMatch(messages[i]))
            {
                count++;
            }
        }
        return count;
    }

    private int CountTotalMatches(ObservableCollection<LogMessage> messages)
    {
        return messages.Count(IsMatch);
    }

    /// <summary>
    /// Sets the document to search in.
    /// </summary>
    public void SetSearchTarget(LogDocumentViewModel? document)
    {
        _targetDocument = document;
        _currentSearchIndex = -1;
        _lastStartIndex = -1;
        ((RelayCommand)FindNextCommand).NotifyCanExecuteChanged();
        ((RelayCommand)FindPreviousCommand).NotifyCanExecuteChanged();

        if (document == null)
        {
            StatusMessage = "No document selected";
        }
        else
        {
            StatusMessage = null;
        }
    }
}
