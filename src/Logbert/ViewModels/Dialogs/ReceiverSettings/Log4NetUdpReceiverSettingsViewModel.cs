using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using Logbert.Interfaces;
using Logbert.Receiver;
using Logbert.Receiver.Log4NetUdpReceiver;

namespace Logbert.ViewModels.Dialogs.ReceiverSettings
{
    /// <summary>
    /// ViewModel for Log4Net UDP Receiver configuration.
    /// </summary>
    public partial class Log4NetUdpReceiverSettingsViewModel : ObservableObject, ILogSettingsCtrl
    {
        [ObservableProperty]
        private int _port = 8080;

        [ObservableProperty]
        private string _listenInterfaceIp = "0.0.0.0";

        [ObservableProperty]
        private string _multicastIp = string.Empty;

        [ObservableProperty]
        private EncodingInfo? _selectedEncoding;

        [ObservableProperty]
        private ObservableCollection<EncodingInfo> _availableEncodings = new();

        public Log4NetUdpReceiverSettingsViewModel()
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

            return new Log4NetUdpReceiver(multicast, listenEndPoint, codepage);
        }

        /// <summary>
        /// Gets a display string for the receiver configuration.
        /// </summary>
        public string GetDisplayInfo()
        {
            return $"Port {Port}";
        }

        public void Dispose()
        {
            // Nothing to dispose
        }
    }
}
