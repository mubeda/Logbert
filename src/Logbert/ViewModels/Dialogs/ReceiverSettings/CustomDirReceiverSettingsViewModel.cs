using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using Couchcoding.Logbert.Interfaces;
using Couchcoding.Logbert.Receiver;
using Couchcoding.Logbert.Receiver.CustomReceiver;
using Couchcoding.Logbert.Receiver.Log4NetDirReceiver;

namespace Couchcoding.Logbert.ViewModels.Dialogs.ReceiverSettings;

public partial class CustomDirReceiverSettingsViewModel : ViewModelBase, ILogSettingsCtrl
{
    [ObservableProperty]
    private string _directoryPath = string.Empty;

    [ObservableProperty]
    private string _filenamePattern = "*.log";

    [ObservableProperty]
    private bool _startFromBeginning;

    [ObservableProperty]
    private EncodingInfo? _selectedEncoding;

    public Columnizer? Columnizer { get; set; }

    public ObservableCollection<EncodingInfo> AvailableEncodings { get; } = new();

    public CustomDirReceiverSettingsViewModel()
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
        if (string.IsNullOrWhiteSpace(DirectoryPath))
        {
            return ValidationResult.Error("Please select a log directory.");
        }

        if (!Directory.Exists(DirectoryPath))
        {
            return ValidationResult.Error($"Directory not found: {DirectoryPath}");
        }

        if (string.IsNullOrWhiteSpace(FilenamePattern))
        {
            return ValidationResult.Error("Please enter a filename pattern.");
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
            return ValidationResult.Error("Please select a file encoding.");
        }

        return ValidationResult.Success;
    }

    public ILogProvider GetConfiguredInstance()
    {
        int codepage = SelectedEncoding?.CodePage ?? Encoding.UTF8.CodePage;
        return new CustomDirReceiver(DirectoryPath, FilenamePattern, StartFromBeginning, Columnizer!, codepage);
    }

    public void Dispose()
    {
        // No resources to dispose
    }
}
