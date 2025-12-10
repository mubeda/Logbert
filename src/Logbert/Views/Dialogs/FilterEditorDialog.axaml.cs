using System;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Couchcoding.Logbert.Models;
using Couchcoding.Logbert.ViewModels.Dialogs;

namespace Couchcoding.Logbert.Views.Dialogs;

public partial class FilterEditorDialog : Window
{
    /// <summary>
    /// Gets the view model.
    /// </summary>
    public FilterEditorDialogViewModel ViewModel { get; }

    /// <summary>
    /// Gets the resulting filter rule after dialog closes.
    /// </summary>
    public FilterRule? ResultRule { get; private set; }

    public FilterEditorDialog()
    {
        InitializeComponent();

        ViewModel = new FilterEditorDialogViewModel();
        DataContext = ViewModel;

        // Add converters to resources
        Resources["BoolToBackgroundConverter"] = new BoolToBackgroundConverter();
        Resources["BoolToForegroundConverter"] = new BoolToForegroundConverter();
        Resources["BoolToIconConverter"] = new BoolToIconConverter();

        // Subscribe to ViewModel events
        ViewModel.Accepted += OnAccepted;
        ViewModel.Cancelled += OnCancelled;
    }

    /// <summary>
    /// Initializes the dialog for adding a new filter rule.
    /// </summary>
    public void InitializeForAdd()
    {
        ViewModel.InitializeForAdd();
    }

    /// <summary>
    /// Initializes the dialog for editing an existing filter rule.
    /// </summary>
    /// <param name="rule">The rule to edit.</param>
    public void InitializeForEdit(FilterRule rule)
    {
        ViewModel.InitializeForEdit(rule);
    }

    private void OnAccepted(object? sender, FilterRule rule)
    {
        ResultRule = rule;
        Close(true);
    }

    private void OnCancelled(object? sender, EventArgs e)
    {
        ResultRule = null;
        Close(false);
    }
}

/// <summary>
/// Converts a boolean to a background brush (green for success, red for error).
/// </summary>
public class BoolToBackgroundConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        if (value is bool isSuccess)
        {
            return isSuccess
                ? new SolidColorBrush(Color.FromRgb(232, 245, 233)) // Light green
                : new SolidColorBrush(Color.FromRgb(255, 235, 238)); // Light red
        }
        return new SolidColorBrush(Colors.Transparent);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}

/// <summary>
/// Converts a boolean to a foreground brush (green for success, red for error).
/// </summary>
public class BoolToForegroundConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        if (value is bool isSuccess)
        {
            return isSuccess
                ? new SolidColorBrush(Color.FromRgb(46, 125, 50)) // Dark green
                : new SolidColorBrush(Color.FromRgb(198, 40, 40)); // Dark red
        }
        return new SolidColorBrush(Colors.Gray);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}

/// <summary>
/// Converts a boolean to an icon path (checkmark for success, X for error).
/// </summary>
public class BoolToIconConverter : IValueConverter
{
    // Checkmark icon path
    private const string CheckIcon = "M9,20.42L2.79,14.21L5.62,11.38L9,14.77L18.88,4.88L21.71,7.71L9,20.42Z";
    // X icon path
    private const string XIcon = "M19,6.41L17.59,5L12,10.59L6.41,5L5,6.41L10.59,12L5,17.59L6.41,19L12,13.41L17.59,19L19,17.59L13.41,12L19,6.41Z";

    public object? Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        if (value is bool isSuccess)
        {
            return Geometry.Parse(isSuccess ? CheckIcon : XIcon);
        }
        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
