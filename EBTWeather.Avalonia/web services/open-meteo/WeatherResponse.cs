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
using EBTWeather.web_services.open_meteo;

namespace EBTWeather.Avalonia.web_services.open_meteo;

public class WeatherResponse
{
    public double latitude { get; set; }
    public double longitude { get; set; }
    public int utc_offset_seconds { get; set; }
    public string timezone { get; set; }
    public string timezone_abbreviation { get; set; }
    public double elevation { get; set; }
    public Current current { get; set; }
    public Daily daily { get; set; }
    public Hourly hourly { get; set; }
    
    public bool error { get; set; }
    public string reason { get; set; }
}

public class Current
{
    public string time { get; set; }
    public int interval { get; set; }
    public int weather_code { get; set; }
    public double temperature_2m { get; set; }
    public double apparent_temperature { get; set; }
    public double wind_speed_10m { get; set; }
    public int relative_humidity_2m { get; set; }
    public double dew_point_2m { get; set; }
    public double pressure_msl { get; set; }
    public int wind_direction_10m { get; set; }
    public double wind_gusts_10m { get; set; }
    public double visibility { get; set; }
    public double uv_index { get; set; }
    public int cloud_cover { get; set; }
    public int is_day { get; set; }
}

public class Hourly
{
    public DateTime[] time { get; set; }
    public double[] temperature_2m {  get; set; }
    public double[] apparent_temperature { get; set; }
    public double[] dew_point_2m {  get; set; }
    public double[] wind_speed_10m {  get; set; }
    public int[] wind_direction_10m {  get; set; }
    public double[] wind_gusts_10m {  get; set; }
    public int[] relative_humidity_2m {  get; set; }
    public double[] pressure_msl {  get; set; }
    public double[] precipitation_probability { get; set; }
    public double[] precipitation { get; set; }
    public int[] weather_code {  get; set; }
    public double[] visibility {  get; set; }
    public int[] cloud_cover { get; set; }
}
