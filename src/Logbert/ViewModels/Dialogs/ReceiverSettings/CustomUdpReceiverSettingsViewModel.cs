using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using Couchcoding.Logbert.Interfaces;
using Couchcoding.Logbert.Receiver;
using Couchcoding.Logbert.Receiver.CustomReceiver;
using Couchcoding.Logbert.Receiver.CustomReceiver.CustomUdpReceiver;

namespace Couchcoding.Logbert.ViewModels.Dialogs.ReceiverSettings;

public partial class CustomUdpReceiverSettingsViewModel : ViewModelBase, ILogSettingsCtrl
{
    [ObservableProperty]
    private int _port = 9999;

    [ObservableProperty]
    private string _listenInterfaceIp = "0.0.0.0";

    [ObservableProperty]
    private string _multicastIp = string.Empty;

    [ObservableProperty]
    private EncodingInfo? _selectedEncoding;

    public Columnizer? Columnizer { get; set; }

    public ObservableCollection<EncodingInfo> AvailableEncodings { get; } = new();

    public CustomUdpReceiverSettingsViewModel()
    {
        // Populate available encodings
        foreach (var encoding in Encoding.GetEncodings().OrderBy(e => e.DisplayName))
        {
            AvailableEncodings.Add(encoding);
        }

        // Set UTF-8 as default
        SelectedEncoding = AvailableEncodings.FirstOrDefault(e => e.CodePage == Encoding.UTF8.CodePage);
    }

    public ValidationResult ValidateSettings()
    {
        if (Port < 1 || Port > 65535)
        {
            return ValidationResult.Error("Port must be between 1 and 65535.");
        }

        if (!IPAddress.TryParse(ListenInterfaceIp, out _))
        {
            return ValidationResult.Error($"'{ListenInterfaceIp}' is not a valid IP address.");
        }

        if (!string.IsNullOrWhiteSpace(MulticastIp) && !IPAddress.TryParse(MulticastIp, out _))
        {
            return ValidationResult.Error($"'{MulticastIp}' is not a valid multicast IP address.");
        }

        if (Columnizer == null)
        {
            return ValidationResult.Error("Please configure a Columnizer.");
        }

        if (Columnizer.Columns.Count == 0)
        {
            return ValidationResult.Error("Columnizer must have at least one column defined.");
        }

        if (SelectedEncoding == null)
        {
            return ValidationResult.Error("Please select an encoding.");
        }

        return ValidationResult.Success;
    }

    public ILogProvider GetConfiguredInstance()
    {
        IPAddress? multicast = string.IsNullOrWhiteSpace(MulticastIp)
            ? null
            : IPAddress.Parse(MulticastIp);

        IPAddress listenIp = IPAddress.Parse(ListenInterfaceIp);
        IPEndPoint listenEndPoint = new IPEndPoint(listenIp, Port);
        int codepage = SelectedEncoding?.CodePage ?? Encoding.UTF8.CodePage;

        return new CustomUdpReceiver(multicast, listenEndPoint, Columnizer!, codepage);
    }
}
