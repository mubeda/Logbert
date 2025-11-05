using CommunityToolkit.Mvvm.ComponentModel;

namespace Couchcoding.Logbert.ViewModels.Docking;

/// <summary>
/// ViewModel for the filter panel tool window.
/// </summary>
public partial class FilterPanelViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _filterText = string.Empty;

    [ObservableProperty]
    private bool _showTrace = true;

    [ObservableProperty]
    private bool _showDebug = true;

    [ObservableProperty]
    private bool _showInfo = true;

    [ObservableProperty]
    private bool _showWarning = true;

    [ObservableProperty]
    private bool _showError = true;

    [ObservableProperty]
    private bool _showFatal = true;

    [ObservableProperty]
    private bool _caseSensitive = false;

    [ObservableProperty]
    private bool _useRegex = false;

    public FilterPanelViewModel()
    {
        // Listen for property changes to apply filters
        PropertyChanged += (s, e) =>
        {
            if (e.PropertyName is nameof(FilterText) or
                nameof(ShowTrace) or nameof(ShowDebug) or
                nameof(ShowInfo) or nameof(ShowWarning) or
                nameof(ShowError) or nameof(ShowFatal) or
                nameof(CaseSensitive) or nameof(UseRegex))
            {
                // Filter changes will be handled by the LogDocumentViewModel
                // through data binding
            }
        };
    }
}
