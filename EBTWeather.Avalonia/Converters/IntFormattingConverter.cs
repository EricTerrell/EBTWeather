using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace EBTWeather.Avalonia.Converters;

public class IntFormattingConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double doubleValue)
        {
            return ((int) Math.Round(doubleValue)).ToString("N0");
        }
        
        return string.Empty;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null;
    }
}
