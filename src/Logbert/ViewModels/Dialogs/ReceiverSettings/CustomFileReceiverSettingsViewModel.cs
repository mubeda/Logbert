using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using Logbert.Interfaces;
using Logbert.Receiver;
using Logbert.Receiver.CustomReceiver;
using Logbert.Receiver.CustomReceiver.CustomFileReceiver;

namespace Logbert.ViewModels.Dialogs.ReceiverSettings;

public partial class CustomFileReceiverSettingsViewModel : ViewModelBase, ILogSettingsCtrl
{
    [ObservableProperty]
    private string _filePath = string.Empty;

    [ObservableProperty]
    private bool _startFromBeginning;

    [ObservableProperty]
    private EncodingInfo? _selectedEncoding;

    public Columnizer? Columnizer { get; set; }

    public ObservableCollection<EncodingInfo> AvailableEncodings { get; } = new();

    public CustomFileReceiverSettingsViewModel()
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
        if (string.IsNullOrWhiteSpace(FilePath))
        {
            return ValidationResult.Error("Please select a log file.");
        }

        if (!File.Exists(FilePath))
        {
            return ValidationResult.Error($"File not found: {FilePath}");
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
        return new CustomFileReceiver(FilePath, StartFromBeginning, Columnizer!, codepage);
    }

    public void Dispose()
    {
        // No resources to dispose
    }
}
