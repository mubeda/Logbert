using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Couchcoding.Logbert.Receiver.CustomReceiver;

namespace Couchcoding.Logbert.ViewModels.Dialogs;

/// <summary>
/// ViewModel for the Columnizer Test dialog.
/// </summary>
public partial class ColumnizerTestDialogViewModel : ViewModelBase
{
    #region Observable Properties

    /// <summary>
    /// Gets or sets the regex pattern to test.
    /// </summary>
    [ObservableProperty]
    private string _pattern = string.Empty;

    /// <summary>
    /// Gets or sets the sample log lines to test against.
    /// </summary>
    [ObservableProperty]
    private string _sampleText = string.Empty;

    /// <summary>
    /// Gets or sets whether the pattern is valid.
    /// </summary>
    [ObservableProperty]
    private bool _isPatternValid = true;

    /// <summary>
    /// Gets or sets the pattern validation message.
    /// </summary>
    [ObservableProperty]
    private string? _validationMessage;

    /// <summary>
    /// Gets or sets the match count.
    /// </summary>
    [ObservableProperty]
    private int _matchCount;

    /// <summary>
    /// Gets or sets whether there are any matches.
    /// </summary>
    [ObservableProperty]
    private bool _hasMatches;

    /// <summary>
    /// Gets or sets the status message.
    /// </summary>
    [ObservableProperty]
    private string _statusMessage = "Enter a regex pattern and sample log lines to test.";

    /// <summary>
    /// Gets or sets whether regex options are expanded.
    /// </summary>
    [ObservableProperty]
    private bool _showOptions = false;

    /// <summary>
    /// Gets or sets whether to use case-insensitive matching.
    /// </summary>
    [ObservableProperty]
    private bool _ignoreCase = true;

    /// <summary>
    /// Gets or sets whether to use multiline mode.
    /// </summary>
    [ObservableProperty]
    private bool _multiline = true;

    /// <summary>
    /// Gets or sets whether to use singleline mode.
    /// </summary>
    [ObservableProperty]
    private bool _singleline = false;

    #endregion

    #region Collections

    /// <summary>
    /// Gets the parsed results (matches).
    /// </summary>
    public ObservableCollection<MatchResultViewModel> MatchResults { get; } = new();

    /// <summary>
    /// Gets the extracted capture groups.
    /// </summary>
    public ObservableCollection<CaptureGroupViewModel> CaptureGroups { get; } = new();

    /// <summary>
    /// Gets the column type options.
    /// </summary>
    public ObservableCollection<string> ColumnTypes { get; } = new()
    {
        "Unknown",
        "Timestamp",
        "Level",
        "Logger",
        "Thread",
        "Message"
    };

    /// <summary>
    /// Gets the static list of column type values for binding in DataGrid.
    /// </summary>
    public static LogColumnType[] ColumnTypeValues { get; } =
        (LogColumnType[])Enum.GetValues(typeof(LogColumnType));

    #endregion

    #region Commands

    public IRelayCommand TestPatternCommand { get; }
    public IRelayCommand ClearCommand { get; }
    public IRelayCommand LoadSampleCommand { get; }
    public IRelayCommand InsertPatternCommand { get; }

    #endregion

    #region Constructor

