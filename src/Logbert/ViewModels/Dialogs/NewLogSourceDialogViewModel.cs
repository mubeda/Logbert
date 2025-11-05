using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Couchcoding.Logbert.ViewModels.Dialogs;

/// <summary>
/// ViewModel for the New Log Source dialog.
/// </summary>
public partial class NewLogSourceDialogViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<LogReceiverType> _availableReceivers = new();

    [ObservableProperty]
    private LogReceiverType? _selectedReceiver;

    [ObservableProperty]
    private string _description = string.Empty;

    public IRelayCommand OkCommand { get; } = null!;
    public IRelayCommand CancelCommand { get; } = null!;

    public bool DialogResult { get; private set; }

    public NewLogSourceDialogViewModel()
    {
        OkCommand = new RelayCommand(OnOk, CanOk);
        CancelCommand = new RelayCommand(OnCancel);

        InitializeReceivers();

        PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(SelectedReceiver))
            {
                OnSelectedReceiverChanged();
                ((RelayCommand)OkCommand).NotifyCanExecuteChanged();
            }
        };
    }

    private void InitializeReceivers()
    {
        AvailableReceivers.Add(new LogReceiverType
        {
            Name = "Log4Net File",
            Description = "Read log messages from a Log4Net log file",
            Category = "File"
        });

        AvailableReceivers.Add(new LogReceiverType
        {
            Name = "NLog File",
            Description = "Read log messages from an NLog log file",
            Category = "File"
        });

        AvailableReceivers.Add(new LogReceiverType
        {
            Name = "Syslog File",
            Description = "Read log messages from a Syslog file",
            Category = "File"
        });

        AvailableReceivers.Add(new LogReceiverType
        {
            Name = "Custom File",
            Description = "Read log messages from a custom formatted file",
            Category = "File"
        });

        AvailableReceivers.Add(new LogReceiverType
        {
            Name = "Log4Net UDP",
            Description = "Receive log messages via UDP from Log4Net",
            Category = "Network"
        });

        AvailableReceivers.Add(new LogReceiverType
        {
            Name = "NLog TCP",
            Description = "Receive log messages via TCP from NLog",
            Category = "Network"
        });

        AvailableReceivers.Add(new LogReceiverType
        {
            Name = "NLog UDP",
            Description = "Receive log messages via UDP from NLog",
            Category = "Network"
        });

        AvailableReceivers.Add(new LogReceiverType
        {
            Name = "Syslog UDP",
            Description = "Receive log messages via UDP using Syslog protocol",
            Category = "Network"
        });

        AvailableReceivers.Add(new LogReceiverType
        {
            Name = "Custom UDP",
            Description = "Receive log messages via UDP with custom format",
            Category = "Network"
        });

        AvailableReceivers.Add(new LogReceiverType
        {
            Name = "Custom HTTP",
            Description = "Receive log messages via HTTP endpoint",
            Category = "Network"
        });

        AvailableReceivers.Add(new LogReceiverType
        {
            Name = "Windows Event Log",
            Description = "Read log messages from Windows Event Log",
            Category = "System"
        });

        AvailableReceivers.Add(new LogReceiverType
        {
            Name = "Windows Debug Output",
            Description = "Capture Windows debug output messages",
            Category = "System"
        });
    }

    private void OnSelectedReceiverChanged()
    {
        Description = SelectedReceiver?.Description ?? string.Empty;
    }

    private bool CanOk()
    {
        return SelectedReceiver != null;
    }

    private void OnOk()
    {
        DialogResult = true;
    }

    private void OnCancel()
    {
        DialogResult = false;
    }
}

/// <summary>
/// Represents a type of log receiver.
/// </summary>
public partial class LogReceiverType : ObservableObject
{
    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private string _category = string.Empty;
}
