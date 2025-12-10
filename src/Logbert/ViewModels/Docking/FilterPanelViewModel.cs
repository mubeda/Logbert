using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Couchcoding.Logbert.Models;

namespace Couchcoding.Logbert.ViewModels.Docking;

/// <summary>
/// ViewModel for the filter panel tool window.
/// </summary>
public partial class FilterPanelViewModel : ViewModelBase
{
    #region Quick Filter Properties

    [ObservableProperty]
    private string _filterText = string.Empty;

    [ObservableProperty]
    private bool _caseSensitive = false;

    [ObservableProperty]
    private bool _useRegex = false;

    #endregion

    #region Log Level Filter Properties

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

    #endregion

    #region Advanced Filter Properties

    /// <summary>
    /// Gets or sets the collection of advanced filter rules.
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<FilterRule> _filterRules = new();

    /// <summary>
    /// Gets or sets the selected filter rule.
    /// </summary>
    [ObservableProperty]
    private FilterRule? _selectedFilterRule;

    /// <summary>
    /// Gets or sets the filter logic mode (AND/OR).
    /// </summary>
    [ObservableProperty]
    private FilterLogicMode _logicMode = FilterLogicMode.And;

    /// <summary>
    /// Gets or sets whether to use AND logic (true) or OR logic (false).
    /// </summary>
    [ObservableProperty]
    private bool _useAndLogic = true;

    /// <summary>
    /// Gets or sets the logger filter text.
    /// </summary>
    [ObservableProperty]
    private string _loggerFilter = string.Empty;

    /// <summary>
    /// Gets or sets the thread filter text.
    /// </summary>
    [ObservableProperty]
    private string _threadFilter = string.Empty;

    /// <summary>
    /// Gets or sets the minimum timestamp filter.
    /// </summary>
    [ObservableProperty]
    private DateTime? _timestampFrom;

    /// <summary>
    /// Gets or sets the maximum timestamp filter.
    /// </summary>
    [ObservableProperty]
    private DateTime? _timestampTo;

    /// <summary>
    /// Gets or sets whether to show the advanced filter section.
    /// </summary>
    [ObservableProperty]
    private bool _showAdvancedFilters = false;

    #endregion

    #region Commands

    public IRelayCommand AddFilterRuleCommand { get; }
    public IRelayCommand EditFilterRuleCommand { get; }
    public IRelayCommand RemoveFilterRuleCommand { get; }
    public IRelayCommand ClearAllFiltersCommand { get; }
    public IRelayCommand ToggleFilterEnabledCommand { get; }
    public IRelayCommand ImportFiltersCommand { get; }
    public IRelayCommand ExportFiltersCommand { get; }
    public IRelayCommand MoveFilterUpCommand { get; }
    public IRelayCommand MoveFilterDownCommand { get; }

    #endregion

    #region Events

    /// <summary>
    /// Raised when any filter property changes.
    /// </summary>
    public event EventHandler? FiltersChanged;

    /// <summary>
    /// Raised when the Add Filter dialog should be shown.
    /// </summary>
    public event EventHandler? AddFilterRequested;

    /// <summary>
    /// Raised when the Edit Filter dialog should be shown.
    /// </summary>
    public event EventHandler<FilterRule>? EditFilterRequested;

    /// <summary>
    /// Raised when import dialog should be shown.
    /// </summary>
    public event EventHandler? ImportFiltersRequested;

    /// <summary>
    /// Raised when export dialog should be shown.
    /// </summary>
    public event EventHandler? ExportFiltersRequested;

    #endregion

    #region Constructor

