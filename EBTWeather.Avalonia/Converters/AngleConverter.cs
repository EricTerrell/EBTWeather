using System;
using System.Globalization;
using System.Linq;
using Avalonia.Data.Converters;

namespace EBTWeather.Avalonia.Converters;

public class AngleConverter : IValueConverter
{
    private record AngleInfo(string Abbreviation, double Min,  double Max);

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int angle)
        {
            AngleInfo[] angleInfos = 
            [
                new("N",  337.5, 360.0), 
                new("N",    0.0,  22.5),
                new("NE",  22.5,  67.5),
                new("E",   67.5, 112.5),
                new("SE", 112.5, 157.5),
                new("S",  157.5, 202.5),
                new("SW", 202.5, 247.5),
                new("W",  247.5, 292.5),
                new("NW", 292.5, 337.5)            
            ];

            return angleInfos
                .ToList()
                .FirstOrDefault(angleInfo => angle >= angleInfo.Min && angle < angleInfo.Max, 
                    new AngleInfo(string.Empty, 0, 0)).Abbreviation;
        }
        
        return string.Empty;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null;
    }
}
