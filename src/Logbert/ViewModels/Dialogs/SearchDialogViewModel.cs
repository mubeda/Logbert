using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Couchcoding.Logbert.ViewModels.Dialogs;

/// <summary>
/// ViewModel for the Search dialog.
/// </summary>
public partial class SearchDialogViewModel : ViewModelBase
{
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

    public IRelayCommand FindNextCommand { get; }
    public IRelayCommand FindPreviousCommand { get; }

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
            }
        };
    }

    private bool CanFind()
    {
        return !string.IsNullOrWhiteSpace(SearchText);
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
        // Add to search history if not already present
        if (!string.IsNullOrWhiteSpace(SearchText) && !SearchHistory.Contains(SearchText))
        {
            SearchHistory.Insert(0, SearchText);

            // Limit history to 10 items
            while (SearchHistory.Count > 10)
            {
                SearchHistory.RemoveAt(SearchHistory.Count - 1);
            }
        }

        // TODO: Implement actual search logic
        // This will be implemented when we integrate with LogViewerViewModel
        StatusMessage = $"Searching for: {SearchText}";
    }

    /// <summary>
    /// Sets the document to search in.
    /// </summary>
    public void SetSearchTarget(LogDocumentViewModel? document)
    {
        // TODO: Store reference to search target
    }
}
