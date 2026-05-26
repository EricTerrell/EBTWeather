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

namespace EBTWeather.web_services.open_meteo;

public class Daily
{
    public DateOnly[] time { get; set; }
    public double[] temperature_2m_max  { get; set; }
    public double[] temperature_2m_min { get; set; }
    public string[] sunrise { get; set; }
    public string[] sunset { get; set; }
    public double?[] uv_index_max { get; set; }
    public double[] precipitation_sum { get; set; }
    public int[] precipitation_probability_max { get; set; }
    public double[] wind_gusts_10m_max { get; set; }
    public double[] wind_gusts_10m_mean { get; set; }
    public double[] wind_speed_10m_max { get; set; }
    public double[] wind_speed_10m_mean { get; set; }
    public int[] wind_direction_10m_dominant { get; set; }
    public int[] weather_code { get; set; }
    public int[] cloud_cover_mean { get; set; }
    public double?[] visibility_mean { get; set; }
    public int[] relative_humidity_2m_mean { get; set; }
    public double[] dew_point_2m_mean { get; set; }
    public double[] pressure_msl_mean { get; set; }
}