    public FilterPanelViewModel()
    {
        AddFilterRuleCommand = new RelayCommand(OnAddFilterRule);
        EditFilterRuleCommand = new RelayCommand(OnEditFilterRule, CanEditFilterRule);
        RemoveFilterRuleCommand = new RelayCommand(OnRemoveFilterRule, CanEditFilterRule);
        ClearAllFiltersCommand = new RelayCommand(OnClearAllFilters, HasActiveFilters);
        ToggleFilterEnabledCommand = new RelayCommand<FilterRule>(OnToggleFilterEnabled);
        ImportFiltersCommand = new RelayCommand(OnImportFilters);
        ExportFiltersCommand = new RelayCommand(OnExportFilters, () => FilterRules.Count > 0);
        MoveFilterUpCommand = new RelayCommand(OnMoveFilterUp, CanMoveFilterUp);
        MoveFilterDownCommand = new RelayCommand(OnMoveFilterDown, CanMoveFilterDown);

        // Listen for filter rule collection changes
        FilterRules.CollectionChanged += OnFilterRulesCollectionChanged;

        // Listen for property changes to apply filters
        PropertyChanged += (s, e) =>
        {
            if (e.PropertyName is nameof(FilterText) or
                nameof(ShowTrace) or nameof(ShowDebug) or
                nameof(ShowInfo) or nameof(ShowWarning) or
                nameof(ShowError) or nameof(ShowFatal) or
                nameof(CaseSensitive) or nameof(UseRegex) or
                nameof(LogicMode) or nameof(UseAndLogic) or
                nameof(LoggerFilter) or nameof(ThreadFilter) or
                nameof(TimestampFrom) or nameof(TimestampTo))
            {
                OnFiltersChanged();
            }

            if (e.PropertyName == nameof(UseAndLogic))
            {
                LogicMode = UseAndLogic ? FilterLogicMode.And : FilterLogicMode.Or;
            }

            if (e.PropertyName == nameof(SelectedFilterRule))
            {
                UpdateCommandStates();
            }
        };
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Adds a new filter rule.
    /// </summary>
    public void AddFilterRule(FilterRule rule)
    {
        FilterRules.Add(rule);
        OnFiltersChanged();
    }

    /// <summary>
    /// Updates an existing filter rule.
    /// </summary>
    public void UpdateFilterRule(FilterRule originalRule, FilterRule updatedRule)
    {
        var index = FilterRules.IndexOf(originalRule);
        if (index >= 0)
        {
            FilterRules[index] = updatedRule;
            OnFiltersChanged();
        }
    }

    /// <summary>
    /// Removes a filter rule.
    /// </summary>
    public void RemoveFilterRule(FilterRule rule)
    {
        FilterRules.Remove(rule);
        OnFiltersChanged();
    }

    /// <summary>
    /// Gets the number of active (enabled) filter rules.
    /// </summary>
    public int GetActiveFilterCount()
    {
        return FilterRules.Count(r => r.IsEnabled);
    }

    /// <summary>
    /// Gets a summary of active filters.
    /// </summary>
    public string GetFilterSummary()
    {
        var activeCount = GetActiveFilterCount();
        if (activeCount == 0)
            return "No active filters";

        var logicText = UseAndLogic ? "AND" : "OR";
        return $"{activeCount} filter(s) active ({logicText})";
    }

    /// <summary>
    /// Clears all quick filters but keeps the rules.
    /// </summary>
    public void ClearQuickFilters()
    {
        FilterText = string.Empty;
        LoggerFilter = string.Empty;
        ThreadFilter = string.Empty;
        TimestampFrom = null;
        TimestampTo = null;
    }

    /// <summary>
    /// Resets all log level filters to show all.
    /// </summary>
    public void ShowAllLogLevels()
    {
        ShowTrace = true;
        ShowDebug = true;
        ShowInfo = true;
        ShowWarning = true;
        ShowError = true;
        ShowFatal = true;
    }

    /// <summary>
    /// Exports filter rules to JSON string.
    /// </summary>
    public string ExportFiltersToJson()
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        return JsonSerializer.Serialize(FilterRules, options);
    }

    /// <summary>
    /// Imports filter rules from JSON string.
    /// </summary>
    public bool ImportFiltersFromJson(string json)
    {
        try
        {
            var rules = JsonSerializer.Deserialize<ObservableCollection<FilterRule>>(json);
            if (rules != null)
            {
                FilterRules.Clear();
                foreach (var rule in rules)
                {
                    FilterRules.Add(rule);
                }
                OnFiltersChanged();
                return true;
            }
        }
        catch
        {
            // Invalid JSON
        }
        return false;
    }

