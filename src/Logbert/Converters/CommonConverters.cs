using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace Logbert.Converters;

/// <summary>
/// Converts a boolean to a background brush (green for success/valid, red for error/invalid).
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
/// Converts a boolean to a foreground brush (green for success/valid, red for error/invalid).
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
/// Converts a boolean to an icon geometry path (checkmark for success, X for error).
/// </summary>
public class BoolToIconConverter : IValueConverter
{
    // Checkmark icon path
    private const string CheckIcon = "M9,20.42L2.79,14.21L5.62,11.38L9,14.77L18.88,4.88L21.71,7.71L9,20.42Z";
    // X icon path
    private const string XIcon = "M19,6.41L17.59,5L12,10.59L6.41,5L5,6.41L10.59,12L5,17.59L6.41,19L12,13.41L17.59,19L19,17.59L13.41,12L19,6.41Z";

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isSuccess)
        {
            return Geometry.Parse(isSuccess ? CheckIcon : XIcon);
        }
        return null;
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

/// <summary>
/// Converts a boolean to its inverse value.
/// </summary>
public class InverseBoolConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return !boolValue;
        }
        return false;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return !boolValue;
        }
        return false;
    }
}

/// <summary>
/// Converts a boolean to toggle button text for showing/hiding advanced filters.
/// </summary>
public class AdvancedToggleConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isExpanded)
        {
            return isExpanded ? "▼ Hide Advanced Filters" : "▶ Show Advanced Filters";
        }
        return "▶ Show Advanced Filters";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}

/// <summary>
/// Converts a numeric value to a boolean (true if zero, false otherwise).
/// Used for showing empty state messages.
/// </summary>
public class ZeroToBoolConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int intValue)
        {
            return intValue == 0;
        }
        return false;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
