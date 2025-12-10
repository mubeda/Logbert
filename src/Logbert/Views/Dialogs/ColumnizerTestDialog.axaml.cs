using Avalonia.Controls;
using Avalonia.Interactivity;
using Couchcoding.Logbert.Converters;
using Couchcoding.Logbert.ViewModels.Dialogs;

namespace Couchcoding.Logbert.Views.Dialogs;

/// <summary>
/// Code-behind for the Columnizer Test Dialog.
/// </summary>
public partial class ColumnizerTestDialog : Window
{
    /// <summary>
    /// Gets the view model.
    /// </summary>
    public ColumnizerTestDialogViewModel ViewModel { get; }

    /// <summary>
    /// Gets whether the user accepted the pattern.
    /// </summary>
    public bool PatternAccepted { get; private set; }

    /// <summary>
    /// Gets the resulting pattern if accepted.
    /// </summary>
    public string? ResultPattern { get; private set; }

    public ColumnizerTestDialog()
    {
        InitializeComponent();

        ViewModel = new ColumnizerTestDialogViewModel();
        DataContext = ViewModel;

        // Add converters to resources (using shared converters)
        Resources["BoolToBackgroundConverter"] = new BoolToBackgroundConverter();
        Resources["BoolToForegroundConverter"] = new BoolToForegroundConverter();
        Resources["MatchBackgroundConverter"] = new MatchBackgroundConverter();
        Resources["MatchStatusConverter"] = new MatchStatusConverter();
    }

    /// <summary>
    /// Initializes the dialog with an optional existing pattern.
    /// </summary>
    /// <param name="existingPattern">The existing pattern to test, if any.</param>
    public void Initialize(string? existingPattern = null)
    {
        ViewModel.Initialize(existingPattern);
    }

    private void OnUsePatternClick(object? sender, RoutedEventArgs e)
    {
        if (ViewModel.IsPatternValid)
        {
            PatternAccepted = true;
            ResultPattern = ViewModel.GetPattern();
            Close(true);
        }
    }

    private void OnCloseClick(object? sender, RoutedEventArgs e)
    {
        PatternAccepted = false;
        ResultPattern = null;
        Close(false);
    }
}
