using System;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Media;
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

        // Add converters to resources
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

    private void OnUsePatternClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (ViewModel.IsPatternValid)
        {
            PatternAccepted = true;
            ResultPattern = ViewModel.GetPattern();
            Close(true);
        }
    }

    private void OnCloseClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        PatternAccepted = false;
        ResultPattern = null;
        Close(false);
    }
}

/// <summary>
/// Converts a boolean to a background brush (green for valid/match, red for invalid/no match).
/// </summary>
public class BoolToBackgroundConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isSuccess)
        {
            return isSuccess
                ? new SolidColorBrush(Color.FromRgb(232, 245, 233)) // Light green
                : new SolidColorBrush(Color.FromRgb(255, 235, 238)); // Light red
        }
        return new SolidColorBrush(Colors.Transparent);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}

/// <summary>
/// Converts a boolean to a foreground brush (green for valid/match, red for invalid/no match).
/// </summary>
public class BoolToForegroundConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isSuccess)
        {
            return isSuccess
                ? new SolidColorBrush(Color.FromRgb(46, 125, 50)) // Dark green
                : new SolidColorBrush(Color.FromRgb(198, 40, 40)); // Dark red
        }
        return new SolidColorBrush(Colors.Gray);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}

/// <summary>
/// Converts a match result boolean to an appropriate background color.
/// </summary>
public class MatchBackgroundConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isMatch)
        {
            return isMatch
                ? new SolidColorBrush(Color.FromRgb(232, 245, 233)) // Light green for match
                : new SolidColorBrush(Color.FromRgb(255, 243, 224)); // Light orange for no match
        }
        return new SolidColorBrush(Colors.Transparent);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}

/// <summary>
/// Converts a match result boolean to a status text.
/// </summary>
public class MatchStatusConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isMatch)
        {
            return isMatch ? "Matched" : "No Match";
        }
        return "Unknown";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
