using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Logbert.Logging;

namespace Logbert.ViewModels.Docking;

/// <summary>
/// ViewModel for the dockable search panel.
/// </summary>
public partial class SearchPanelViewModel : ViewModelBase
{
    private LogDocumentViewModel? _targetDocument;
    private int _currentSearchIndex = -1;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private bool _matchCase = false;

    [ObservableProperty]
    private bool _matchWholeWord = false;

    [ObservableProperty]
    private bool _useRegularExpression = false;

    [ObservableProperty]
    private string? _statusMessage;

    [ObservableProperty]
    private ObservableCollection<LogMessage> _searchResults = new();

    [ObservableProperty]
    private LogMessage? _selectedResult;

    /// <summary>
    /// Gets the results header showing count.
    /// </summary>
    public string ResultsHeader => SearchResults.Count > 0
        ? $"Results ({SearchResults.Count})"
        : "Results";

    public IRelayCommand FindNextCommand { get; }
    public IRelayCommand FindPreviousCommand { get; }
    public IRelayCommand FindAllCommand { get; }

    /// <summary>
    /// Event raised when navigation to a result is requested.
    /// </summary>
    public event EventHandler<LogMessage>? NavigateToResult;

    public SearchPanelViewModel()
    {
        FindNextCommand = new RelayCommand(OnFindNext, CanFind);
        FindPreviousCommand = new RelayCommand(OnFindPrevious, CanFind);
        FindAllCommand = new RelayCommand(OnFindAll, CanFind);

        PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(SearchText))
            {
                ((RelayCommand)FindNextCommand).NotifyCanExecuteChanged();
                ((RelayCommand)FindPreviousCommand).NotifyCanExecuteChanged();
                ((RelayCommand)FindAllCommand).NotifyCanExecuteChanged();
                _currentSearchIndex = -1;
            }
            if (e.PropertyName == nameof(SearchResults))
            {
                OnPropertyChanged(nameof(ResultsHeader));
            }
        };

        SearchResults.CollectionChanged += (s, e) => OnPropertyChanged(nameof(ResultsHeader));
    }

    private bool CanFind()
    {
        return !string.IsNullOrWhiteSpace(SearchText) && _targetDocument != null;
    }

    private void OnFindNext()
    {
        PerformSearch(false);
    }

    private void OnFindPrevious()
    {
        PerformSearch(true);
    }

    private void OnFindAll()
    {
        if (_targetDocument == null || string.IsNullOrWhiteSpace(SearchText))
        {
            StatusMessage = "No document selected or empty search text";
            return;
        }

        var messages = _targetDocument.LogViewerViewModel.FilteredMessages;
        SearchResults.Clear();

        foreach (var message in messages)
        {
            if (IsMatch(message))
            {
                SearchResults.Add(message);
            }
        }

        StatusMessage = SearchResults.Count > 0
            ? $"Found {SearchResults.Count} matches"
            : $"'{SearchText}' not found";
    }

    private void PerformSearch(bool backward)
    {
        if (_targetDocument == null || string.IsNullOrWhiteSpace(SearchText))
        {
            StatusMessage = "No document selected or empty search text";
            return;
        }

        var messages = _targetDocument.LogViewerViewModel.FilteredMessages;
        if (messages.Count == 0)
        {
            StatusMessage = "No messages to search";
            return;
        }

        int startIndex;
        if (_currentSearchIndex == -1)
        {
            var selectedIndex = messages.IndexOf(_targetDocument.SelectedMessage);
            startIndex = selectedIndex >= 0 ? selectedIndex : 0;
        }
        else
        {
            startIndex = _currentSearchIndex;
        }

        int foundIndex = backward
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
            // Wrap around
            if (backward && startIndex > 0)
            {
                foundIndex = FindPreviousMatch(messages, messages.Count - 1);
            }
            else if (!backward && startIndex < messages.Count - 1)
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
        SearchResults.Clear();
        ((RelayCommand)FindNextCommand).NotifyCanExecuteChanged();
        ((RelayCommand)FindPreviousCommand).NotifyCanExecuteChanged();
        ((RelayCommand)FindAllCommand).NotifyCanExecuteChanged();

        StatusMessage = document == null ? "No document selected" : null;
    }

    /// <summary>
    /// Navigates to the currently selected search result.
    /// </summary>
    public void NavigateToSelectedResult()
    {
        if (SelectedResult != null)
        {
            NavigateToResult?.Invoke(this, SelectedResult);

            if (_targetDocument != null)
            {
                _targetDocument.SelectedMessage = SelectedResult;
                _targetDocument.LogViewerViewModel.SelectedMessage = SelectedResult;
            }
        }
    }
}
