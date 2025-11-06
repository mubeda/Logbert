using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using Couchcoding.Logbert.Interfaces;
using Couchcoding.Logbert.Receiver;
using Couchcoding.Logbert.Receiver.SyslogUdpReceiver;

namespace Couchcoding.Logbert.ViewModels.Dialogs.ReceiverSettings
{
    /// <summary>
    /// ViewModel for Syslog UDP Receiver configuration.
    /// </summary>
    public partial class SyslogUdpReceiverSettingsViewModel : ObservableObject, ILogSettingsCtrl
    {
        [ObservableProperty]
        private int _port = 514;

        [ObservableProperty]
        private string _listenInterfaceIp = "0.0.0.0";

        [ObservableProperty]
        private string _multicastIp = string.Empty;

        [ObservableProperty]
        private string _timestampFormat = "MMM dd HH:mm:ss";

        [ObservableProperty]
        private EncodingInfo? _selectedEncoding;

        [ObservableProperty]
        private ObservableCollection<EncodingInfo> _availableEncodings = new();

        public SyslogUdpReceiverSettingsViewModel()
        {
            // Load available encodings
            foreach (var encoding in Encoding.GetEncodings().OrderBy(e => e.DisplayName))
            {
                AvailableEncodings.Add(encoding);
            }

            // Default to UTF-8
            SelectedEncoding = AvailableEncodings.FirstOrDefault(e => e.CodePage == Encoding.UTF8.CodePage)
                ?? AvailableEncodings.FirstOrDefault();
        }

        /// <summary>
        /// Validates the entered settings.
        /// </summary>
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

            if (string.IsNullOrWhiteSpace(TimestampFormat))
            {
                return ValidationResult.Error("Please enter a timestamp format (e.g., 'MMM dd HH:mm:ss').");
            }

            return ValidationResult.Success;
        }

        /// <summary>
        /// Creates and returns a fully configured ILogProvider instance.
        /// </summary>
        public ILogProvider GetConfiguredInstance()
        {
            int codepage = SelectedEncoding?.CodePage ?? Encoding.UTF8.CodePage;

            IPAddress? multicast = string.IsNullOrWhiteSpace(MulticastIp)
                ? null
                : IPAddress.Parse(MulticastIp);

            IPAddress listenIp = IPAddress.Parse(ListenInterfaceIp);
            IPEndPoint listenEndPoint = new IPEndPoint(listenIp, Port);

            return new SyslogUdpReceiver(multicast, listenEndPoint, TimestampFormat, codepage);
        }

        public void Dispose()
        {
            // Nothing to dispose
        }
    }
}