    /// <summary>
    /// Saves filter rules to a file.
    /// </summary>
    public async Task SaveFiltersToFileAsync(string filePath)
    {
        var json = ExportFiltersToJson();
        await File.WriteAllTextAsync(filePath, json);
    }

    /// <summary>
    /// Loads filter rules from a file.
    /// </summary>
    public async Task<bool> LoadFiltersFromFileAsync(string filePath)
    {
        if (!File.Exists(filePath))
            return false;

        var json = await File.ReadAllTextAsync(filePath);
        return ImportFiltersFromJson(json);
    }

    #endregion

    #region Private Methods

    private void OnAddFilterRule()
    {
        AddFilterRequested?.Invoke(this, EventArgs.Empty);
    }

    private void OnEditFilterRule()
    {
        if (SelectedFilterRule != null)
        {
            EditFilterRequested?.Invoke(this, SelectedFilterRule);
        }
    }

    private bool CanEditFilterRule()
    {
        return SelectedFilterRule != null;
    }

    private void OnRemoveFilterRule()
    {
        if (SelectedFilterRule != null)
        {
            FilterRules.Remove(SelectedFilterRule);
            SelectedFilterRule = null;
            OnFiltersChanged();
        }
    }

    private void OnClearAllFilters()
    {
        FilterRules.Clear();
        ClearQuickFilters();
        OnFiltersChanged();
    }

    private bool HasActiveFilters()
    {
        return FilterRules.Count > 0 ||
               !string.IsNullOrWhiteSpace(FilterText) ||
               !string.IsNullOrWhiteSpace(LoggerFilter) ||
               !string.IsNullOrWhiteSpace(ThreadFilter) ||
               TimestampFrom.HasValue ||
               TimestampTo.HasValue;
    }

    private void OnToggleFilterEnabled(FilterRule? rule)
    {
        if (rule != null)
        {
            rule.IsEnabled = !rule.IsEnabled;
            OnFiltersChanged();
        }
    }

    private void OnImportFilters()
    {
        ImportFiltersRequested?.Invoke(this, EventArgs.Empty);
    }

    private void OnExportFilters()
    {
        ExportFiltersRequested?.Invoke(this, EventArgs.Empty);
    }

    private void OnMoveFilterUp()
    {
        if (SelectedFilterRule != null)
        {
            var index = FilterRules.IndexOf(SelectedFilterRule);
            if (index > 0)
            {
                FilterRules.Move(index, index - 1);
            }
        }
    }

    private bool CanMoveFilterUp()
    {
        if (SelectedFilterRule == null) return false;
        var index = FilterRules.IndexOf(SelectedFilterRule);
        return index > 0;
    }

    private void OnMoveFilterDown()
    {
        if (SelectedFilterRule != null)
        {
            var index = FilterRules.IndexOf(SelectedFilterRule);
            if (index < FilterRules.Count - 1)
            {
                FilterRules.Move(index, index + 1);
            }
        }
    }

    private bool CanMoveFilterDown()
    {
        if (SelectedFilterRule == null) return false;
        var index = FilterRules.IndexOf(SelectedFilterRule);
        return index >= 0 && index < FilterRules.Count - 1;
    }

    private void OnFilterRulesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        OnFiltersChanged();
        UpdateCommandStates();
    }

    private void OnFiltersChanged()
    {
        FiltersChanged?.Invoke(this, EventArgs.Empty);
    }

    private void UpdateCommandStates()
    {
        ((RelayCommand)EditFilterRuleCommand).NotifyCanExecuteChanged();
        ((RelayCommand)RemoveFilterRuleCommand).NotifyCanExecuteChanged();
        ((RelayCommand)ClearAllFiltersCommand).NotifyCanExecuteChanged();
        ((RelayCommand)ExportFiltersCommand).NotifyCanExecuteChanged();
        ((RelayCommand)MoveFilterUpCommand).NotifyCanExecuteChanged();
        ((RelayCommand)MoveFilterDownCommand).NotifyCanExecuteChanged();
    }

    #endregion
}
