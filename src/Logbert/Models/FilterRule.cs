using System;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Couchcoding.Logbert.Models;

/// <summary>
/// Represents a single filter rule for log message filtering.
/// </summary>
public partial class FilterRule : ObservableObject
{
    #region Enums

    /// <summary>
    /// The column to filter on.
    /// </summary>
    public enum FilterColumn
    {
        Message = 0,
        Logger = 1,
        Thread = 2,
        Timestamp = 3,
        Level = 4,
        Any = 5
    }

    /// <summary>
    /// The filter operator.
    /// </summary>
    public enum FilterOperator
    {
        Contains = 0,
        NotContains = 1,
        Equals = 2,
        NotEquals = 3,
        StartsWith = 4,
        EndsWith = 5,
        MatchesRegex = 6,
        NotMatchesRegex = 7
    }

    #endregion

    #region Observable Properties

    [ObservableProperty]
    private bool _isEnabled = true;

    [ObservableProperty]
    private string _name = "New Filter";

    [ObservableProperty]
    private FilterColumn _column = FilterColumn.Message;

    [ObservableProperty]
    private FilterOperator _operator = FilterOperator.Contains;

    [ObservableProperty]
    private string _value = string.Empty;

    [ObservableProperty]
    private bool _caseSensitive = false;

    #endregion

    #region Computed Properties

    /// <summary>
    /// Gets a display string for this filter rule.
    /// </summary>
    [JsonIgnore]
    public string DisplayText => $"{Column} {GetOperatorDisplayText()} \"{Value}\"";

    /// <summary>
    /// Gets a value indicating whether this filter uses regex.
    /// </summary>
    [JsonIgnore]
    public bool IsRegexFilter => Operator == FilterOperator.MatchesRegex || Operator == FilterOperator.NotMatchesRegex;

    /// <summary>
    /// Gets a value indicating whether the regex pattern is valid.
    /// </summary>
    [JsonIgnore]
    public bool IsValidPattern
    {
        get
        {
            if (!IsRegexFilter) return true;
            if (string.IsNullOrEmpty(Value)) return false;
            try
            {
                _ = new Regex(Value);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Gets the validation error message, if any.
    /// </summary>
    [JsonIgnore]
    public string? ValidationError
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Value))
                return "Filter value cannot be empty";

            if (IsRegexFilter)
            {
                try
                {
                    _ = new Regex(Value);
                }
                catch (ArgumentException ex)
                {
                    return $"Invalid regex: {ex.Message}";
                }
            }
            return null;
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Tests if the given text matches this filter rule.
    /// </summary>
    /// <param name="text">The text to test.</param>
    /// <returns>True if the text matches the filter; otherwise false.</returns>
    public bool Matches(string? text)
    {
        if (string.IsNullOrEmpty(text))
            return Operator == FilterOperator.NotContains ||
                   Operator == FilterOperator.NotEquals ||
                   Operator == FilterOperator.NotMatchesRegex;

        var comparison = CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        var regexOptions = CaseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase;

        return Operator switch
        {
            FilterOperator.Contains => text.Contains(Value, comparison),
            FilterOperator.NotContains => !text.Contains(Value, comparison),
            FilterOperator.Equals => text.Equals(Value, comparison),
            FilterOperator.NotEquals => !text.Equals(Value, comparison),
            FilterOperator.StartsWith => text.StartsWith(Value, comparison),
            FilterOperator.EndsWith => text.EndsWith(Value, comparison),
            FilterOperator.MatchesRegex => TryMatchRegex(text, regexOptions),
            FilterOperator.NotMatchesRegex => !TryMatchRegex(text, regexOptions),
            _ => false
        };
    }

    /// <summary>
    /// Creates a deep copy of this filter rule.
    /// </summary>
    /// <returns>A new FilterRule with the same values.</returns>
    public FilterRule Clone()
    {
        return new FilterRule
        {
            IsEnabled = IsEnabled,
            Name = Name,
            Column = Column,
            Operator = Operator,
            Value = Value,
            CaseSensitive = CaseSensitive
        };
    }

    #endregion

    #region Private Methods

    private bool TryMatchRegex(string text, RegexOptions options)
    {
        try
        {
            return Regex.IsMatch(text, Value, options);
        }
        catch
        {
            return false;
        }
    }

    private string GetOperatorDisplayText()
    {
        return Operator switch
        {
            FilterOperator.Contains => "contains",
            FilterOperator.NotContains => "does not contain",
            FilterOperator.Equals => "equals",
            FilterOperator.NotEquals => "does not equal",
            FilterOperator.StartsWith => "starts with",
            FilterOperator.EndsWith => "ends with",
            FilterOperator.MatchesRegex => "matches regex",
            FilterOperator.NotMatchesRegex => "does not match regex",
            _ => "?"
        };
    }

    #endregion

    #region Static Factory Methods

    /// <summary>
    /// Creates a new filter rule with default values.
    /// </summary>
    public static FilterRule CreateDefault()
    {
        return new FilterRule
        {
            Name = "New Filter",
            Column = FilterColumn.Message,
            Operator = FilterOperator.Contains,
            Value = string.Empty,
            IsEnabled = true,
            CaseSensitive = false
        };
    }

    /// <summary>
    /// Creates a filter rule for quick message text filtering.
    /// </summary>
    public static FilterRule CreateMessageContains(string text)
    {
        return new FilterRule
        {
            Name = $"Message contains \"{text}\"",
            Column = FilterColumn.Message,
            Operator = FilterOperator.Contains,
            Value = text,
            IsEnabled = true,
            CaseSensitive = false
        };
    }

    /// <summary>
    /// Creates a filter rule for logger name filtering.
    /// </summary>
    public static FilterRule CreateLoggerEquals(string loggerName)
    {
        return new FilterRule
        {
            Name = $"Logger = {loggerName}",
            Column = FilterColumn.Logger,
            Operator = FilterOperator.Equals,
            Value = loggerName,
            IsEnabled = true,
            CaseSensitive = false
        };
    }

    #endregion
}

/// <summary>
/// Represents the logic mode for combining multiple filter rules.
/// </summary>
public enum FilterLogicMode
{
    /// <summary>
    /// All filter rules must match (AND logic).
    /// </summary>
    And = 0,

    /// <summary>
    /// Any filter rule must match (OR logic).
    /// </summary>
    Or = 1
}
