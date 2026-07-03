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

namespace EBTWeather.Avalonia.web_services.open_weather_map;

public record coord(double lon, double lat);

public record main(int aqi);

public record components(
    double co,
    double no,
    double no2,
    double o3,
    double so2,
    double pm2_5,
    double pm10,
    double nh3);

public record main_plus_components(main main, components components);

public record AirPollutionResponse(coord coord, main_plus_components[] list);