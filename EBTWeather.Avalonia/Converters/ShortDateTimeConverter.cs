using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace EBTWeather.Avalonia.Converters;

public class ShortDateTimeConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        const string formatString = "M/dd hh:mm tt";
        
        if (value is DateOnly dateOnly)
        {
            return dateOnly.ToString(formatString);
        }
        else if (value is DateTime dateTime)
        {
            return dateTime.ToString(formatString);
        }
        
        return string.Empty;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null;
    }
}
