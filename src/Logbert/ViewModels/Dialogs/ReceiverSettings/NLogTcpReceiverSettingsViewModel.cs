using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using Couchcoding.Logbert.Interfaces;
using Couchcoding.Logbert.Receiver;
using Couchcoding.Logbert.Receiver.NlogTcpReceiver;

namespace Couchcoding.Logbert.ViewModels.Dialogs.ReceiverSettings
{
    /// <summary>
    /// ViewModel for NLog TCP Receiver configuration.
    /// </summary>
    public partial class NLogTcpReceiverSettingsViewModel : ObservableObject, ILogSettingsCtrl
    {
        [ObservableProperty]
        private int _port = 4505;

        [ObservableProperty]
        private string _listenInterfaceIp = "0.0.0.0";

        [ObservableProperty]
        private EncodingInfo? _selectedEncoding;

        [ObservableProperty]
        private ObservableCollection<EncodingInfo> _availableEncodings = new();

        public NLogTcpReceiverSettingsViewModel()
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

            return ValidationResult.Success;
        }

        /// <summary>
        /// Creates and returns a fully configured ILogProvider instance.
        /// </summary>
        public ILogProvider GetConfiguredInstance()
        {
            int codepage = SelectedEncoding?.CodePage ?? Encoding.UTF8.CodePage;

            IPAddress listenIp = IPAddress.Parse(ListenInterfaceIp);
            IPEndPoint listenEndPoint = new IPEndPoint(listenIp, Port);

            return new NlogTcpReceiver(Port, listenEndPoint, codepage);
        }

        public void Dispose()
        {
            // Nothing to dispose
        }
    }
}
