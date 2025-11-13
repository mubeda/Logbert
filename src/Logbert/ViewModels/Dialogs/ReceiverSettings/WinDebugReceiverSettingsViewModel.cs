using System;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using Couchcoding.Logbert.Interfaces;
using Couchcoding.Logbert.Receiver;
using Couchcoding.Logbert.Receiver.WinDebugReceiver;

namespace Couchcoding.Logbert.ViewModels.Dialogs.ReceiverSettings;

public partial class WinDebugReceiverSettingsViewModel : ViewModelBase, ILogSettingsCtrl
{
    [ObservableProperty]
    private bool _captureAllProcesses = true;

    [ObservableProperty]
    private int _processId = 0;

    public ValidationResult ValidateSettings()
    {
        if (!CaptureAllProcesses && ProcessId <= 0)
        {
            return ValidationResult.Error("Please enter a valid Process ID (greater than 0).");
        }

        // Check if the process exists when capturing a specific process
        if (!CaptureAllProcesses)
        {
            try
            {
                var process = Process.GetProcessById(ProcessId);
                // Process exists, validation successful
            }
            catch (ArgumentException)
            {
                return ValidationResult.Error($"Process with ID {ProcessId} not found. Please verify the Process ID.");
            }
        }

        return ValidationResult.Success;
    }

    public ILogProvider GetConfiguredInstance()
    {
        if (CaptureAllProcesses)
        {
            // Pass null to capture all processes
            return new WinDebugReceiver(null);
        }
        else
        {
            try
            {
                var process = Process.GetProcessById(ProcessId);
                return new WinDebugReceiver(process);
            }
            catch (ArgumentException)
            {
                // This shouldn't happen as we validated in ValidateSettings
                // But as a fallback, return a receiver that captures all
                return new WinDebugReceiver(null);
            }
        }
    }

    public void Dispose()
    {
        // No resources to dispose
    }
}
