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

public class AirPollutionComponentsConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        const string header = "air quality index";
        
        if (value is AirPollutionInfo airPollutionInfo)
        {
            return $"{header}\n\nAQI: {airPollutionInfo.aqi}\n\nScale: 1 (good) to 5 (very poor)\n\nPollutant concentration in μg/m3:\n\nPM2_5: {airPollutionInfo.components.pm2_5,7:F2}\nPM10:  {airPollutionInfo.components.pm10,7:F2}\n\nCO:    {airPollutionInfo.components.co,7:F2}\nNO:    {airPollutionInfo.components.no,7:F2}\nNO2:   {airPollutionInfo.components.no2,7:F2}\nO3:    {airPollutionInfo.components.o3,7:F2}\nSO2:   {airPollutionInfo.components.so2,7:F2}\nNH3:   {airPollutionInfo.components.nh3,7:F2}";
        }
        
        return $"{header} is not available";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null;
    }
}
