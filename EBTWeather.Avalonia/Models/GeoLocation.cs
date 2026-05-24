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

using EBTWeather.Avalonia.UnitValues;

namespace EBTWeather.Avalonia.Models;

public class GeoLocation(double latitude, double longitude, ShortDistance elevation)
{
    public double Latitude { get; } = latitude;
    public double Longitude { get; } = longitude;

    public ShortDistance Elevation { get; } = elevation;
    
    public override string ToString()
    {
        return $"Latitude: {Latitude} Longitude: {Longitude} Elevation: {Elevation}";
    }
}
