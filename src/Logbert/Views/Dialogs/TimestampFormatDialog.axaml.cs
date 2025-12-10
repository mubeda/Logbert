using Avalonia.Controls;
using Avalonia.Interactivity;
using Couchcoding.Logbert.Converters;
using Couchcoding.Logbert.ViewModels.Dialogs;

namespace Couchcoding.Logbert.Views.Dialogs;

/// <summary>
/// Code-behind for the Timestamp Format Dialog.
/// </summary>
public partial class TimestampFormatDialog : Window
{
    /// <summary>
    /// Gets the view model.
    /// </summary>
    public TimestampFormatDialogViewModel ViewModel { get; }

    /// <summary>
    /// Gets whether the user accepted the dialog.
    /// </summary>
    public bool FormatAccepted { get; private set; }

    /// <summary>
    /// Gets the resulting format string if accepted.
    /// </summary>
    public string? ResultFormat { get; private set; }

    public TimestampFormatDialog()
    {
        InitializeComponent();

        ViewModel = new TimestampFormatDialogViewModel();
        DataContext = ViewModel;

        // Add converters to resources (using shared converters)
        Resources["BoolToBackgroundConverter"] = new BoolToBackgroundConverter();
        Resources["BoolToForegroundConverter"] = new BoolToForegroundConverter();

        // Subscribe to ViewModel events
        ViewModel.Accepted += OnAccepted;
        ViewModel.Cancelled += OnCancelled;
    }

    /// <summary>
    /// Initializes the dialog with an optional existing format string.
    /// </summary>
    /// <param name="existingFormat">The existing format string, if any.</param>
    public void Initialize(string? existingFormat = null)
    {
        ViewModel.Initialize(existingFormat);
    }

    private void OnOkClick(object? sender, RoutedEventArgs e)
    {
        if (ViewModel.IsFormatValid)
        {
            FormatAccepted = true;
            ResultFormat = ViewModel.GetFormatString();
            Close(true);
        }
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e)
    {
        FormatAccepted = false;
        ResultFormat = null;
        Close(false);
    }

    private void OnAccepted(object? sender, string format)
    {
        FormatAccepted = true;
        ResultFormat = format;
        Close(true);
    }

    private void OnCancelled(object? sender, System.EventArgs e)
    {
        FormatAccepted = false;
        ResultFormat = null;
        Close(false);
    }
}
