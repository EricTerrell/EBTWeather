using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace EBTWeather.Avalonia.Converters;

public class StringToDmsConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double doubleValue)
        {
            var angle = doubleValue;
            var sign =  Math.Sign(angle);
            
            angle = Math.Abs(angle);
            
            var degrees = Math.Truncate(angle);
            angle -= degrees;
            
            var minutes = Math.Truncate(angle * 60);
            angle -= minutes / 60;
            
            var seconds = angle * (60 * 60);
            var secondsInt = Math.Truncate(seconds);
            var secondsFraction = seconds - secondsInt;

            // Avoid secondsFraction rounding to 1.00.
            var secondsFractionDigits = (int) Math.Truncate(secondsFraction * 1000);
            
            var direction = parameter switch
            {
                "latitude"  => sign < 0 ? "S" : "N",
                "longitude" => sign < 0 ? "W" : "E",
                _ => string.Empty
            };

            var degreesString = parameter switch
            {
                "latitude"  => $"{(int) degrees,2}",
                "longitude" => $"{(int) degrees,3}",
                _ => string.Empty
            };
            
            return $"{degreesString}°{minutes:00}'{secondsInt:00}.{secondsFractionDigits:000}\" {direction}";
        }
        
        return string.Empty;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null;
    }
}
