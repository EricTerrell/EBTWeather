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
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace EBTWeather.Avalonia.Models;

public class LocationData(string id, string name, GeoLocation geoLocation, string countryCode = "", string admin1 = "")
{
    public string Id { get; set; } = id;

    public string Name { get; set; } = name;
    
    public GeoLocation GeoLocation { get; set; } = geoLocation;

    public string CountryCode { get; set; } = countryCode;
    
    public string Admin1 { get; set; } = admin1;

    public override bool Equals(object? obj)
    {
        if (obj is LocationData location)
        {
            return location.Id == Id;    
        }
        else
        {
            return false;
        }
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Name, GeoLocation, CountryCode, Admin1);
    }

    public override string ToString()
    {
        return $"{Name} {Admin1} {CountryCode}";
    }

    public string CalculateId()
    {
        var json = JsonSerializer.Serialize(this, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        
        var inputBytes = Encoding.UTF8.GetBytes(json);
        var hashBytes = MD5.HashData(inputBytes);

        return $"USER_SPECIFIED_LOCATION_{Convert.ToHexString(hashBytes)}";
    }
}
