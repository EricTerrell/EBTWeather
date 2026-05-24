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

namespace EBTWeather.web_services.open_meteo;

public class LocationResponse
{
    public LocationInfo[]? results { get; set; }
}

public class LocationInfo
{
    public int id { get; set; }
    public string name { get; set; }
    public double latitude { get; set; }
    public double longitude { get; set; }
    public double elevation { get; set; }
    public string feature_code  { get; set; }
    public string country_code { get; set; }
    public int admin1_id  { get; set; }
    public int admin2_id  { get; set; }
    public int admin3_id  { get; set; }
    public string timezone { get; set; }
    public int population { get; set; }
    public int country_id  { get; set; }
    public string country { get; set; }
    public string admin1 { get; set; }
    public string admin2 { get; set; }
    public string admin3 { get; set; }
    public string[] postcodes { get; set; }
}
