using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace EBTWeather.Avalonia.Converters;

public class BooleanToIntConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool booleanValue)
        {
            if (parameter is "Invert")
            {
                booleanValue = !booleanValue;
            }
            
            return booleanValue ? 1 : 0;
        }
        
        return 0;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null;
    }
}