    public ColumnizerTestDialogViewModel()
    {
        TestPatternCommand = new RelayCommand(OnTestPattern, CanTestPattern);
        ClearCommand = new RelayCommand(OnClear);
        LoadSampleCommand = new RelayCommand(OnLoadSample);
        InsertPatternCommand = new RelayCommand<string>(OnInsertPattern);

        PropertyChanged += (s, e) =>
        {
            if (e.PropertyName is nameof(Pattern) or nameof(SampleText))
            {
                ValidatePattern();
                ((RelayCommand)TestPatternCommand).NotifyCanExecuteChanged();
            }

            if (e.PropertyName is nameof(IgnoreCase) or nameof(Multiline) or nameof(Singleline))
            {
                if (!string.IsNullOrWhiteSpace(Pattern) && !string.IsNullOrWhiteSpace(SampleText))
                {
                    OnTestPattern();
                }
            }
        };
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Initializes the dialog with an existing pattern.
    /// </summary>
    public void Initialize(string? existingPattern = null)
    {
        if (!string.IsNullOrWhiteSpace(existingPattern))
        {
            Pattern = existingPattern;
        }
        else
        {
            // Set a default pattern as example
            Pattern = @"^(?<timestamp>\d{4}-\d{2}-\d{2}\s+\d{2}:\d{2}:\d{2})\s+\[(?<level>\w+)\]\s+(?<logger>[\w\.]+)\s+-\s+(?<message>.*)$";
        }

        // Set sample log lines
        SampleText = @"2024-01-15 10:30:45 [INFO] MyApp.Services.UserService - User login successful for user: admin
2024-01-15 10:30:46 [DEBUG] MyApp.Database.Connection - Connection pool size: 5
2024-01-15 10:30:47 [ERROR] MyApp.Api.AuthController - Authentication failed for user: guest
2024-01-15 10:30:48 [WARN] MyApp.Cache.Redis - Cache miss for key: session_12345";

        ValidatePattern();
    }

    /// <summary>
    /// Gets the current pattern.
    /// </summary>
    public string GetPattern() => Pattern;

    /// <summary>
    /// Gets the capture group mappings.
    /// </summary>
    public Dictionary<string, LogColumnType> GetCaptureGroupMappings()
    {
        var mappings = new Dictionary<string, LogColumnType>();
        foreach (var group in CaptureGroups)
        {
            mappings[group.GroupName] = group.ColumnType;
        }
        return mappings;
    }

    #endregion

    #region Private Methods

    private void ValidatePattern()
    {
        if (string.IsNullOrWhiteSpace(Pattern))
        {
            IsPatternValid = false;
            ValidationMessage = "Pattern cannot be empty.";
            return;
        }

        try
        {
            _ = new Regex(Pattern);
            IsPatternValid = true;
            ValidationMessage = "Valid regex pattern.";
        }
        catch (ArgumentException ex)
        {
            IsPatternValid = false;
            ValidationMessage = $"Invalid regex: {ex.Message}";
        }
    }

    private bool CanTestPattern()
    {
        return IsPatternValid &&
               !string.IsNullOrWhiteSpace(Pattern) &&
               !string.IsNullOrWhiteSpace(SampleText);
    }

    private void OnTestPattern()
    {
        MatchResults.Clear();
        CaptureGroups.Clear();

        if (!IsPatternValid || string.IsNullOrWhiteSpace(SampleText))
        {
            StatusMessage = "Enter valid pattern and sample text.";
            HasMatches = false;
            MatchCount = 0;
            return;
        }

        try
        {
            var options = RegexOptions.None;
            if (IgnoreCase) options |= RegexOptions.IgnoreCase;
            if (Multiline) options |= RegexOptions.Multiline;
            if (Singleline) options |= RegexOptions.Singleline;

            var regex = new Regex(Pattern, options);
            var lines = SampleText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            // Get group names
            var groupNames = regex.GetGroupNames()
                .Where(g => !int.TryParse(g, out _)) // Filter out numbered groups
                .ToList();

            // Initialize capture groups
            foreach (var groupName in groupNames)
            {
                CaptureGroups.Add(new CaptureGroupViewModel
                {
                    GroupName = groupName,
                    ColumnType = InferColumnType(groupName)
                });
            }

            int matchIndex = 0;
            foreach (var line in lines)
            {
                var match = regex.Match(line);
                if (match.Success)
                {
                    var result = new MatchResultViewModel
                    {
                        LineNumber = matchIndex + 1,
                        OriginalLine = line,
                        IsMatch = true
                    };

                    // Extract group values
                    foreach (var groupName in groupNames)
                    {
                        var group = match.Groups[groupName];
                        if (group.Success)
                        {
                            result.GroupValues.Add(new GroupValueViewModel
                            {
                                GroupName = groupName,
                                Value = group.Value
                            });
                        }
                    }

                    MatchResults.Add(result);
                }
                else
                {
                    MatchResults.Add(new MatchResultViewModel
                    {
                        LineNumber = matchIndex + 1,
                        OriginalLine = line,
                        IsMatch = false
                    });
                }

                matchIndex++;
            }

            MatchCount = MatchResults.Count(r => r.IsMatch);
            HasMatches = MatchCount > 0;

            StatusMessage = MatchCount > 0
                ? $"Pattern matched {MatchCount} of {lines.Length} lines. Found {groupNames.Count} named groups."
                : "Pattern did not match any lines.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error testing pattern: {ex.Message}";
            HasMatches = false;
            MatchCount = 0;
        }
    }

    private LogColumnType InferColumnType(string groupName)
    {
        var name = groupName.ToLowerInvariant();

        if (name.Contains("time") || name.Contains("date") || name.Contains("timestamp"))
            return LogColumnType.Timestamp;

        if (name.Contains("level") || name.Contains("severity"))
            return LogColumnType.Level;

        if (name.Contains("logger") || name.Contains("class") || name.Contains("source"))
            return LogColumnType.Logger;

        if (name.Contains("thread") || name.Contains("tid"))
            return LogColumnType.Thread;

        if (name.Contains("message") || name.Contains("msg") || name.Contains("text"))
            return LogColumnType.Message;

        return LogColumnType.Unknown;
    }

    private void OnClear()
    {
        SampleText = string.Empty;
        MatchResults.Clear();
        CaptureGroups.Clear();
        MatchCount = 0;
        HasMatches = false;
        StatusMessage = "Enter a regex pattern and sample log lines to test.";
    }

    private void OnLoadSample()
    {
        // Add common log format samples
        SampleText = @"2024-01-15 10:30:45.123 [INFO] [Thread-1] MyApp.Services.UserService - User login successful
2024-01-15 10:30:46.456 [DEBUG] [Thread-2] MyApp.Database.Connection - Executing query: SELECT * FROM users
2024-01-15 10:30:47.789 [ERROR] [Thread-1] MyApp.Api.AuthController - Authentication failed: Invalid credentials
2024-01-15 10:30:48.012 [WARN] [Thread-3] MyApp.Cache.Redis - Cache miss for key: user_profile_123
2024-01-15 10:30:49.345 [TRACE] [Thread-2] MyApp.Logging.Internal - Detailed trace information
2024-01-15 10:30:50.678 [FATAL] [Thread-1] MyApp.Core.Startup - Critical system failure detected";
    }

    private void OnInsertPattern(string? patternName)
    {
        Pattern = patternName switch
        {
            "simple" => @"^(?<level>\w+)\s+(?<message>.*)$",
            "log4j" => @"^(?<timestamp>\d{4}-\d{2}-\d{2}\s+\d{2}:\d{2}:\d{2},\d{3})\s+\[(?<thread>[^\]]+)\]\s+(?<level>\w+)\s+(?<logger>[\w\.]+)\s+-\s+(?<message>.*)$",
            "syslog" => @"^(?<timestamp>\w{3}\s+\d+\s+\d{2}:\d{2}:\d{2})\s+(?<host>\S+)\s+(?<process>\S+)\[?(?<pid>\d+)?\]?:\s+(?<message>.*)$",
            "iis" => @"^(?<date>\d{4}-\d{2}-\d{2})\s+(?<time>\d{2}:\d{2}:\d{2})\s+(?<sitename>\S+)\s+(?<server>\S+)\s+(?<ip>\S+)\s+(?<method>\w+)\s+(?<uristem>\S+)\s+(?<uriquery>\S+)\s+(?<port>\d+)\s+(?<username>\S+)\s+(?<clientip>\S+).*$",
            "custom" => @"^\[(?<timestamp>[^\]]+)\]\s+\[(?<level>[^\]]+)\]\s+\[(?<logger>[^\]]+)\]\s+(?<message>.*)$",
            _ => Pattern
        };
    }

    #endregion
}

/// <summary>
/// Represents a match result from testing.
/// </summary>
public partial class MatchResultViewModel : ObservableObject
{
    [ObservableProperty]
    private int _lineNumber;

    [ObservableProperty]
    private string _originalLine = string.Empty;

    [ObservableProperty]
    private bool _isMatch;

    public ObservableCollection<GroupValueViewModel> GroupValues { get; } = new();

    /// <summary>
    /// Gets a display string for the match result.
    /// </summary>
    public string DisplayText => IsMatch
        ? $"Line {LineNumber}: Matched ({GroupValues.Count} groups)"
        : $"Line {LineNumber}: No match";
}

/// <summary>
/// Represents a captured group value.
/// </summary>
public partial class GroupValueViewModel : ObservableObject
{
    [ObservableProperty]
    private string _groupName = string.Empty;

    [ObservableProperty]
    private string _value = string.Empty;
}

/// <summary>
/// Represents a capture group with its column type mapping.
/// </summary>
public partial class CaptureGroupViewModel : ObservableObject
{
    [ObservableProperty]
    private string _groupName = string.Empty;

    [ObservableProperty]
    private LogColumnType _columnType = LogColumnType.Unknown;

    public string ColumnTypeName => ColumnType.ToString();
}
