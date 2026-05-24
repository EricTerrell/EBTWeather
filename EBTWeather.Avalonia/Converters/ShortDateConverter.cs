using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace EBTWeather.Avalonia.Converters;

public class ShortDateConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        const string formatString = "M/dd (ddd)";
        
        if (value is DateOnly dateOnly)
        {
            return dateOnly.ToString(formatString);
        }
        else if (value is DateTime dateTime)
        {
            return dateTime.ToLocalTime().ToString(formatString);
        }
        
        return string.Empty;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null;
    }
}
