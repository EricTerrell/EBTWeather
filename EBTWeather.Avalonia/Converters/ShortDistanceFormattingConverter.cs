using System;
using System.Globalization;
using Avalonia.Data.Converters;
using EBTWeather.Avalonia.UnitValues;

namespace EBTWeather.Avalonia.Converters;

public class ShortDistanceFormattingConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is ShortDistance shortDistance)
        {
            return ((int) Math.Round(double.Parse(shortDistance.ToString()))).ToString("N0");
        }
        
        return string.Empty;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null;
    }
}