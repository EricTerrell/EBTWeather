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

namespace EBTWeather.Avalonia.Misc;

public record Angle(int Degrees, int Minutes, int Seconds);

public class AngleUtils
{
    public static double DmsToDecimalDegrees(double degrees, double minutes, double seconds)
    {
        return degrees +  (minutes / 60.0) + (seconds / 3600.0);
    }

    /// <summary>
    /// Convert a decimal angle to degrees, minutes, and seconds format
    /// </summary>
    /// <param name="angleInDegrees">angle in decimal degrees</param>
    /// <returns>angle in degrees, minutes, seconds format</returns>
    public static Angle ConvertToDms(double angleInDegrees)
    {
        var decimalDegrees = Math.Abs(angleInDegrees);

        var degrees = (int) Math.Truncate(decimalDegrees);

        var fractionalDegrees = decimalDegrees - degrees;

        var minutes = (int) Math.Truncate(fractionalDegrees * 60.0);

        fractionalDegrees -= minutes / 60.0;

        var seconds = (int) Math.Truncate(fractionalDegrees * 60.0 * 60.0);
        
        return new Angle(Math.Sign(angleInDegrees) * degrees, minutes, seconds);
    }

    /// <summary>
    /// Convert signed latitude or longitude to direction number (N (0)/S (1) or E (0)/W (1))
    /// </summary>
    /// <param name="degrees">latitude or longitude</param>
    /// <returns>0 or 1 for </returns>
    public static int DegreesToDirection(double degrees)
    {
        return Math.Sign(degrees) >= 0 ? 0 : 1;
    }
}