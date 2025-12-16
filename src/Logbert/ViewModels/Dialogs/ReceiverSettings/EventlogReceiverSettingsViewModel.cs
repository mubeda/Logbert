using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Logbert.Interfaces;
using Logbert.Receiver;
using Logbert.Receiver.EventlogReceiver;

namespace Logbert.ViewModels.Dialogs.ReceiverSettings;

public partial class EventlogReceiverSettingsViewModel : ViewModelBase, ILogSettingsCtrl
{
    [ObservableProperty]
    private string _logName = "Application";

    [ObservableProperty]
    private string _machineName = ".";

    [ObservableProperty]
    private string _sourceName = string.Empty;

    /// <summary>
    /// Gets the collection of common event log names
    /// </summary>
    public ObservableCollection<string> AvailableLogNames { get; } = new()
    {
        "Application",
        "System",
        "Security",
        "Setup"
    };

    public ValidationResult ValidateSettings()
    {
        if (string.IsNullOrWhiteSpace(LogName))
        {
            return ValidationResult.Error("Log name cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(MachineName))
        {
            return ValidationResult.Error("Machine name cannot be empty.");
        }

        return ValidationResult.Success;
    }

    public ILogProvider GetConfiguredInstance()
    {
        return new EventlogReceiver(
            LogName.Trim(),
            MachineName.Trim(),
            string.IsNullOrWhiteSpace(SourceName) ? string.Empty : SourceName.Trim());
    }

    /// <summary>
    /// Gets a display string for the receiver configuration.
    /// </summary>
    public string GetDisplayInfo()
    {
        return LogName;
    }

    public void Dispose()
    {
        // No resources to dispose
    }
}
