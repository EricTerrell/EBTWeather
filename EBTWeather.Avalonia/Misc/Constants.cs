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

public class Constants
{
    public const int WebServiceCallFrequencyMinutes = 5;

    public static readonly TimeSpan CacheRetentionPeriodCurrent = 
        TimeSpan.FromMinutes(WebServiceCallFrequencyMinutes - 1);
    public static readonly TimeSpan CacheRetentionPeriodHistorical = TimeSpan.FromMinutes(15);
    
    public static readonly TimeSpan CacheExpirationScanFrequency = TimeSpan.FromMinutes(1);
    
    public const int LocationSearchResults = 100;
    
    public const string OpenMeteoForecastClientName = "OPEN_METEO_FORECAST_CLIENT";
    public const string OpenMeteoHistoricalClientName = "OPEN_METEO_HISTORICAL_CLIENT";
    public const string OpenMeteoGeoCodingClientName = "OPEN_METEO_GEOCODING_CLIENT";
    public const string MainWebsiteClientName = "MAIN_WEBSITE_CLIENT";
    
    public const string MainWebsiteUrl = "https://www.EricBT.com";
    public const string CodeDownloadUrl = "/versions/EBTWeather";
    public const string DownloadUpdatesUrl = "https://www.EricBT.com/EBTWeather/download";
    
    public const string SupportEmail = "EBTWeather@EricBT.com";

    public const string AppName = "EBT Weather";

    public static TimeSpan CheckForUpdatesInterval = TimeSpan.FromDays(7);

    public const string MinimizeArg = "--minimize";
}
