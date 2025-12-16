using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Logbert.Interfaces;
using Logbert.Receiver;
using Logbert.Receiver.NLogDirReceiver;

namespace Logbert.ViewModels.Dialogs.ReceiverSettings
{
    /// <summary>
    /// ViewModel for NLog Directory Receiver configuration.
    /// </summary>
    public partial class NLogDirReceiverSettingsViewModel : ObservableObject, ILogSettingsCtrl
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

        public NLogDirReceiverSettingsViewModel()
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

            return new NLogDirReceiver(
                DirectoryPath,
                FilenamePattern,
                StartFromBeginning,
                codepage);
        }

        /// <summary>
        /// Gets a display string for the receiver configuration.
        /// </summary>
        public string GetDisplayInfo()
        {
            return string.IsNullOrEmpty(DirectoryPath) ? "New" : System.IO.Path.GetFileName(DirectoryPath);
        }

        public void Dispose()
        {
            // Nothing to dispose
        }
    }
}
