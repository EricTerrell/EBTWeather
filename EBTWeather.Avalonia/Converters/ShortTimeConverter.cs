using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace EBTWeather.Avalonia.Converters;

public class ShortTimeConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string dateTimeString)
        {
            return DateTime.Parse(dateTimeString).ToShortTimeString();
        } else if (value is DateTime dateTime)
        {
            return dateTime.ToShortTimeString();
        }
        
        return string.Empty;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null;
    }
}
