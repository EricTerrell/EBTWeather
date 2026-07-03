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
using EBTWeather.Avalonia.WeatherData;

namespace EBTWeather.Avalonia.Converters;

public class AirPollutionAqiConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is AirPollutionInfo airPollutionInfo)
        {
            var description = airPollutionInfo.aqi switch
            {
                1 => "Good",
                2 => "Fair",
                3 => "Moderate",
                4 => "Poor",
                5 => "Very Poor",
                _ => string.Empty
            };

            return $"{airPollutionInfo.aqi} - {description}";
        }
        
        return "n/a";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null;
    }
}
