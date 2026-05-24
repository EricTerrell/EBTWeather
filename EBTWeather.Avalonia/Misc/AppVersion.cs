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
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using log4net;

namespace EBTWeather.Avalonia.Misc;

public class AppVersion
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(AppVersion));

    public AppVersion(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    private IHttpClientFactory _httpClientFactory;

    public async Task<Version?> GetLatestVersion(IHttpClientFactory httpClientFactory)
    {
        try
        {
            const string requestUri = Constants.CodeDownloadUrl;

            Log.Info($"GetLatestVersion: \"{requestUri}\"");
            
            using var httpClient = httpClientFactory.CreateClient(Constants.MainWebsiteClientName);

            var startTime = DateTimeOffset.Now;
            using var response = await httpClient.GetAsync(requestUri);

            var responseString = (await response.Content.ReadAsStringAsync()).Trim();

            var elapsedTime = DateTimeOffset.Now - startTime;
            Log.Info($"elapsedTime: {elapsedTime}");
            
            var tokens = responseString.Split('.');

            Log.Info($"Response: \"{responseString}\" tokens: {tokens}");
            
            if (tokens.Length == 2)
            {
                var version = new Version(responseString);
                Log.Info($"Version: {version}");
                
                return version;
            }
            else
            {
                throw new InvalidDataException($"Incorrect version: {responseString}");
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex);

            return null;
        }
    }

    public static Version RunningVersion()
    {
        return Assembly.GetEntryAssembly()?.GetName().Version!;
    }

    public async Task<bool> IsUpdateAvailable()
    {
        var latestVersion = await GetLatestVersion(_httpClientFactory);

        if (latestVersion != null)
        {
            var currentVersion = RunningVersion();

            var newVersionIsAvailable = currentVersion < latestVersion;

            Log.Info(
                $"currentVersion: {currentVersion} latestVersion: {latestVersion} newVersionIsAvailable: {newVersionIsAvailable}");

            return newVersionIsAvailable;
        }
        else
        {
            Log.Warn("Could not determine latest version.");
            
            return false;
        }
    }
}