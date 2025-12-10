using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Logbert.Logging;

namespace Logbert.Converters;

/// <summary>
/// Converts a log level to a color brush.
/// </summary>
public class LogLevelToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is LogLevel level)
        {
            return level switch
            {
                LogLevel.Trace => new SolidColorBrush(Color.FromRgb(128, 128, 128)),
                LogLevel.Debug => new SolidColorBrush(Color.FromRgb(0, 0, 255)),
                LogLevel.Info => new SolidColorBrush(Color.FromRgb(0, 128, 0)),
                LogLevel.Warning => new SolidColorBrush(Color.FromRgb(255, 165, 0)),
                LogLevel.Error => new SolidColorBrush(Color.FromRgb(255, 0, 0)),
                LogLevel.Fatal => new SolidColorBrush(Color.FromRgb(139, 0, 0)),
                _ => new SolidColorBrush(Colors.Black)
            };
        }

        return new SolidColorBrush(Colors.Black);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
