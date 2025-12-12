using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using Logbert.Logging;
using Logbert.Services;

namespace Logbert.ViewModels.Dialogs;

/// <summary>
/// ViewModel for the Export dialog.
/// </summary>
public partial class ExportDialogViewModel : ViewModelBase
{
    private IReadOnlyList<LogMessage>? _allMessages;
    private IReadOnlyList<LogMessage>? _filteredMessages;

    /// <summary>
    /// Gets the available export scopes.
    /// </summary>
    public ObservableCollection<string> ExportScopes { get; } = new()
    {
        "All Messages",
        "Filtered Messages Only"
    };

    /// <summary>
    /// Gets the available export formats.
    /// </summary>
    public ObservableCollection<string> ExportFormats { get; } = new()
    {
        "CSV (Comma Separated Values)",
        "Text (Plain Text)"
    };

    /// <summary>
    /// Gets the available encodings.
    /// </summary>
    public ObservableCollection<EncodingItem> Encodings { get; } = new();

    /// <summary>
    /// Gets or sets the selected export scope.
    /// </summary>
    [ObservableProperty]
    private string _selectedScope = "All Messages";

    /// <summary>
    /// Gets or sets the selected export format.
    /// </summary>
    [ObservableProperty]
    private string _selectedFormat = "CSV (Comma Separated Values)";

    /// <summary>
    /// Gets or sets the selected encoding.
    /// </summary>
    [ObservableProperty]
    private EncodingItem? _selectedEncoding;

    /// <summary>
    /// Gets or sets whether to include headers in the export.
    /// </summary>
    [ObservableProperty]
    private bool _includeHeaders = true;

    /// <summary>
    /// Gets or sets the export file path.
    /// </summary>
    [ObservableProperty]
    private string _filePath = string.Empty;

    /// <summary>
    /// Gets or sets whether an export is in progress.
    /// </summary>
    [ObservableProperty]
    private bool _isExporting;

    /// <summary>
    /// Gets or sets the export progress percentage.
    /// </summary>
    [ObservableProperty]
    private double _progressPercentage;

    /// <summary>
    /// Gets or sets the progress status text.
    /// </summary>
    [ObservableProperty]
    private string _progressStatus = string.Empty;

    /// <summary>
    /// Gets the total message count for "All Messages" scope.
    /// </summary>
    [ObservableProperty]
    private int _allMessageCount;

    /// <summary>
    /// Gets the filtered message count.
    /// </summary>
    [ObservableProperty]
    private int _filteredMessageCount;

    /// <summary>
    /// Gets whether the export can proceed.
    /// </summary>
    public bool CanExport => !string.IsNullOrEmpty(FilePath) && !IsExporting && GetMessagesToExport().Count > 0;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExportDialogViewModel"/> class.
    /// </summary>
    public ExportDialogViewModel()
    {
        // Populate encodings
        Encodings.Add(new EncodingItem("UTF-8", Encoding.UTF8));
        Encodings.Add(new EncodingItem("UTF-8 with BOM", new UTF8Encoding(true)));
        Encodings.Add(new EncodingItem("UTF-16 (Unicode)", Encoding.Unicode));
        Encodings.Add(new EncodingItem("UTF-16 BE", Encoding.BigEndianUnicode));
        Encodings.Add(new EncodingItem("ASCII", Encoding.ASCII));
        Encodings.Add(new EncodingItem("ISO-8859-1 (Latin-1)", Encoding.Latin1));

        SelectedEncoding = Encodings.First();
    }

    /// <summary>
    /// Sets the messages to be exported.
    /// </summary>
    /// <param name="allMessages">All log messages.</param>
    /// <param name="filteredMessages">Currently filtered/visible messages.</param>
    public void SetMessages(IReadOnlyList<LogMessage> allMessages, IReadOnlyList<LogMessage>? filteredMessages = null)
    {
        _allMessages = allMessages;
        _filteredMessages = filteredMessages ?? allMessages;

        AllMessageCount = _allMessages.Count;
        FilteredMessageCount = _filteredMessages.Count;
    }

    /// <summary>
    /// Gets the messages to export based on the selected scope.
    /// </summary>
    /// <returns>The list of messages to export.</returns>
    public IReadOnlyList<LogMessage> GetMessagesToExport()
    {
        if (SelectedScope == "Filtered Messages Only" && _filteredMessages != null)
        {
            return _filteredMessages;
        }

        return _allMessages ?? Array.Empty<LogMessage>();
    }

    /// <summary>
    /// Gets the selected export format enum value.
    /// </summary>
    /// <returns>The export format.</returns>
    public ExportService.ExportFormat GetExportFormat()
    {
        return SelectedFormat.StartsWith("CSV") ? ExportService.ExportFormat.Csv : ExportService.ExportFormat.Text;
    }

    /// <summary>
    /// Gets the default file extension for the selected format.
    /// </summary>
    /// <returns>The file extension including the dot.</returns>
    public string GetDefaultExtension()
    {
        return GetExportFormat() == ExportService.ExportFormat.Csv ? ".csv" : ".txt";
    }

    partial void OnFilePathChanged(string value)
    {
        OnPropertyChanged(nameof(CanExport));
    }

    partial void OnIsExportingChanged(bool value)
    {
        OnPropertyChanged(nameof(CanExport));
    }

    partial void OnSelectedScopeChanged(string value)
    {
        OnPropertyChanged(nameof(CanExport));
    }
}

/// <summary>
/// Represents an encoding option for the export dialog.
/// </summary>
public class EncodingItem
{
    /// <summary>
    /// Gets the display name of the encoding.
    /// </summary>
    public string DisplayName { get; }

    /// <summary>
    /// Gets the encoding instance.
    /// </summary>
    public Encoding Encoding { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EncodingItem"/> class.
    /// </summary>
    /// <param name="displayName">The display name.</param>
    /// <param name="encoding">The encoding instance.</param>
    public EncodingItem(string displayName, Encoding encoding)
    {
        DisplayName = displayName;
        Encoding = encoding;
    }

    public override string ToString() => DisplayName;
}
