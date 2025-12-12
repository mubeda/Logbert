using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using System.Globalization;
using Logbert.ViewModels.Controls;

namespace Logbert.Views.Controls;

public partial class ColorMapControl : UserControl
{
    public ColorMapControl()
    {
        InitializeComponent();
        DataContext = new ColorMapViewModel();

        // Add converter to resources
        Resources["PositionToTopConverter"] = new PositionToTopConverter();
    }
}

/// <summary>
/// Converts a position (0.0 to 1.0) to a canvas top coordinate.
/// </summary>
public class PositionToTopConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double position)
        {
            // Default height of 600, this will be overridden by actual control height
            return position * 600;
        }
        return 0;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
