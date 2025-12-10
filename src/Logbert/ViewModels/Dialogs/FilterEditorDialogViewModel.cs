using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Couchcoding.Logbert.Models;

namespace Couchcoding.Logbert.ViewModels.Dialogs;

/// <summary>
/// ViewModel for the Filter Editor dialog.
/// </summary>
public partial class FilterEditorDialogViewModel : ViewModelBase
{
    #region Fields

    private FilterRule? _editingRule;

    #endregion

    #region Observable Properties

    /// <summary>
    /// Gets or sets whether the filter rule is enabled.
    /// </summary>
    [ObservableProperty]
    private bool _isEnabled = true;

    /// <summary>
    /// Gets or sets the filter name.
    /// </summary>
    [ObservableProperty]
    private string _filterName = "New Filter";

    /// <summary>
    /// Gets or sets the selected column index.
    /// </summary>
    [ObservableProperty]
    private int _selectedColumnIndex = 0;

    /// <summary>
    /// Gets or sets the selected operator index.
    /// </summary>
    [ObservableProperty]
    private int _selectedOperatorIndex = 0;

    /// <summary>
    /// Gets or sets the filter value.
    /// </summary>
    [ObservableProperty]
    private string _filterValue = string.Empty;

    /// <summary>
    /// Gets or sets whether the filter is case sensitive.
    /// </summary>
    [ObservableProperty]
    private bool _caseSensitive = false;

    /// <summary>
    /// Gets or sets the regex validation message.
    /// </summary>
    [ObservableProperty]
    private string? _validationMessage;

    /// <summary>
    /// Gets or sets whether the regex is valid.
    /// </summary>
    [ObservableProperty]
    private bool _isValid = true;

    /// <summary>
    /// Gets or sets the sample text for testing.
    /// </summary>
    [ObservableProperty]
    private string _sampleText = string.Empty;

    /// <summary>
    /// Gets or sets the test result message.
    /// </summary>
    [ObservableProperty]
    private string? _testResultMessage;

    /// <summary>
    /// Gets or sets whether the test passed.
    /// </summary>
    [ObservableProperty]
    private bool _testPassed = false;

    /// <summary>
    /// Gets or sets the dialog title.
    /// </summary>
    [ObservableProperty]
    private string _dialogTitle = "Add Filter Rule";

    #endregion

    #region Collections

    /// <summary>
    /// Gets the available columns for filtering.
    /// </summary>
    public ObservableCollection<string> Columns { get; } = new()
    {
        "Message",
        "Logger",
        "Thread",
        "Timestamp",
        "Level",
        "Any Column"
    };

    /// <summary>
    /// Gets the available filter operators.
    /// </summary>
    public ObservableCollection<string> Operators { get; } = new()
    {
        "Contains",
        "Does Not Contain",
        "Equals",
        "Does Not Equal",
        "Starts With",
        "Ends With",
        "Matches Regex",
        "Does Not Match Regex"
    };

    #endregion

    #region Commands

    public IRelayCommand TestFilterCommand { get; }
    public IRelayCommand ClearTestCommand { get; }
    public IRelayCommand AcceptCommand { get; }
    public IRelayCommand CancelCommand { get; }

    #endregion

    #region Events

    /// <summary>
    /// Raised when the dialog is accepted.
    /// </summary>
    public event EventHandler<FilterRule>? Accepted;

    /// <summary>
    /// Raised when the dialog is cancelled.
    /// </summary>
    public event EventHandler? Cancelled;

    #endregion

    #region Constructor

