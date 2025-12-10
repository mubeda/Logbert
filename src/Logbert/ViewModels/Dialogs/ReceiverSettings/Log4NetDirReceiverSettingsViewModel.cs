using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Logbert.Interfaces;
using Logbert.Receiver;
using Logbert.Receiver.Log4NetDirReceiver;

namespace Logbert.ViewModels.Dialogs.ReceiverSettings
{
    /// <summary>
    /// ViewModel for Log4Net Directory Receiver configuration.
    /// </summary>
    public partial class Log4NetDirReceiverSettingsViewModel : ObservableObject, ILogSettingsCtrl
    {
        [ObservableProperty]
        private string _directoryPath = string.Empty;

        [ObservableProperty]
        private string _filenamePattern = "*.log";

        [ObservableProperty]
        private bool _startFromBeginning = false;

        [ObservableProperty]
        private EncodingInfo? _selectedEncoding;

        [ObservableProperty]
        private ObservableCollection<EncodingInfo> _availableEncodings = new();

        public Log4NetDirReceiverSettingsViewModel()
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
            if (string.IsNullOrWhiteSpace(DirectoryPath))
            {
                return ValidationResult.Error("Please select a log directory.");
            }

            if (!Directory.Exists(DirectoryPath))
            {
                return ValidationResult.Error($"The directory '{DirectoryPath}' does not exist.");
            }

            if (string.IsNullOrWhiteSpace(FilenamePattern))
            {
                return ValidationResult.Error("Please enter a filename pattern (e.g., *.log).");
            }

            return ValidationResult.Success;
        }

        /// <summary>
        /// Creates and returns a fully configured ILogProvider instance.
        /// </summary>
        public ILogProvider GetConfiguredInstance()
        {
            int codepage = SelectedEncoding?.CodePage ?? Encoding.UTF8.CodePage;

            return new Log4NetDirReceiver(
                DirectoryPath,
                FilenamePattern,
                StartFromBeginning,
                codepage);
        }

        public void Dispose()
        {
            // Nothing to dispose
        }
    }
}
