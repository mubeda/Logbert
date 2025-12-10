using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Logbert.Interfaces;
using Logbert.Receiver;
using Logbert.Receiver.NLogFileReceiver;

namespace Logbert.ViewModels.Dialogs.ReceiverSettings
{
    /// <summary>
    /// ViewModel for NLog File Receiver configuration.
    /// </summary>
    public partial class NLogFileReceiverSettingsViewModel : ObservableObject, ILogSettingsCtrl
    {
        [ObservableProperty]
        private string _filePath = string.Empty;

        [ObservableProperty]
        private bool _startFromBeginning = false;

        [ObservableProperty]
        private EncodingInfo? _selectedEncoding;

        [ObservableProperty]
        private ObservableCollection<EncodingInfo> _availableEncodings = new();

        public NLogFileReceiverSettingsViewModel()
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
            if (string.IsNullOrWhiteSpace(FilePath))
            {
                return ValidationResult.Error("Please select a log file.");
            }

            if (!File.Exists(FilePath))
            {
                return ValidationResult.Error($"The file '{FilePath}' does not exist.");
            }

            return ValidationResult.Success;
        }

        /// <summary>
        /// Creates and returns a fully configured ILogProvider instance.
        /// </summary>
        public ILogProvider GetConfiguredInstance()
        {
            int codepage = SelectedEncoding?.CodePage ?? Encoding.UTF8.CodePage;

            return new NLogFileReceiver(
                FilePath,
                StartFromBeginning,
                codepage);
        }

        public void Dispose()
        {
            // Nothing to dispose
        }
    }
}
