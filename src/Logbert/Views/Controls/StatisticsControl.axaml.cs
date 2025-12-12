using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Logbert.ViewModels.Controls;
using System.Globalization;

namespace Logbert.Views.Controls;

public partial class StatisticsControl : UserControl
{
    public StatisticsControl()
    {
        InitializeComponent();
        DataContext = new StatisticsViewModel();

        // Add converters to resources
        Resources["PercentageToWidthConverter"] = new PercentageToWidthConverter();
        Resources["StringToColorConverter"] = new StringToColorConverter();
    }
}

/// <summary>
/// Converts a percentage (0-100) to a width value for the progress bar.
/// </summary>
public class PercentageToWidthConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double percentage)
        {
            // Maximum width is 300 pixels
            return percentage * 3.0;
        }
        return 0.0;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converts a hex color string to a Color.
/// </summary>
public class StringToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string colorString && !string.IsNullOrEmpty(colorString))
        {
            try
            {
                return Color.Parse(colorString);
            }
            catch
            {
                return Colors.Black;
            }
        }
        return Colors.Black;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
