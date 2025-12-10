using System;
using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Couchcoding.Logbert.ViewModels.Dialogs;

/// <summary>
/// ViewModel for the Timestamp Format dialog.
/// </summary>
public partial class TimestampFormatDialogViewModel : ViewModelBase
{
    #region Observable Properties

    /// <summary>
    /// Gets or sets the custom format string.
    /// </summary>
    [ObservableProperty]
    private string _formatString = "yyyy-MM-dd HH:mm:ss.fff";

    /// <summary>
    /// Gets or sets the selected preset index.
    /// </summary>
    [ObservableProperty]
    private int _selectedPresetIndex = 0;

    /// <summary>
    /// Gets or sets the preview result.
    /// </summary>
    [ObservableProperty]
    private string _previewResult = string.Empty;

    /// <summary>
    /// Gets or sets whether the format is valid.
    /// </summary>
    [ObservableProperty]
    private bool _isFormatValid = true;

    /// <summary>
    /// Gets or sets the validation message.
    /// </summary>
    [ObservableProperty]
    private string? _validationMessage;

    /// <summary>
    /// Gets or sets the sample timestamp text for parsing test.
    /// </summary>
    [ObservableProperty]
    private string _sampleTimestamp = string.Empty;

    /// <summary>
    /// Gets or sets the parsed result from sample.
    /// </summary>
    [ObservableProperty]
    private string _parsedResult = string.Empty;

    /// <summary>
    /// Gets or sets whether the sample was parsed successfully.
    /// </summary>
    [ObservableProperty]
    private bool _sampleParsed;

    /// <summary>
    /// Gets or sets the selected timezone.
    /// </summary>
    [ObservableProperty]
    private string _selectedTimezone = "Local";

    #endregion

    #region Collections

    /// <summary>
    /// Gets the available format presets.
    /// </summary>
    public ObservableCollection<TimestampFormatPreset> FormatPresets { get; } = new()
    {
        new TimestampFormatPreset("Default", "yyyy-MM-dd HH:mm:ss.fff"),
        new TimestampFormatPreset("ISO 8601", "yyyy-MM-ddTHH:mm:ss.fffZ"),
        new TimestampFormatPreset("ISO 8601 (no ms)", "yyyy-MM-ddTHH:mm:ssZ"),
        new TimestampFormatPreset("US Format", "MM/dd/yyyy HH:mm:ss"),
        new TimestampFormatPreset("European Format", "dd/MM/yyyy HH:mm:ss"),
        new TimestampFormatPreset("Syslog BSD", "MMM dd HH:mm:ss"),
        new TimestampFormatPreset("Syslog BSD (year)", "MMM dd yyyy HH:mm:ss"),
        new TimestampFormatPreset("Log4j", "yyyy-MM-dd HH:mm:ss,fff"),
        new TimestampFormatPreset("Time Only", "HH:mm:ss.fff"),
        new TimestampFormatPreset("Time Only (no ms)", "HH:mm:ss"),
        new TimestampFormatPreset("Unix Epoch", "Unix Timestamp"),
        new TimestampFormatPreset("Custom", "")
    };

    /// <summary>
    /// Gets the available timezones.
    /// </summary>
    public ObservableCollection<string> Timezones { get; } = new()
    {
        "Local",
        "UTC",
        "UTC+1",
        "UTC+2",
        "UTC+3",
        "UTC+4",
        "UTC+5",
        "UTC+6",
        "UTC+7",
        "UTC+8",
        "UTC+9",
        "UTC+10",
        "UTC+11",
        "UTC+12",
        "UTC-1",
        "UTC-2",
        "UTC-3",
        "UTC-4",
        "UTC-5",
        "UTC-6",
        "UTC-7",
        "UTC-8",
        "UTC-9",
        "UTC-10",
        "UTC-11",
        "UTC-12"
    };

    #endregion

    #region Commands

    public IRelayCommand TestParseCommand { get; }
    public IRelayCommand InsertTokenCommand { get; }

    #endregion

    #region Events

    /// <summary>
    /// Raised when the dialog is accepted.
    /// </summary>
    public event EventHandler<string>? Accepted;

    /// <summary>
    /// Raised when the dialog is cancelled.
    /// </summary>
    public event EventHandler? Cancelled;

    #endregion

    #region Constructor

    public TimestampFormatDialogViewModel()
    {
        TestParseCommand = new RelayCommand(OnTestParse);
        InsertTokenCommand = new RelayCommand<string>(OnInsertToken);

        // Subscribe to property changes for live preview
        PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(FormatString) || e.PropertyName == nameof(SelectedTimezone))
            {
                UpdatePreview();
                ValidateFormat();
            }

