using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Logbert.Logging;

namespace Logbert.ViewModels.Dialogs;

/// <summary>
/// ViewModel for the Log Level Mapping dialog.
/// Allows configuration of regex patterns for each log level.
/// </summary>
public partial class LogLevelMapDialogViewModel : ObservableObject
{
    #region Observable Properties

    [ObservableProperty]
    private string _tracePattern = @"(?i)TRACE(?-i)";

    [ObservableProperty]
    private string _debugPattern = @"(?i)DEBUG(?-i)";

    [ObservableProperty]
    private string _infoPattern = @"(?i)INFO(?-i)";

    [ObservableProperty]
    private string _warningPattern = @"(?i)WARN|WARNING(?-i)";

    [ObservableProperty]
    private string _errorPattern = @"(?i)ERROR(?-i)";

    [ObservableProperty]
    private string _fatalPattern = @"(?i)FATAL(?-i)";

    [ObservableProperty]
    private bool? _dialogResult;

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets the log level mapping dictionary.
    /// </summary>
    public Dictionary<LogLevel, string> LogLevelMapping => new()
    {
        { LogLevel.Trace, TracePattern },
        { LogLevel.Debug, DebugPattern },
        { LogLevel.Info, InfoPattern },
        { LogLevel.Warning, WarningPattern },
        { LogLevel.Error, ErrorPattern },
        { LogLevel.Fatal, FatalPattern }
    };

    #endregion

    #region Commands

    [RelayCommand]
    private void Accept()
    {
        DialogResult = true;
    }

    [RelayCommand]
    private void Cancel()
    {
        DialogResult = false;
    }

    [RelayCommand]
    private void ResetToDefaults()
    {
        TracePattern = @"(?i)TRACE(?-i)";
        DebugPattern = @"(?i)DEBUG(?-i)";
        InfoPattern = @"(?i)INFO(?-i)";
        WarningPattern = @"(?i)WARN|WARNING(?-i)";
        ErrorPattern = @"(?i)ERROR(?-i)";
        FatalPattern = @"(?i)FATAL(?-i)";
    }

    #endregion

    #region Constructor

    /// <summary>
    /// Creates a new instance of the LogLevelMapDialogViewModel.
    /// </summary>
    public LogLevelMapDialogViewModel()
    {
    }

    /// <summary>
    /// Creates a new instance with existing mappings.
    /// </summary>
    /// <param name="existingMappings">The existing log level mappings to edit.</param>
    public LogLevelMapDialogViewModel(Dictionary<LogLevel, string> existingMappings)
    {
        if (existingMappings == null) return;

        if (existingMappings.TryGetValue(LogLevel.Trace, out var trace))
            TracePattern = trace;
        if (existingMappings.TryGetValue(LogLevel.Debug, out var debug))
            DebugPattern = debug;
        if (existingMappings.TryGetValue(LogLevel.Info, out var info))
            InfoPattern = info;
        if (existingMappings.TryGetValue(LogLevel.Warning, out var warning))
            WarningPattern = warning;
        if (existingMappings.TryGetValue(LogLevel.Error, out var error))
            ErrorPattern = error;
        if (existingMappings.TryGetValue(LogLevel.Fatal, out var fatal))
            FatalPattern = fatal;
    }

    #endregion
}
