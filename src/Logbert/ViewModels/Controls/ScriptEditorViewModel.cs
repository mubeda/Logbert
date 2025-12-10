using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Logbert.ViewModels.Controls;

/// <summary>
/// ViewModel for the script editor control.
/// </summary>
public partial class ScriptEditorViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _scriptText = string.Empty;

    [ObservableProperty]
    private string _fileName = "Untitled.lua";

    [ObservableProperty]
    private bool _isModified = false;

    [ObservableProperty]
    private int _cursorLine = 1;

    [ObservableProperty]
    private int _cursorColumn = 1;

    [ObservableProperty]
    private string _statusMessage = "Ready";

    [ObservableProperty]
    private ObservableCollection<string> _outputMessages = new();

    public IRelayCommand ExecuteScriptCommand { get; } = null!;
    public IRelayCommand ClearOutputCommand { get; } = null!;
    public IRelayCommand NewScriptCommand { get; } = null!;
    public IRelayCommand OpenScriptCommand { get; } = null!;
    public IRelayCommand SaveScriptCommand { get; } = null!;

    public ScriptEditorViewModel()
    {
        ExecuteScriptCommand = new RelayCommand(OnExecuteScript, CanExecuteScript);
        ClearOutputCommand = new RelayCommand(OnClearOutput);
        NewScriptCommand = new RelayCommand(OnNewScript);
        OpenScriptCommand = new RelayCommand(OnOpenScript);
        SaveScriptCommand = new RelayCommand(OnSaveScript, CanSaveScript);

        PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(ScriptText))
            {
                IsModified = true;
                ((RelayCommand)ExecuteScriptCommand).NotifyCanExecuteChanged();
                ((RelayCommand)SaveScriptCommand).NotifyCanExecuteChanged();
            }
        };

        // Set default Lua template
        ScriptText = @"-- Logbert Lua Script
-- This script can filter and process log messages

function filter(message)
    -- Return true to show the message, false to hide it
    -- Available properties:
    --   message.Level (Trace, Debug, Info, Warning, Error, Fatal)
    --   message.Message (string)
    --   message.Logger (string)
    --   message.Timestamp (datetime)
    --   message.ThreadName (string)

    -- Example: Only show errors and warnings
    -- return message.Level == 'Error' or message.Level == 'Warning'

    return true
end

function process(message)
    -- Process or transform the message
    -- Return the modified message or nil to remove it

    return message
end

print('Script loaded successfully')
";
        IsModified = false;
    }

    private bool CanExecuteScript()
    {
        return !string.IsNullOrWhiteSpace(ScriptText);
    }

    private void OnExecuteScript()
    {
        OutputMessages.Clear();
        OutputMessages.Add($"[{DateTime.Now:HH:mm:ss}] Executing script...");

        try
        {
            // TODO: Implement MoonSharp execution
            OutputMessages.Add($"[{DateTime.Now:HH:mm:ss}] Script execution not yet implemented");
            StatusMessage = "Execution completed";
        }
        catch (Exception ex)
        {
            OutputMessages.Add($"[{DateTime.Now:HH:mm:ss}] ERROR: {ex.Message}");
            StatusMessage = "Execution failed";
        }
    }

    private void OnClearOutput()
    {
        OutputMessages.Clear();
        StatusMessage = "Output cleared";
    }

    private void OnNewScript()
    {
        if (IsModified)
        {
            // TODO: Show confirmation dialog
        }

        ScriptText = "-- New Lua Script\n\n";
        FileName = "Untitled.lua";
        IsModified = false;
        StatusMessage = "New script created";
    }

    private void OnOpenScript()
    {
        // TODO: Show file open dialog
        StatusMessage = "Open script not yet implemented";
    }

    private bool CanSaveScript()
    {
        return IsModified && !string.IsNullOrWhiteSpace(ScriptText);
    }

    private void OnSaveScript()
    {
        // TODO: Show file save dialog
        StatusMessage = "Save script not yet implemented";
    }

    public void AppendOutput(string message)
    {
        OutputMessages.Add($"[{DateTime.Now:HH:mm:ss}] {message}");
    }

    public void UpdateCursorPosition(int line, int column)
    {
        CursorLine = line;
        CursorColumn = column;
    }
}
