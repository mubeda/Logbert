using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using Logbert.Interfaces;
using Logbert.Receiver;
using Logbert.Receiver.CustomReceiver;
using Logbert.Receiver.CustomReceiver.CustomHttpReceiver;

namespace Logbert.ViewModels.Dialogs.ReceiverSettings;

public partial class CustomHttpReceiverSettingsViewModel : ViewModelBase, ILogSettingsCtrl
{
    [ObservableProperty]
    private string _url = string.Empty;

    [ObservableProperty]
    private int _pollIntervalSeconds = 30;

    [ObservableProperty]
    private bool _useAuthentication;

    [ObservableProperty]
    private string _username = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private EncodingInfo? _selectedEncoding;

    public Columnizer? Columnizer { get; set; }

    public ObservableCollection<EncodingInfo> AvailableEncodings { get; } = new();

    public CustomHttpReceiverSettingsViewModel()
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
        if (string.IsNullOrWhiteSpace(Url))
        {
            return ValidationResult.Error("Please enter a URL.");
        }

        if (!Uri.TryCreate(Url, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            return ValidationResult.Error($"'{Url}' is not a valid HTTP or HTTPS URL.");
        }

        if (PollIntervalSeconds < 1 || PollIntervalSeconds > 3600)
        {
            return ValidationResult.Error("Poll interval must be between 1 and 3600 seconds.");
        }

        if (UseAuthentication)
        {
            if (string.IsNullOrWhiteSpace(Username))
            {
                return ValidationResult.Error("Username is required when authentication is enabled.");
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                return ValidationResult.Error("Password is required when authentication is enabled.");
            }
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
            return ValidationResult.Error("Please select an encoding.");
        }

        return ValidationResult.Success;
    }

    public ILogProvider GetConfiguredInstance()
    {
        BasicHttpAuthentication? authentication = UseAuthentication
            ? new BasicHttpAuthentication(Username, Password)
            : null;

        int codepage = SelectedEncoding?.CodePage ?? Encoding.UTF8.CodePage;

        return new CustomHttpReceiver(Url, authentication!, PollIntervalSeconds, Columnizer!, codepage);
    }

    public void Dispose()
    {
        // No resources to dispose
    }
}