    public FilterEditorDialogViewModel()
    {
        TestFilterCommand = new RelayCommand(OnTestFilter, CanTestFilter);
        ClearTestCommand = new RelayCommand(OnClearTest);
        AcceptCommand = new RelayCommand(OnAccept, CanAccept);
        CancelCommand = new RelayCommand(OnCancel);

        PropertyChanged += (s, e) =>
        {
            if (e.PropertyName is nameof(FilterValue) or nameof(SelectedOperatorIndex))
            {
                ValidateFilter();
                ((RelayCommand)TestFilterCommand).NotifyCanExecuteChanged();
                ((RelayCommand)AcceptCommand).NotifyCanExecuteChanged();
            }

            if (e.PropertyName is nameof(SampleText))
            {
                ((RelayCommand)TestFilterCommand).NotifyCanExecuteChanged();
            }
        };
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Initializes the dialog for adding a new filter rule.
    /// </summary>
    public void InitializeForAdd()
    {
        _editingRule = null;
        DialogTitle = "Add Filter Rule";

        IsEnabled = true;
        FilterName = "New Filter";
        SelectedColumnIndex = 0;
        SelectedOperatorIndex = 0;
        FilterValue = string.Empty;
        CaseSensitive = false;
        SampleText = string.Empty;
        TestResultMessage = null;
        TestPassed = false;

        ValidateFilter();
    }

    /// <summary>
    /// Initializes the dialog for editing an existing filter rule.
    /// </summary>
    /// <param name="rule">The rule to edit.</param>
    public void InitializeForEdit(FilterRule rule)
    {
        _editingRule = rule;
        DialogTitle = "Edit Filter Rule";

        IsEnabled = rule.IsEnabled;
        FilterName = rule.Name;
        SelectedColumnIndex = (int)rule.Column;
        SelectedOperatorIndex = (int)rule.Operator;
        FilterValue = rule.Value;
        CaseSensitive = rule.CaseSensitive;
        SampleText = string.Empty;
        TestResultMessage = null;
        TestPassed = false;

        ValidateFilter();
    }

    /// <summary>
    /// Gets the filter rule from the current dialog state.
    /// </summary>
    public FilterRule GetFilterRule()
    {
        var rule = _editingRule ?? new FilterRule();

        rule.IsEnabled = IsEnabled;
        rule.Name = FilterName;
        rule.Column = (FilterRule.FilterColumn)SelectedColumnIndex;
        rule.Operator = (FilterRule.FilterOperator)SelectedOperatorIndex;
        rule.Value = FilterValue;
        rule.CaseSensitive = CaseSensitive;

        return rule;
    }

    #endregion

    #region Private Methods

    private void ValidateFilter()
    {
        if (string.IsNullOrWhiteSpace(FilterValue))
        {
            ValidationMessage = "Filter value cannot be empty";
            IsValid = false;
            return;
        }

        // Check if using regex operators
        if (SelectedOperatorIndex == 6 || SelectedOperatorIndex == 7)
        {
            try
            {
                _ = new Regex(FilterValue);
                ValidationMessage = "Valid regex pattern";
                IsValid = true;
            }
            catch (ArgumentException ex)
            {
                ValidationMessage = $"Invalid regex: {ex.Message}";
                IsValid = false;
            }
        }
        else
        {
            ValidationMessage = null;
            IsValid = true;
        }
    }

    private bool CanTestFilter()
    {
        return !string.IsNullOrWhiteSpace(FilterValue) &&
               !string.IsNullOrWhiteSpace(SampleText) &&
               IsValid;
    }

    private void OnTestFilter()
    {
        var rule = GetFilterRule();
        bool matches = rule.Matches(SampleText);

        TestPassed = matches;
        TestResultMessage = matches
            ? "Filter matches the sample text"
            : "Filter does NOT match the sample text";
    }

    private void OnClearTest()
    {
        SampleText = string.Empty;
        TestResultMessage = null;
        TestPassed = false;
    }

    private bool CanAccept()
    {
        return !string.IsNullOrWhiteSpace(FilterValue) && IsValid;
    }

    private void OnAccept()
    {
        var rule = GetFilterRule();
        Accepted?.Invoke(this, rule);
    }

    private void OnCancel()
    {
        Cancelled?.Invoke(this, EventArgs.Empty);
    }

    #endregion

    #region Import/Export

    /// <summary>
    /// Exports a filter rule to JSON.
    /// </summary>
    public static string ExportToJson(FilterRule rule)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        return JsonSerializer.Serialize(rule, options);
    }

    /// <summary>
    /// Exports multiple filter rules to JSON.
    /// </summary>
    public static string ExportToJson(ObservableCollection<FilterRule> rules)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        return JsonSerializer.Serialize(rules, options);
    }

    /// <summary>
    /// Imports a filter rule from JSON.
    /// </summary>
    public static FilterRule? ImportFromJson(string json)
    {
        try
        {
            return JsonSerializer.Deserialize<FilterRule>(json);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Imports multiple filter rules from JSON.
    /// </summary>
    public static ObservableCollection<FilterRule>? ImportMultipleFromJson(string json)
    {
        try
        {
            var rules = JsonSerializer.Deserialize<ObservableCollection<FilterRule>>(json);
            return rules;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Saves filter rules to a file.
    /// </summary>
    public static async System.Threading.Tasks.Task SaveToFileAsync(ObservableCollection<FilterRule> rules, string filePath)
    {
        var json = ExportToJson(rules);
        await File.WriteAllTextAsync(filePath, json);
    }

    /// <summary>
    /// Loads filter rules from a file.
    /// </summary>
    public static async System.Threading.Tasks.Task<ObservableCollection<FilterRule>?> LoadFromFileAsync(string filePath)
    {
        if (!File.Exists(filePath))
            return null;

        var json = await File.ReadAllTextAsync(filePath);
        return ImportMultipleFromJson(json);
    }

    #endregion
}