            if (e.PropertyName == nameof(SelectedPresetIndex))
            {
                ApplyPreset();
            }
        };

        // Initialize preview
        UpdatePreview();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Initializes the dialog with an existing format string.
    /// </summary>
    public void Initialize(string? existingFormat = null)
    {
        if (!string.IsNullOrWhiteSpace(existingFormat))
        {
            FormatString = existingFormat;

            // Try to find matching preset
            var presetIndex = -1;
            for (int i = 0; i < FormatPresets.Count; i++)
            {
                if (FormatPresets[i].Format == existingFormat)
                {
                    presetIndex = i;
                    break;
                }
            }

            SelectedPresetIndex = presetIndex >= 0 ? presetIndex : FormatPresets.Count - 1; // Custom
        }

        // Set sample timestamp
        SampleTimestamp = DateTime.Now.ToString(FormatString, CultureInfo.InvariantCulture);

        UpdatePreview();
        ValidateFormat();
    }

    /// <summary>
    /// Gets the resulting format string.
    /// </summary>
    public string GetFormatString() => FormatString;

    /// <summary>
    /// Accepts the dialog with the current format.
    /// </summary>
    public void Accept()
    {
        if (IsFormatValid)
        {
            Accepted?.Invoke(this, FormatString);
        }
    }

    /// <summary>
    /// Cancels the dialog.
    /// </summary>
    public void Cancel()
    {
        Cancelled?.Invoke(this, EventArgs.Empty);
    }

    #endregion

    #region Private Methods

    private void UpdatePreview()
    {
        try
        {
            var now = GetCurrentTimeWithTimezone();

            if (FormatString == "Unix Timestamp")
            {
                var unixTime = ((DateTimeOffset)now).ToUnixTimeSeconds();
                PreviewResult = unixTime.ToString();
            }
            else
            {
                PreviewResult = now.ToString(FormatString, CultureInfo.InvariantCulture);
            }
        }
        catch (Exception ex)
        {
            PreviewResult = $"Error: {ex.Message}";
        }
    }

    private void ValidateFormat()
    {
        if (string.IsNullOrWhiteSpace(FormatString))
        {
            IsFormatValid = false;
            ValidationMessage = "Format string cannot be empty.";
            return;
        }

        if (FormatString == "Unix Timestamp")
        {
            IsFormatValid = true;
            ValidationMessage = "Valid format (Unix epoch timestamp).";
            return;
        }

        try
        {
            // Try to format current time with the format string
            DateTime.Now.ToString(FormatString, CultureInfo.InvariantCulture);
            IsFormatValid = true;
            ValidationMessage = "Valid format string.";
        }
        catch (FormatException ex)
        {
            IsFormatValid = false;
            ValidationMessage = $"Invalid format: {ex.Message}";
        }
    }

    private void ApplyPreset()
    {
        if (SelectedPresetIndex >= 0 && SelectedPresetIndex < FormatPresets.Count)
        {
            var preset = FormatPresets[SelectedPresetIndex];
            if (!string.IsNullOrEmpty(preset.Format))
            {
                FormatString = preset.Format;
            }
        }
    }

    private void OnTestParse()
    {
        if (string.IsNullOrWhiteSpace(SampleTimestamp))
        {
            ParsedResult = "Enter a sample timestamp to test.";
            SampleParsed = false;
            return;
        }

        try
        {
            DateTime parsed;

            if (FormatString == "Unix Timestamp")
            {
                if (long.TryParse(SampleTimestamp.Trim(), out var unixTime))
                {
                    parsed = DateTimeOffset.FromUnixTimeSeconds(unixTime).DateTime;
                    ParsedResult = $"Parsed: {parsed:yyyy-MM-dd HH:mm:ss.fff} (from Unix: {unixTime})";
                    SampleParsed = true;
                }
                else
                {
                    ParsedResult = "Failed to parse as Unix timestamp.";
                    SampleParsed = false;
                }
            }
            else
            {
                if (DateTime.TryParseExact(SampleTimestamp.Trim(), FormatString,
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
                {
                    ParsedResult = $"Parsed: {parsed:yyyy-MM-dd HH:mm:ss.fff}";
                    SampleParsed = true;
                }
                else
                {
                    // Try with more flexible parsing
                    if (DateTime.TryParse(SampleTimestamp.Trim(), CultureInfo.InvariantCulture,
                        DateTimeStyles.None, out parsed))
                    {
                        ParsedResult = $"Parsed (flexible): {parsed:yyyy-MM-dd HH:mm:ss.fff}";
                        SampleParsed = true;
                    }
                    else
                    {
                        ParsedResult = $"Failed to parse with format: {FormatString}";
                        SampleParsed = false;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ParsedResult = $"Parse error: {ex.Message}";
            SampleParsed = false;
        }
    }

    private void OnInsertToken(string? token)
    {
        if (!string.IsNullOrEmpty(token))
        {
            FormatString += token;
        }
    }

    private DateTime GetCurrentTimeWithTimezone()
    {
        var now = DateTime.Now;

        if (SelectedTimezone == "UTC")
        {
            return DateTime.UtcNow;
        }

        if (SelectedTimezone.StartsWith("UTC+") || SelectedTimezone.StartsWith("UTC-"))
        {
            var offset = SelectedTimezone.StartsWith("UTC+")
                ? int.Parse(SelectedTimezone.Substring(4))
                : -int.Parse(SelectedTimezone.Substring(4));

            return DateTime.UtcNow.AddHours(offset);
        }

        return now;
    }

    #endregion
}

/// <summary>
/// Represents a timestamp format preset.
/// </summary>
public record TimestampFormatPreset(string Name, string Format)
{
    public override string ToString() => Name;
}
