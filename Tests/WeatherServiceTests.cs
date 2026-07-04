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
using EBTWeather.Avalonia.Models;
using EBTWeather.Avalonia.UnitValues;
using EBTWeather.Avalonia.WebService;
using log4net;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace Tests;

public class WeatherServiceTests
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(WeatherServiceTests));

    private OpenMeteo _weatherService;
    
    private readonly LocationData _locationData = new(
        "-1",
        "Cortez", 
        new GeoLocation(37.3489, -108.5859, new ShortDistance(1972.0)), "America/Denver");

    private IMemoryCache _memoryCache;
    
    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        var defaultTimeout = TimeSpan.FromMinutes(1);

        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        
        var mockIHttpClientFactory = new Mock<IHttpClientFactory>();
        mockIHttpClientFactory.Setup(factory => factory.CreateClient(Constants.OpenMeteoForecastClientName))
            .Returns(new HttpClient
            {
                BaseAddress = new Uri("https://api.open-meteo.com"),
                Timeout = defaultTimeout
            });
        
        mockIHttpClientFactory.Setup(factory => factory.CreateClient(Constants.OpenMeteoHistoricalClientName))
            .Returns(new HttpClient
            {
                BaseAddress = new Uri("https://archive-api.open-meteo.com"),
                Timeout = defaultTimeout
            });
        
        mockIHttpClientFactory.Setup(factory => factory.CreateClient(Constants.OpenMeteoGeoCodingClientName))
            .Returns(new HttpClient
            {
                BaseAddress = new Uri("https://geocoding-api.open-meteo.com"),
                Timeout = defaultTimeout
            });
        
        _weatherService = new OpenMeteo(mockIHttpClientFactory.Object);
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _memoryCache.Dispose();
    }
    
    [Test]
    public async Task GetCurrentWeather()
    {
        var result = await _weatherService.GetCurrentWeather(_locationData, _memoryCache);
    }

    [Test]
    public async Task GetHistoricalWeather()
    {
        var startDate = new DateOnly(2026, 02, 09);
        var endDate = new DateOnly(2026, 02, 18);
        
        var result = await _weatherService.GetHistoricalWeather(_locationData, startDate, endDate);
    }

    [Test]
    public async Task GetGeoLocations()
    {
        var result = await _weatherService.GetGeoLocations("Cortez", null);
    }
}