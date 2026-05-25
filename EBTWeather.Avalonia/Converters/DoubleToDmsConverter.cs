/*
  EBT Weather
  (C) Copyright 2026, Eric Bergman-Terrell

  This file is part of EBT Weather.

  EBT Weather is free software: you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation, either version 3 of the License, or
  (at your option) any later version.

  EBT Weather is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.

  You should have received a copy of the GNU General Public License
  along with EBT Weather.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Globalization;
using Avalonia.Data.Converters;
using EBTWeather.Avalonia.Misc;

namespace EBTWeather.Avalonia.Converters;

public class DoubleToDmsConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double doubleValue)
        {
            var angle = AngleUtils.ConvertToDms(doubleValue);
            var sign =  Math.Sign(angle.Degrees);
            
            var direction = parameter switch
            {
                "latitude"  => sign < 0 ? "S" : "N",
                "longitude" => sign < 0 ? "W" : "E",
                _ => string.Empty
            };

            var degreesString = parameter switch
            {
                "latitude"  => $"{Math.Abs(angle.Degrees),2}",
                "longitude" => $"{Math.Abs(angle.Degrees),3}",
                _ => string.Empty
            };
            
            return $"{degreesString}°{angle.Minutes:00}'{angle.Seconds:00}\" {direction}";
        }
        
        return string.Empty;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null;
    }
}
