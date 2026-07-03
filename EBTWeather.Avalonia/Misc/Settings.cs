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
using System.IO;
using System.Linq;
using System.Text.Json;
using EBTWeather.Avalonia.Models;
using log4net;

namespace EBTWeather.Avalonia.Misc;

/// <summary>
/// In-memory user preferences which are loaded from a file on app startup, and saved to the file before shutdown.
/// </summary>
public static class Settings
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(Settings));

    public static Units Units
    {
        get => _persistedSettings.Units;
        set { _persistedSettings.Units = value; Save(); }
    }

    public static ScreenMode ScreenMode
    {
        get => _persistedSettings.ScreenMode;
        set { _persistedSettings.ScreenMode = value; Save(); }
    }

    public static bool AcceptedLicenseTerms
    {
        get => _persistedSettings.AcceptedLicenseTerms;
        set { _persistedSettings.AcceptedLicenseTerms = value; Save(); }
    }

    public static DateTimeOffset LastAutomaticCheckForUpdates
    {
        get => _persistedSettings.LastAutomaticCheckForUpdates;
        set { _persistedSettings.LastAutomaticCheckForUpdates = value; Save(); }
    }

    public static bool AutomaticallyCheckForUpdates
    {
        get => _persistedSettings.AutomaticallyCheckForUpdates;
        set { _persistedSettings.AutomaticallyCheckForUpdates = value; Save(); }
    }

    public static Dictionary<string, LocationData> Locations
    {
        get => _persistedSettings.Locations;
        set { _persistedSettings.Locations = value; Save(); }
    }

    public static string CountryCode
    {
        get => _persistedSettings.CountryCode;
        set { _persistedSettings.CountryCode = value; Save(); }
    }

    public static bool SpecifyCountryCode
    {
        get => _persistedSettings.SpecifyCountryCode;
        set { _persistedSettings.SpecifyCountryCode = value; Save(); }
    }

    public static int CurrentLocationIndex
    {
        get => _persistedSettings.CurrentLocationIndex;
        set { _persistedSettings.CurrentLocationIndex = value; Save(); }
    }

    public static string? AirPollutionApiKey
    {
        get => _persistedSettings.AirPollutionApiKey;
        set { _persistedSettings.AirPollutionApiKey = value; Save(); }
    }

    /// <summary>
    /// Return sorted list of locations
    /// </summary>
    public static LocationsData LocationsData
    {
        get
        {
            var locations = Locations.Keys.ToList().Select<string, LocationData>(locationKey => Locations[locationKey])
                .OrderBy(x => x!.Name)
                .ThenBy(x => x!.CountryCode)
                .ThenBy(x => x!.Admin1)
                .ToList();

            return new LocationsData(locations);
        }
    }

    /// <summary>
    /// Return index of a given location 
    /// </summary>
    /// <param name="locationId"></param>
    /// <returns>index or -1 if not found</returns>
    public static int LocationIndexFromId(string locationId)
    {
        return LocationsData.Locations.FindIndex(location => location.Id == locationId);
    }

    private static readonly string SettingsFolder = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "EBT Weather");

    private static readonly string SettingsPath = Path.Combine(SettingsFolder, "settings.json");

    private class PersistedSettings
    {
        public Units Units { get; set; } = Units.Metric;

        public ScreenMode ScreenMode { get; set; } = ScreenMode.System;

        public bool AcceptedLicenseTerms { get; set; }

        public DateTimeOffset LastAutomaticCheckForUpdates { get; set; } = DateTimeOffset.MinValue;

        public bool AutomaticallyCheckForUpdates { get; set; } = true;

        public Dictionary<string, LocationData> Locations { get; set; } = new();
    
        public string CountryCode { get; set; } = string.Empty;
    
        public bool SpecifyCountryCode {  get; set; }

        public int CurrentLocationIndex { get; set; } = -1;

        public string? AirPollutionApiKey { get; set; }
    }
    
    private static PersistedSettings _persistedSettings = new();

    public static void Load()
    {
        Log.Info($"Load from: \"{SettingsPath}\"");

        if (File.Exists(SettingsPath))
        {
            try
            {
                var json = File.ReadAllText(SettingsPath);

                _persistedSettings = JsonSerializer.Deserialize<PersistedSettings>(json) ?? new PersistedSettings();
            }
            catch (Exception ex)
            {
                Log.Error($"Settings.Load: {ex}");
            }
        }
    }

    public static void Save()
    {
        Log.Info($"Save to: \"{SettingsPath}\"");
        
        var json = JsonSerializer.Serialize(_persistedSettings, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        if (!Directory.Exists(SettingsFolder))
        {
            Directory.CreateDirectory(SettingsFolder);
        }
        
        File.WriteAllText(SettingsPath, json);
    }
}
