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
/// In-memory user preferences which are loaded from a file, and saved to the file before shutdown.
/// TODO: Should build upon this: https://learn.microsoft.com/en-us/dotnet/core/extensions/configuration#configuration-providers
/// </summary>
public static class Settings
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(Settings));

    public static Units Units { get; set; } = Units.Metric;

    public static ScreenMode ScreenMode { get; set; } = ScreenMode.System;

    public static bool AcceptedLicenseTerms { get; set; }

    public static DateTimeOffset LastAutomaticCheckForUpdates { get; set; } = DateTimeOffset.MinValue;

    public static bool AutomaticallyCheckForUpdates { get; set; } = true;

    public static Dictionary<string, LocationData> Locations  { get; set; } = new();
    
    public static string CountryCode { get; set; } = string.Empty;
    
    public static bool SpecifyCountryCode {  get; set; }

    public static int CurrentLocationIndex { get; set; }

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

    public static int LocationIndexFromId(string locationId)
    {
        return LocationsData.Locations.FindIndex(location => location.Id == locationId);
    }

    private static readonly string SettingsFolder = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "EBT Weather");

    private static readonly string SettingsPath = Path.Combine(SettingsFolder,  "settings.json");

    private class PersistedSettings
    {
        public Units Units { get; set; }

        public ScreenMode ScreenMode { get; set; }

        public bool AcceptedLicenseTerms { get; set; }

        public DateTimeOffset LastAutomaticCheckForUpdates { get; set; }

        public bool AutomaticallyCheckForUpdates { get; set; } = true;

        public Dictionary<string, LocationData> Locations  { get; set; }
    
        public string CountryCode { get; set; } = string.Empty;
    
        public bool SpecifyCountryCode {  get; set; }

        public int CurrentLocationIndex { get; set; }
    }

    public static void Load()
    {
        Log.Info($"Load from: \"{SettingsPath}\"");

        if (File.Exists(SettingsPath))
        {
            try
            {
                var json = File.ReadAllText(SettingsPath);

                var persistedSettings = JsonSerializer.Deserialize<PersistedSettings>(json) ?? new PersistedSettings();

                Units = persistedSettings.Units;
                ScreenMode = persistedSettings.ScreenMode;
                AcceptedLicenseTerms = persistedSettings.AcceptedLicenseTerms;
                LastAutomaticCheckForUpdates = persistedSettings.LastAutomaticCheckForUpdates;
                AutomaticallyCheckForUpdates = persistedSettings.AutomaticallyCheckForUpdates;
                Locations = persistedSettings.Locations;
                CountryCode = persistedSettings.CountryCode;
                SpecifyCountryCode = persistedSettings.SpecifyCountryCode;
                CurrentLocationIndex = persistedSettings.CurrentLocationIndex;
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
        
        var persistedSettings = new PersistedSettings
        {
            Units = Units,
            ScreenMode = ScreenMode,
            AcceptedLicenseTerms = AcceptedLicenseTerms,
            LastAutomaticCheckForUpdates = LastAutomaticCheckForUpdates,
            AutomaticallyCheckForUpdates = AutomaticallyCheckForUpdates,
            Locations = Locations,
            CountryCode = CountryCode,
            SpecifyCountryCode = SpecifyCountryCode,
            CurrentLocationIndex = CurrentLocationIndex,
        };
        
        var json = JsonSerializer.Serialize(persistedSettings, new JsonSerializerOptions
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
