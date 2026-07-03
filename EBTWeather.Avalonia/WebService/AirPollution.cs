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
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using EBTWeather.Avalonia.Misc;
using EBTWeather.Avalonia.Models;
using EBTWeather.Avalonia.WeatherData;
using EBTWeather.Avalonia.web_services.open_weather_map;
using log4net;

namespace EBTWeather.Avalonia.WebService;

public class AirPollution
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(AirPollution));

    private readonly IHttpClientFactory _httpClientFactory;

    public AirPollution(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    private static string GetRequestUri(LocationData locationData, string apiKey)
    {
        return
            $"/data/2.5/air_pollution?lat={locationData.GeoLocation.Latitude}&lon={locationData.GeoLocation.Longitude}&appid={apiKey}";
    }

    public async Task<AirPollutionInfo?> GetCurrentAirPollutionInfo(LocationData locationData, string apiKey)
    {
        Log.Info($"***** GetCurrentWeather ***** Location: {locationData.Name}");

        var requestUri = GetRequestUri(locationData, apiKey);
        Log.Info(requestUri);

        var startTime = DateTime.Now;

        try
        {
            using var client = _httpClientFactory.CreateClient(Constants.AirPollutionClientName);
            using var response = await client.GetAsync(requestUri);

            var jsonResponse = await response.Content.ReadAsStringAsync();

            var responseObject = JsonSerializer.Deserialize<AirPollutionResponse>(jsonResponse);

            return new AirPollutionInfo(responseObject.list[0].main.aqi, new Components(
                responseObject.list[0].components.co,
                responseObject.list[0].components.no,
                responseObject.list[0].components.no2,
                responseObject.list[0].components.o3,
                responseObject.list[0].components.so2,
                responseObject.list[0].components.pm2_5,
                responseObject.list[0].components.pm10,
                responseObject.list[0].components.nh3)
            );
        }
        catch (Exception ex)
        {
            Log.Error($"GetCurrentAirPollutionInfo: web service call exception: {ex}");

            return null;
        }
        finally
        {
            var duration = DateTime.Now - startTime;

            Log.Info($"GetCurrentAirPollutionInfo: web service call duration: {duration}");
        }
    }
}