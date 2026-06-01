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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using EBTWeather.Avalonia.Misc;
using EBTWeather.Avalonia.Models;
using EBTWeather.Avalonia.UnitValues;
using EBTWeather.Avalonia.WeatherData;
using EBTWeather.Avalonia.web_services.open_meteo;
using EBTWeather.web_services.open_meteo;
using log4net;
using Microsoft.Extensions.Caching.Memory;

namespace EBTWeather.Avalonia.WebService;

public class OpenMeteo
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(OpenMeteo));

    public OpenMeteo(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;

        #region weather codes
        
        // https://github.com/Leftium/weather-sense/blob/62d94b403f198c5531cf9e74f09d995249eb6a5a/src/lib/util.ts#L46-L85
        _decodeWeatherCode[0] = new WeatherDescriptionAndGraphic(
            "Clear", "avares://EBTWeather/Assets/Icons/airy/clear@4x.png");
        
        _decodeWeatherCode[1] = new WeatherDescriptionAndGraphic(
            "Mostly Clear", "avares://EBTWeather/Assets/Icons/airy/mostly-clear@4x.png");
        _decodeWeatherCode[2] = new WeatherDescriptionAndGraphic(
            "Partly Cloudy", "avares://EBTWeather/Assets/Icons/airy/partly-cloudy@4x.png");
        _decodeWeatherCode[3] = new WeatherDescriptionAndGraphic(
            "Cloudy", "avares://EBTWeather/Assets/Icons/airy/overcast@4x.png");
        
        _decodeWeatherCode[45] = new WeatherDescriptionAndGraphic(
            "Fog", "avares://EBTWeather/Assets/Icons/airy/fog@4x.png");
        _decodeWeatherCode[48] = new WeatherDescriptionAndGraphic(
            "Freezing Fog", "avares://EBTWeather/Assets/Icons/airy/rime-fog@4x.png");
        
        _decodeWeatherCode[51] = new WeatherDescriptionAndGraphic(
            "Light Drizzle", "avares://EBTWeather/Assets/Icons/airy/light-drizzle@4x.png");
        _decodeWeatherCode[53] = new WeatherDescriptionAndGraphic(
            "Drizzle", "avares://EBTWeather/Assets/Icons/airy/moderate-drizzle@4x.png");
        _decodeWeatherCode[55] = new WeatherDescriptionAndGraphic(
            "Heavy Drizzle", "avares://EBTWeather/Assets/Icons/airy/dense-drizzle@4x.png");
        
        _decodeWeatherCode[56] = new WeatherDescriptionAndGraphic(
            "Light Freezing Drizzle", "avares://EBTWeather/Assets/Icons/airy/light-freezing-drizzle@4x.png");
        _decodeWeatherCode[57] = new WeatherDescriptionAndGraphic(
            "Freezing Drizzle", "avares://EBTWeather/Assets/Icons/airy/dense-freezing-drizzle@4x.png");
        
        _decodeWeatherCode[61] = new WeatherDescriptionAndGraphic(
            "Light Rain", "avares://EBTWeather/Assets/Icons/airy/light-rain@4x.png");
        _decodeWeatherCode[63] = new WeatherDescriptionAndGraphic(
            "Rain", "avares://EBTWeather/Assets/Icons/airy/moderate-rain@4x.png");
        _decodeWeatherCode[65] = new WeatherDescriptionAndGraphic(
            "Heavy Rain", "avares://EBTWeather/Assets/Icons/airy/heavy-rain@4x.png");
        
        _decodeWeatherCode[66] = new WeatherDescriptionAndGraphic(
            "Light Freezing Rain", "avares://EBTWeather/Assets/Icons/airy/light-freezing-rain@4x.png");
        _decodeWeatherCode[67] = new WeatherDescriptionAndGraphic(
            "Freezing Rain", "avares://EBTWeather/Assets/Icons/airy/heavy-freezing-rain@4x.png");
        
        _decodeWeatherCode[71] = new WeatherDescriptionAndGraphic(
            "Light Snow", "avares://EBTWeather/Assets/Icons/airy/slight-snowfall@4x.png");
        _decodeWeatherCode[73] = new WeatherDescriptionAndGraphic(
            "Snow", "avares://EBTWeather/Assets/Icons/airy/moderate-snowfall@4x.png");
        _decodeWeatherCode[75] = new WeatherDescriptionAndGraphic(
            "Heavy Snow", "avares://EBTWeather/Assets/Icons/airy/heavy-snowfall@4x.png");
        
        _decodeWeatherCode[77] = new WeatherDescriptionAndGraphic(
            "Snow Grains", "avares://EBTWeather/Assets/Icons/airy/snowflake@4x.png");
        
        _decodeWeatherCode[80] = new WeatherDescriptionAndGraphic(
            "Light Rain Shower", "avares://EBTWeather/Assets/Icons/airy/light-rain@4x.png");
        _decodeWeatherCode[81] = new WeatherDescriptionAndGraphic(
            "Rain Shower", "avares://EBTWeather/Assets/Icons/airy/moderate-rain@4x.png");
        _decodeWeatherCode[82] = new WeatherDescriptionAndGraphic(
            "Heavy Rain Shower", "avares://EBTWeather/Assets/Icons/airy/heavy-rain@4x.png");
        
        _decodeWeatherCode[85] = new WeatherDescriptionAndGraphic(
            "Snow Shower", "avares://EBTWeather/Assets/Icons/airy/slight-snowfall@4x.png");
        _decodeWeatherCode[86] = new WeatherDescriptionAndGraphic(
            "Heavy Snow Shower", "avares://EBTWeather/Assets/Icons/airy/heavy-snowfall@4x.png");
        
        _decodeWeatherCode[95] = new WeatherDescriptionAndGraphic(
            "Thunderstorm", "avares://EBTWeather/Assets/Icons/airy/thunderstorm@4x.png");
        
        _decodeWeatherCode[96] = new WeatherDescriptionAndGraphic(
            "Hailstorm", "avares://EBTWeather/Assets/Icons/airy/thunderstorm-with-hail@4x.png");
        _decodeWeatherCode[99] = new WeatherDescriptionAndGraphic(
                "Heavy Hailstorm", "avares://EBTWeather/Assets/Icons/airy/'thunderstorm-with-hail@4x.png");

        // Create a dictionary that has the appropriate graphic uris for night.
        _decodeWeatherCode.ToList().ForEach(keyValuePair =>
        {
            var value = keyValuePair.Value;
            
            value = value.WeatherIconUri switch
            {
                "avares://EBTWeather/Assets/Icons/airy/clear@4x.png" => value with { WeatherIconUri = "avares://EBTWeather/Assets/Icons/google-v2/clear_night.png" },
                "avares://EBTWeather/Assets/Icons/airy/mostly-clear@4x.png" => value with { WeatherIconUri = "avares://EBTWeather/Assets/Icons/google-v2/mostly_clear_night.png" },
                "avares://EBTWeather/Assets/Icons/airy/partly-cloudy@4x.png" => value with { WeatherIconUri = "avares://EBTWeather/Assets/Icons/google-v2/partly_cloudy_night.png" },
                _ => value
            };

            _decodeWeatherCodeNight[keyValuePair.Key] = value;
        });

        #endregion
    }
    
    private IHttpClientFactory _httpClientFactory;
    
    public record WeatherDescriptionAndGraphic(string Description, string WeatherIconUri);
    
    private readonly Dictionary<int, WeatherDescriptionAndGraphic> _decodeWeatherCode = new();

    private readonly Dictionary<int, WeatherDescriptionAndGraphic> _decodeWeatherCodeNight = new();

    public async Task<WeatherInfo> GetCachedCurrentWeather(LocationData locationData, IMemoryCache cache)
    {
        Log.Info($"***** GetCachedCurrentWeather ***** Location: {locationData.Name}");
        
        var key = GetCurrentWeatherRequestUri(locationData);

        if (cache.TryGetValue(key, out var weatherInfo))
        {
            return (weatherInfo as WeatherInfo)!;            
        }
        else
        {
            var result = await GetCurrentWeather(locationData);

            cache.Set(key, result, Constants.CacheRetentionPeriodCurrent);

            return result;
        }
    }

    public async Task<WeatherInfo> GetCurrentWeather(LocationData locationData)
    {
        Log.Info($"***** GetCurrentWeather ***** Location: {locationData.Name}");

        var requestUri = GetCurrentWeatherRequestUri(locationData);
        Log.Info(requestUri);
        
        var startTime = DateTime.Now;
        
        using var client = _httpClientFactory.CreateClient(Constants.OpenMeteoForecastClientName);
        using var response = await client.GetAsync(requestUri);

        var duration = DateTime.Now - startTime;
        Log.Info($"GetCurrentWeather: web service call duration: {duration}");
        
        var jsonResponse = await response.Content.ReadAsStringAsync();

        var responseObject = JsonSerializer.Deserialize<WeatherResponse>(jsonResponse);
        
        var location = new GeoLocation(
            responseObject!.latitude, responseObject.longitude, new ShortDistance(responseObject.elevation));

        var hourlyWeatherInfo = responseObject.hourly.time.Select((dateTime, i) => 
            new HourlyWeatherInfo(
                DateTimeUtils.ToUtc(dateTime, responseObject.timezone),
                new Temperature(responseObject.hourly.temperature_2m[i]), 
                new Temperature(responseObject.hourly.apparent_temperature[i]), 
                new Temperature(responseObject.hourly.dew_point_2m[i]), 
                new Speed(responseObject.hourly.wind_speed_10m[i]), 
                responseObject.hourly.wind_direction_10m[i], 
                new Speed(responseObject.hourly.wind_gusts_10m[i]), 
                responseObject.hourly.relative_humidity_2m[i],
                new Pressure(responseObject.hourly.pressure_msl[i]),
                new Precipitation(responseObject.hourly.precipitation[i]),
                responseObject.hourly.precipitation_probability[i], 
                responseObject.hourly.weather_code[i],
                _decodeWeatherCode[responseObject.hourly.weather_code[i]].Description,
                new Visibility(responseObject.hourly.visibility[i]),
                responseObject.hourly.cloud_cover[i])
        ).ToList();
        
        hourlyWeatherInfo.Sort();

        var now = DateTime.Now.ToUniversalTime();
        
        var todayStart = DateTime.SpecifyKind(
            new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0), DateTimeKind.Utc);
        
        var hourlyWeatherForRestOfTodayAndFuture = 
            hourlyWeatherInfo.Where(item => item.DateTime >= todayStart).ToList();

        var hoursUntilMidnightTomorrow = 48 - DateTime.Now.Hour;
        
        var hourlyWeatherInfoForRestOfTodayAndTomorrow = 
            hourlyWeatherForRestOfTodayAndFuture[..hoursUntilMidnightTomorrow];
        
        var dictionary = responseObject.current.is_day != 0 ? _decodeWeatherCode : _decodeWeatherCodeNight;

        var dailyWeatherInfo = responseObject.daily.time.Select((date, i) => new DailyWeatherInfo(
            date,
            new Temperature(responseObject.daily.temperature_2m_min[i]), 
            new Temperature(responseObject.daily.temperature_2m_max[i]), 
            responseObject.daily.sunrise[i], 
            responseObject.daily.sunset[i], 
            responseObject.daily.uv_index_max[i] ?? double.NaN, 
            new Pressure(responseObject.daily.pressure_msl_mean[i]),
            responseObject.daily.relative_humidity_2m_mean[i],
            new Temperature(responseObject.daily.dew_point_2m_mean[i]),
            responseObject.daily.cloud_cover_mean[i],
            new Visibility(responseObject.daily.visibility_mean[i] ?? double.NaN),
            new Precipitation(responseObject.daily.precipitation_sum[i]), 
            responseObject.daily.precipitation_probability_max[i], 
            new Speed(responseObject.daily.wind_gusts_10m_max[i]),
            new Speed(responseObject.daily.wind_gusts_10m_mean[i]),
            new Speed(responseObject.daily.wind_speed_10m_max[i]),
            new Speed(responseObject.daily.wind_speed_10m_mean[i]),
            responseObject.daily.wind_direction_10m_dominant[i],
            _decodeWeatherCode[responseObject.daily.weather_code[i]].Description,
            dictionary[responseObject.daily.weather_code[i]].WeatherIconUri,
            [])).ToList();

        foreach (var dwi in dailyWeatherInfo)
        {
            var hourlyListForDay = hourlyWeatherInfo.Where(hd => 
                DateOnly.FromDateTime(hd.DateTime).CompareTo(dwi.Date) == 0);
            dwi.HourlyWeatherInfo.AddRange(hourlyListForDay);
        }

        dailyWeatherInfo.Sort();

        var airPressureChange = AirPressureChange(
            responseObject.current.pressure_msl, 
            hourlyWeatherForRestOfTodayAndFuture[2].AirPressure.MetricValue);

        var todayWeatherData = new CurrentWeatherInfo(
            "Today",
            true,
            responseObject.current.is_day != 0,
            new Temperature(responseObject.current.temperature_2m), 
            new Temperature(responseObject.current.apparent_temperature),
            dailyWeatherInfo[0].TemperatureMin, 
            dailyWeatherInfo[0].TemperatureMax,
            responseObject.current.uv_index,
            dailyWeatherInfo[0].UvIndexMax,
            responseObject.current.cloud_cover,
            dailyWeatherInfo[0].Sunrise,
            dailyWeatherInfo[0].Sunset,
            responseObject.current.relative_humidity_2m, 
            new Temperature(responseObject.current.dew_point_2m),
            new Pressure(responseObject.current.pressure_msl),
            airPressureChange,
            new Speed(responseObject.current.wind_speed_10m),
            new Speed(responseObject.current.wind_gusts_10m),
            responseObject.current.wind_direction_10m,
            new Visibility(responseObject.current.visibility),
            dailyWeatherInfo[0].PrecipitationProbabilityMax,
            dailyWeatherInfo[0].PrecipitationSum,
            dictionary[responseObject.current.weather_code].Description,
            dictionary[responseObject.current.weather_code].WeatherIconUri
        );
        
        var tomorrowWeatherData = new CurrentWeatherInfo(
            "Tomorrow",
            false,
            true,
            new Temperature(0), 
            new Temperature(0),
            dailyWeatherInfo[1].TemperatureMin,
            dailyWeatherInfo[1].TemperatureMax,
            0,
            dailyWeatherInfo[1].UvIndexMax,
            dailyWeatherInfo[1].CloudCoverMean,
            dailyWeatherInfo[1].Sunrise,
            dailyWeatherInfo[1].Sunset,
            dailyWeatherInfo[1].RelativeHumidityMean,
            dailyWeatherInfo[1].DewPointMean,
            dailyWeatherInfo[1].AirPressureMean,
            string.Empty,
            dailyWeatherInfo[1].WindSpeedMean,
            dailyWeatherInfo[1].WindGustsMean,
            dailyWeatherInfo[1].WindDirectionDominant,
            dailyWeatherInfo[1].VisibilityMean,
            dailyWeatherInfo[1].PrecipitationProbabilityMax,
            dailyWeatherInfo[1].PrecipitationSum,
            dailyWeatherInfo[1].WeatherDescription,
            dailyWeatherInfo[1].WeatherIconUri
        );

        CurrentWeatherInfo[] todayAndTomorrowWeatherInfo = [todayWeatherData, tomorrowWeatherData];
        
        return new WeatherInfo(
            location,
            DateTime.Parse(responseObject.current.time),
            [..todayAndTomorrowWeatherInfo],
            hourlyWeatherInfoForRestOfTodayAndTomorrow,
            dailyWeatherInfo,
            hourlyWeatherInfo);
    }

    private static string GetCurrentWeatherRequestUri(LocationData locationData)
    {
        return $"/v1/forecast?latitude={locationData.GeoLocation.Latitude}&longitude={locationData.GeoLocation.Longitude}&elevation={locationData.GeoLocation.Elevation.MetricValue}&timezone=auto&forecast_days=10&daily=cloud_cover_mean,visibility_mean,relative_humidity_2m_mean,dew_point_2m_mean,pressure_msl_mean,wind_gusts_10m_mean,wind_speed_10m_mean,temperature_2m_max,temperature_2m_min,sunrise,sunset,uv_index_max,precipitation_sum,precipitation_probability_max,wind_gusts_10m_max,wind_speed_10m_max,wind_direction_10m_dominant,weather_code&hourly=cloud_cover,precipitation_probability,precipitation,apparent_temperature,temperature_2m,dew_point_2m,wind_speed_10m,wind_direction_10m,wind_gusts_10m,relative_humidity_2m,weather_code,visibility,pressure_msl&current=dew_point_2m,is_day,uv_index,apparent_temperature,weather_code,temperature_2m,wind_speed_10m,wind_gusts_10m,relative_humidity_2m,pressure_msl,wind_direction_10m,visibility,cloud_cover&temperature_unit=celsius&wind_speed_unit=kmh&pressure_msl_unit=hPa";
    }

    public async Task<HistoricalWeatherInfo> GetCachedHistoricalWeather(LocationData locationData, DateOnly startDate,
        DateOnly endDate, IMemoryCache cache)
    {
        Log.Info($"***** GetCachedCurrentWeather ***** Location: {locationData.Name}");

        var key = GetHistoricalWeatherRequestUri(locationData, startDate, endDate);

        if (cache.TryGetValue(key, out var historicalWeatherInfo))
        {
            return (historicalWeatherInfo as HistoricalWeatherInfo)!;
        }
        else
        {
            var result = await GetHistoricalWeather(locationData, startDate, endDate);
            
            cache.Set(key, result, Constants.CacheRetentionPeriodHistorical);
            
            return result;
        }
    }

    private static string GetHistoricalWeatherRequestUri(LocationData locationData, DateOnly startDate, DateOnly endDate)
    {
        const string dateFormat = "yyyy-MM-dd";

        var startDateString = startDate.ToString(dateFormat, CultureInfo.InvariantCulture);
        var endDateString = endDate.ToString(dateFormat, CultureInfo.InvariantCulture);

        return $"/v1/archive?latitude={locationData.GeoLocation.Latitude}&longitude={locationData.GeoLocation.Longitude}&elevation={locationData.GeoLocation.Elevation.MetricValue}&timezone=auto&start_date={startDateString}&end_date={endDateString}&daily=visibility_mean,relative_humidity_2m_mean,dew_point_2m_mean,pressure_msl_mean,wind_gusts_10m_mean,wind_speed_10m_mean,cloud_cover_mean,weather_code,uv_index_max,temperature_2m_max,temperature_2m_min,apparent_temperature_min,apparent_temperature_max,sunrise,sunset,precipitation_sum,wind_speed_10m_max,wind_gusts_10m_max,wind_direction_10m_dominant";
    }
    
    public async Task<HistoricalWeatherInfo> GetHistoricalWeather(LocationData locationData, DateOnly startDate, 
        DateOnly endDate)
    {
        Log.Info($"***** GetHistoricalWeather ***** location: {locationData.Name} startDate: {startDate} endDate: {endDate}");

        var startDateDateOnly = DateOnly.FromDateTime(startDate.ToDateTime(TimeOnly.MinValue));
        var endDateDateOnly = DateOnly.FromDateTime(endDate.ToDateTime(TimeOnly.MinValue));
        var nowDateOnly = DateOnly.FromDateTime(DateTime.Now);

        string? errorMessage = null;
        
        if (startDateDateOnly > nowDateOnly)
        {
            errorMessage = "Start Date is not in the past";
        } else if (endDateDateOnly >= nowDateOnly)
        {
            errorMessage = "End Date is not in the past";
        } else if (endDateDateOnly < startDateDateOnly)
        {
            errorMessage = "End Date must not be earlier than Start Date";
        }
        
        if (errorMessage != null)
        {
            return new HistoricalWeatherInfo(
                [], 
                new Precipitation(0.0), 
                new Temperature(0.0), 
                new Temperature(0.0), 
                new Speed(0.0),
                new Speed(0.0),
                true, 
                errorMessage);
        }

        var requestUri = GetHistoricalWeatherRequestUri(locationData, startDate, endDate);

        var startTime = DateTime.Now;
        
        using var client = _httpClientFactory.CreateClient(Constants.OpenMeteoHistoricalClientName);
        using var response = await client.GetAsync(requestUri);

        var duration = DateTime.Now - startTime;
        Log.Info($"GetHistoricalWeather: web service call elapsed time: {duration}");

        if (response.IsSuccessStatusCode)
        {
            var jsonResponse = await response.Content.ReadAsStringAsync();

            // TODO: Need to create HistoricalWeatherResponse class and update data extraction code accordingly.
            var responseObject = JsonSerializer.Deserialize<HistoricalResponse>(jsonResponse);

            Log.Info($"responseObject: error: {responseObject!.error} reason: {responseObject.reason}");

            var dailyWeatherInfo = responseObject.daily.time.Select((date, i) => new DailyWeatherInfo(
                date,
                new Temperature(responseObject.daily.temperature_2m_min[i]),
                new Temperature(responseObject.daily.temperature_2m_max[i]),
                responseObject.daily.sunrise[i],
                responseObject.daily.sunset[i],
                responseObject.daily.uv_index_max[i] ?? Double.NaN,
                new Pressure(responseObject.daily.pressure_msl_mean[i]),
                responseObject.daily.relative_humidity_2m_mean[i],
                new Temperature(responseObject.daily.dew_point_2m_mean[i]),
                responseObject.daily.cloud_cover_mean[i],
                new Visibility(responseObject.daily.visibility_mean[i] ?? Double.NaN),
                new Precipitation(responseObject.daily.precipitation_sum[i]),
                0,
                new Speed(responseObject.daily.wind_gusts_10m_max[i]),
                new Speed(responseObject.daily.wind_gusts_10m_mean[i]),
                new Speed(responseObject.daily.wind_speed_10m_max[i]),
                new Speed(responseObject.daily.wind_speed_10m_mean[i]),
                responseObject.daily.wind_direction_10m_dominant[i],
                _decodeWeatherCode[responseObject.daily.weather_code[i]].Description,
                _decodeWeatherCode[responseObject.daily.weather_code[i]].WeatherIconUri,
                [])).ToList();

            dailyWeatherInfo.Sort();
            
            var totalPrecipitation = new Precipitation(dailyWeatherInfo.Sum(x => x.PrecipitationSum.MetricValue));
            var minTemperature = new Temperature(dailyWeatherInfo.Min(x => x.TemperatureMin.MetricValue));
            var maxTemperature = new Temperature(dailyWeatherInfo.Max(x => x.TemperatureMax.MetricValue));
            var maxWindSpeed = new Speed(dailyWeatherInfo.Max(x => x.WindSpeedMax.MetricValue));
            var maxWindGusts = new Speed(dailyWeatherInfo.Max(x => x.WindGustsMax.MetricValue));
                
            var result = new HistoricalWeatherInfo(
                dailyWeatherInfo, 
                totalPrecipitation, 
                minTemperature, 
                maxTemperature, 
                maxWindSpeed,
                maxWindGusts,
                responseObject.error, 
                responseObject.reason);

            return result;
        }
        else
        {
            Log.Info($"GetHistoricalWeather: got response of {response.StatusCode} reason: {response.ReasonPhrase}");
            return new HistoricalWeatherInfo(
                [], 
                new Precipitation(0.0), 
                new Temperature(0.0), 
                new Temperature(0.0), 
                new Speed(0.0),
                new Speed(0.0),
                true, 
                response.ReasonPhrase!);
        }
    }

    private static string AirPressureChange(double current, double oneHourInTheFuture)
    {
        var result = string.Empty;

        // We consider a change of >= 2 hPa in about an hour to be significant.
        var significantChange = Math.Abs(current - oneHourInTheFuture) >= 2.0;

        if (significantChange)
        {
            result = oneHourInTheFuture > current ? "↑" : "↓";
        }

        return result;
    }

    public async Task<LocationsData> GetGeoLocations(string locationName, string? countryCode)
    {
        Log.Info($"GetGeoLocations locationName: \"{locationName}\" *****");

        var requestUri =
            $"/v1/search?name={HttpUtility.UrlEncode(locationName.Trim())}&count={Constants.LocationSearchResults}&language=en&format=json";

        if (countryCode != null)
        {
            requestUri =  $"{requestUri}&countryCode={countryCode.ToUpper()}";
        }
        
        Log.Info(requestUri);

        var startTime = DateTime.Now;

        using var client = _httpClientFactory.CreateClient(Constants.OpenMeteoGeoCodingClientName);
        using var response = await client.GetAsync(requestUri);

        var duration = DateTime.Now - startTime;
        Log.Info($"GetGeoLocations: web service call duration: {duration}");

        var jsonResponse = await response.Content.ReadAsStringAsync();

        var responseObject = JsonSerializer.Deserialize<LocationResponse>(jsonResponse);

        if (responseObject!.results is not null)
        {
            Log.Info($"GetGeoLocations: returning list of {responseObject.results.Length} items");

            return new LocationsData(responseObject.results.Select(locationInfo => new LocationData(
                    locationInfo.id.ToString(),
                    locationInfo.name,
                    new GeoLocation(locationInfo.latitude, locationInfo.longitude,
                        new ShortDistance(locationInfo.elevation)),
                    locationInfo.country_code,
                    locationInfo.admin1)
                )
                .OrderBy(x => x.Name)
                .ThenBy(x => x.CountryCode)
                .ThenBy(x => x.Admin1)
                .ToList());
        }
        else
        {
            return new LocationsData([]);
        }
    }
}
