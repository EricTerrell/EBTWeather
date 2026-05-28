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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using EBTWeather.Avalonia.Messages;
using EBTWeather.Avalonia.Misc;
using EBTWeather.Avalonia.Models;
using EBTWeather.Avalonia.UnitValues;
using EBTWeather.Avalonia.Views;
using EBTWeather.Avalonia.WeatherData;
using EBTWeather.Avalonia.WebService;
using log4net;
using Microsoft.Extensions.Caching.Memory;

namespace EBTWeather.Avalonia.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(MainWindowViewModel));

    public MainWindowViewModel(IMemoryCache cache, IHttpClientFactory httpClientFactory)
    {
        _cache = cache;
        HttpClientFactory = httpClientFactory;

        UpdateSuffixes();

        LocationData = [];
        Settings.LocationsData.Locations.ForEach(location => { LocationData.Add(location); });

        _openMeteo = new OpenMeteo(HttpClientFactory);

        // Display information about menu items as each one is highlighted
        WeakReferenceMessenger.Default.Register<MainWindowViewModel, MenuMessage>
        (this, (vm, msg) => { ProcessMenuMessage(msg.MenuName, msg.Selected); }
        );
        
        // We may need to launch the ManageLocationsDialog after the LicenseTermsDialog is closed.
        WeakReferenceMessenger.Default.Register<MainWindowViewModel, ClosedLicenseTermsDialog>
        (this, (vm, msg) => { ProcessClosedLicenseTermsDialog(); }
        );
    }

    // Allow XAML Preview to work
    public MainWindowViewModel()
    {
    }

    private readonly IMemoryCache _cache;

    public IHttpClientFactory HttpClientFactory { get; }

    // DispatcherTimer doesn't cause the PC to not sleep when app is running.
    private DispatcherTimer _timer;

    // Query web service every 5 minutes
    private const int TimerInterval = Constants.WebServiceCallFrequencyMinutes * 60 * 1000;

    [ObservableProperty] private string _temperatureSuffix;

    [ObservableProperty] private string _visibilitySuffix;

    [ObservableProperty] private string _speedSuffix;

    [ObservableProperty] private string _airPressureSuffix;

    [ObservableProperty] private string _precipitationSuffix;

    public ObservableCollection<LocationData> LocationData { get; set; }

    [ObservableProperty] private int? _currentLocationIndex = Settings.CurrentLocationIndex;

    public static DateTime HistoricalMinDate => new(1950, 1, 1);

    public static DateTime HistoricalMaxDate { get; set; } = DateTime.Now.AddDays(-1);

    [ObservableProperty] private DateOnly? _historicalSelectedMinDate;

    [ObservableProperty] private DateOnly? _historicalSelectedMaxDate;

    public SelectedDatesCollection? HistoricalDates
    {
        get;
        set
        {
            Log.Info($"HistoricalDates: updating to: {value}");

            field = value;

            HistoricalSelectedMinDate = DateOnly.FromDateTime(value!.Min());
            HistoricalSelectedMaxDate = DateOnly.FromDateTime(value!.Max());

            Task.Run(async () => { await UpdateHistoricalWeatherData(); });
        }
    }

    partial void OnCurrentLocationIndexChanged(int? value)
    {
        Log.Info("OnCurrentLocationIndexChanged");

        Settings.CurrentLocationIndex = value!.Value;

        // Want to clear the current and historical weather displays. It can be confusing when the retrieval of the
        // new data is slow. In this case the user could incorrectly assume the data on the screen corresponds with the
        // new location.
        CurrentAndFutureWeather = null;
        HistoricalWeather = null;

        UpdateWeatherData();
    }

    [ObservableProperty] private string _statusMessage;

    [ObservableProperty] private WeatherInfo? _CurrentAndFutureWeather;

    [ObservableProperty] private HistoricalWeatherInfo? _historicalWeather;

    private readonly OpenMeteo _openMeteo;

    [RelayCommand]
    private async Task LaunchSettingsDialog(Window window)
    {
        await new SettingsDialog().Launch(window);

        UpdateSuffixes();

        // Refresh weather display in case units changed.
        var saveWeatherInfo = CurrentAndFutureWeather;
        CurrentAndFutureWeather = null;
        CurrentAndFutureWeather = saveWeatherInfo;
    }

    [RelayCommand]
    private async Task LaunchLicenseTermsDialog(Window window)
    {
        await new LicenseTermsDialog().Launch(window);
    }

    [RelayCommand]
    private async Task LaunchAboutDialog(Window window)
    {
        await new AboutDialog().Launch(window);
    }

    [RelayCommand]
    public async Task LaunchManageLocationsDialog(Window window)
    {
        await new ManageLocationsDialog().Launch(window);

        RefreshLocationsData();
    }

    private void RefreshLocationsData()
    {
        var saveIndex = Settings.CurrentLocationIndex;

        LocationData.Clear();
        CurrentLocationIndex = -1;

        Settings.LocationsData.Locations.ForEach(location => { LocationData.Add(location); });

        CurrentLocationIndex = saveIndex < Settings.LocationsData.Locations.Count ? saveIndex : -1;

        if (CurrentLocationIndex < 0 || CurrentLocationIndex >= Settings.LocationsData.Locations.Count)
        {
            CurrentAndFutureWeather = null;
            HistoricalWeather = null;
        }
    }

    [RelayCommand]
    private void FileExit(Window window)
    {
        window.Close();
    }

    public async Task StartTimer()
    {
        Log.Info("StartTimer");

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(TimerInterval)
        };

        _timer.Tick += async (sender, o) =>
        {
            Log.Info("timer tick");

            // Need to update max date on date picker in case the app ran past midnight.
            HistoricalMaxDate = DateTime.Now.AddDays(-1);
                
            await CheckForUpdates();

            await UpdateWeatherData();
        };

        _timer.Start();
        await UpdateWeatherData();
    }

    private async Task CheckForUpdates()
    {
        if (Settings.AutomaticallyCheckForUpdates)
        {
            var now = DateTimeOffset.Now;
            var elapsedTime = now - Settings.LastAutomaticCheckForUpdates;

            if (elapsedTime >= Constants.CheckForUpdatesInterval)
            {
                Log.Info(
                    $"Checking for updates now={now} LastCheckForUpdates={Settings.LastAutomaticCheckForUpdates} elapsedTime={elapsedTime} ({elapsedTime.Days} days)");

                var isUpdateAvailable = await new AppVersion(HttpClientFactory).IsUpdateAvailable();
                Settings.LastAutomaticCheckForUpdates = DateTimeOffset.Now;
                
                if (isUpdateAvailable && ApplicationUtils.GetMainWindow() is { } window)
                {
                    Log.Info("Launching check for updates dialog");

                    await HelpCheckForUpdates(window);
                }
            }
        }
    }

    public async Task UpdateWeatherData()
    {
        if (CurrentLocationIndex >= 0 && CurrentLocationIndex < Settings.LocationsData.Locations.Count)
        {
            StatusMessage = $"Retrieving weather data at {DateTime.Now.ToLongTimeString()}";

            var location = Settings.LocationsData.Locations[CurrentLocationIndex.Value];

            var error = false;

            try
            {
                CurrentAndFutureWeather = await _openMeteo.GetCachedCurrentWeather(location, _cache);
                await UpdateHistoricalWeatherData();
            }
            catch (Exception ex)
            {
                Log.Error(ex);

                error = true;
                StatusMessage = $"Error: {ex.Message}";
            }

            if (!error)
            {
                StatusMessage = $"Retrieved weather data at {DateTime.Now.ToLongTimeString()}";
            }
        }
    }

    private async Task UpdateHistoricalWeatherData()
    {
        if (HistoricalSelectedMinDate != null && HistoricalSelectedMaxDate != null && 
            CurrentLocationIndex >= 0 && CurrentLocationIndex < Settings.LocationsData.Locations.Count)
        {
            Log.Info(
                $"UpdateHistoricalWeatherData: {HistoricalSelectedMinDate.Value} {HistoricalSelectedMaxDate.Value}");

            var location = Settings.LocationsData.Locations[CurrentLocationIndex.Value];

            StatusMessage = $"Retrieving historical weather data at {DateTime.Now.ToLongTimeString()}";

            HistoricalWeather = await _openMeteo.GetCachedHistoricalWeather(
                location, HistoricalSelectedMinDate.Value, HistoricalSelectedMaxDate.Value, _cache);

            StatusMessage = $"Retrieved historical weather data at {DateTime.Now.ToLongTimeString()}";
        }
    }

    private void UpdateSuffixes()
    {
        TemperatureSuffix = Settings.Units == Units.Metric ? Temperature.MetricSuffix : Temperature.USASuffix;

        SpeedSuffix = Settings.Units == Units.Metric ? Speed.MetricSuffix : Speed.USASuffix;

        AirPressureSuffix = Settings.Units == Units.Metric ? Pressure.MetricSuffix : Pressure.USASuffix;

        VisibilitySuffix = Settings.Units == Units.Metric ? Visibility.MetricSuffix : Visibility.USASuffix;

        PrecipitationSuffix = Settings.Units == Units.Metric ? Precipitation.MetricSuffix : Precipitation.USASuffix;
    }

    [RelayCommand]
    private void HelpFeedback()
    {
        var mailtoUrl = $"mailto:{Constants.SupportEmail}?subject={Constants.AppName} Feedback";

        Process.Start(new ProcessStartInfo
            {
                FileName = mailtoUrl,
                UseShellExecute = true
            }
        );
    }

    [RelayCommand]
    private void HelpVisit()
    {
        Process.Start(new ProcessStartInfo
            {
                FileName = Constants.MainWebsiteUrl,
                UseShellExecute = true
            }
        );
    }

    [RelayCommand]
    private async Task HelpCheckForUpdates(Window window)
    {
        await new CheckForUpdatesDialog().Launch(window);
    }

    /// <summary>
    /// Display help text for menu items in status bar when each menu item is highlighted.
    /// </summary>
    /// <param name="menuName">Name of menu item</param>
    /// <param name="selected">menu item selection flag</param>
    private void ProcessMenuMessage(string menuName, bool selected)
    {
        var text = string.Empty;

        if (Application.Current!.TryGetResource(menuName, out var resource))
        {
            text = resource as string ?? string.Empty;
        }

        StatusMessage = selected ? $"{text}" : string.Empty;
    }

    /// <summary>
    /// Allow the user to search for locations if none have been saved previously.
    /// </summary>
    private void ProcessClosedLicenseTermsDialog()
    {
        if (Settings.Locations.Count == 0)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                LaunchManageLocationsDialog(ApplicationUtils.GetMainWindow()!);
            });
        }
    }
}