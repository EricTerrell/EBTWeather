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

using EBTWeather.Avalonia.Misc;

namespace EBTWeather.Avalonia.UnitValues;

public class ShortDistance : IUnitValue
{
    public ShortDistance(double metricValue)
    {
        MetricValue = metricValue;
    }

    public override string ToString()
    {
        var value = UnitsNet.Length.FromMeters(MetricValue);
        
        return Settings.Units == Units.Metric
            ? value.Meters.ToString("F0")
            : value.Feet.ToString("F0");
    }

    public double MetricValue { get; private set; }

    public static string MetricSuffix => " m";

    public static string USASuffix => " ft";
}