using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace EBTWeather.Avalonia.Converters;

public class BitMapAssetValueConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string rawUri && !string.IsNullOrEmpty(rawUri))
        {
            var uri = new Uri(rawUri);
            var assets = AssetLoader.Open(uri);
            return new Bitmap(assets);
        }

        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null;
    }
}