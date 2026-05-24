using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using Avalonia.Styling;
using EBTWeather.Avalonia.Misc;

namespace EBTWeather.Avalonia.Converters;

public class DateToBackgroundColorConverter : IValueConverter
{
    private static readonly ImmutableSolidColorBrush GreyBrush = 
        new (Color.FromArgb(0xff, 0x30, 0x30, 0x30));

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DateTime dateTime)
        {
            if (Application.Current is App app)
            {
                IImmutableSolidColorBrush todayBrush, tomorrowBrush;
                
                if (app.ActualThemeVariant == ThemeVariant.Dark)
                {
                    todayBrush = Brushes.Black;
                    tomorrowBrush = GreyBrush;
                }
                else
                {
                    todayBrush = Brushes.White;
                    tomorrowBrush = Brushes.LightGray;
                }
                
                return DateTimeUtils.IsToday(dateTime.ToLocalTime()) ? todayBrush : tomorrowBrush;
            }
        }
        
        return Brushes.White;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null;
    }
}