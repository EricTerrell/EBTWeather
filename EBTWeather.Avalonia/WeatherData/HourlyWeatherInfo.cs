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
using EBTWeather.Avalonia.UnitValues;

namespace EBTWeather.Avalonia.WeatherData;

public record HourlyWeatherInfo(
    DateTime DateTime,
    Temperature Temperature,
    Temperature ApparentTemperature,
    Temperature DewPoint,
    Speed WindSpeed,
    int WindDirection,
    Speed WindGusts,
    double RelativeHumidity,
    Pressure AirPressure,
    Precipitation Precipitation,
    double PrecipitationProbability,
    int WeatherCode,
    string WeatherDescription,
    Visibility Visibility,
    double CloudCover
    ) : IComparable<HourlyWeatherInfo>
{
    public int CompareTo(HourlyWeatherInfo? other)
    {
        return DateTime.CompareTo(other!.DateTime);
    }
}
