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
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EBTWeather.Avalonia.Misc;
using EBTWeather.Avalonia.Models;
using EBTWeather.Avalonia.UnitValues;
using EBTWeather.Avalonia.Views;
using EBTWeather.Avalonia.WebService;
using log4net;

namespace EBTWeather.Avalonia.ViewModels;

public partial class ManageLocationsDialogViewModel : ObservableValidator
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(ManageLocationsDialogViewModel));

    public ManageLocationsDialogViewModel(IHttpClientFactory httpClientFactory)
    {
        _openMeteo = new OpenMeteo(httpClientFactory);

        UpdateSuffix();

        if (Settings.CurrentLocationIndex >= 0 &&
            Settings.CurrentLocationIndex < Settings.LocationsData.Locations.Count)
        {
            _selectedLocationId = Settings.LocationsData.Locations[Settings.CurrentLocationIndex].Id;
        }
    }

    // Allow XAML Preview to work
    public ManageLocationsDialogViewModel()
    {
    }

    private void UpdateSuffix()
    {
        ElevationSuffix = (Settings.Units == Units.USA ? ShortDistance.USASuffix : ShortDistance.MetricSuffix).Trim();
    }
    
    private string _selectedLocationId = string.Empty;
    
    private const int MinElevation = -1500;
    private const int MaxElevation = 30000;

    private const int MaxLatitudeDegrees = 90;
    private const int MaxLongitudeDegrees = 180;

    private const int MaxMinutes = 59;
    private const int MaxSeconds = 60;

    [ObservableProperty]
    [Required(ErrorMessage = "Location name is required")]
    [NotifyCanExecuteChangedFor(nameof(SearchCommand))]
    private string _locationName = string.Empty;

    [ObservableProperty] 
    [Required] 
    [Range(0, MaxLatitudeDegrees)] 
    [NotifyCanExecuteChangedFor(nameof(AddCommand))]
    private int? _latitudeDegrees = 0;

    [ObservableProperty] 
    [Required] 
    [Range(0, MaxMinutes)] 
    [NotifyCanExecuteChangedFor(nameof(AddCommand))]
    private int? _latitudeMinutes = 0;

    [ObservableProperty]
    [Required]
    [NotifyCanExecuteChangedFor(nameof(AddCommand))]
    [CustomValidation(typeof(ManageLocationsDialogViewModel), nameof(ValidateSeconds))]
    private double? _latitudeSeconds = 0.0;

    [ObservableProperty] private int? _latitudeDirection = 0;

    [ObservableProperty] 
    [Required] 
    [Range(0, MaxLongitudeDegrees)] 
    [NotifyCanExecuteChangedFor(nameof(AddCommand))]
    private int? _longitudeDegrees = 0;

    [ObservableProperty] 
    [Required] 
    [Range(0, MaxMinutes)] 
    [NotifyCanExecuteChangedFor(nameof(AddCommand))]
    private int? _longitudeMinutes = 0;

    [ObservableProperty] 
    private int? _longitudeDirection = 1;

    [ObservableProperty]
    [Required]
    [NotifyCanExecuteChangedFor(nameof(AddCommand))]
    [CustomValidation(typeof(ManageLocationsDialogViewModel), nameof(ValidateSeconds))]
    private double? _longitudeSeconds = 0.0;

    public ObservableCollection<LocationData> Locations { get; set; } = [];

    public ObservableCollection<LocationData> SavedLocationsData { get; set; } = new(Settings.LocationsData.Locations);

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SearchCommand))]
    private bool _specifyCountryCode = Settings.SpecifyCountryCode;

    partial void OnSpecifyCountryCodeChanged(bool value)
    {
        Settings.SpecifyCountryCode = value;
        
        ValidateProperty(CountryCode, nameof(CountryCode));
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SearchCommand))]
    [CustomValidation(typeof(ManageLocationsDialogViewModel), nameof(ValidateCountryCode))]
    private string _countryCode = Settings.CountryCode;

    partial void OnCountryCodeChanged(string? value)
    {
        Settings.CountryCode = value!;
    }
    
    public static ValidationResult? ValidateCountryCode(string? countryCode, ValidationContext context)
    {
        var result = ValidationResult.Success;
        
        if (context.ObjectInstance is ManageLocationsDialogViewModel manageLocationsDialogViewModel)
        {
            if (manageLocationsDialogViewModel.SpecifyCountryCode)
            {
                if (string.IsNullOrWhiteSpace(manageLocationsDialogViewModel.CountryCode))
                {
                    result = new ValidationResult("Country code is required");
                }
            }
        }
        
        return result;
    }

    private readonly OpenMeteo _openMeteo;

    [RelayCommand(CanExecute = nameof(CanSearch))]
    public async Task Search()
    {
        ErrorMessage = string.Empty;
        
        try
        {
            var result = await
                _openMeteo.GetGeoLocations(LocationName, SpecifyCountryCode ? CountryCode : null);

            Locations.Clear();
            result.Locations.ForEach(locationData => Locations.Add(locationData));
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error: {ex.Message}";
        }
    }

    public bool CanSearch()
    {
        return LocationName.Trim().Length > 0 && (!SpecifyCountryCode || CountryCode.Trim().Length > 0);
    }
    
    [RelayCommand]
    public void AddLocation(LocationData location)
    {
        Settings.Locations.Remove(location.Id);
        Settings.Locations.Add(location.Id, location);

        ReloadLocations();
        
        _selectedLocationId = location.Id;
    }

    [RelayCommand]
    public void DeleteLocation(LocationData location)
    {
        Settings.Locations.Remove(location.Id);
        
        ReloadLocations();
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddCommand))]
    [Required(ErrorMessage = "Location name is required")]
    private string? _addLocationName;

    [ObservableProperty]
    [Required(ErrorMessage = "State/Province is required")]
    [NotifyCanExecuteChangedFor(nameof(AddCommand))]
    private string? _stateProvince;

    [ObservableProperty]
    [Required(ErrorMessage = "Country Code is required")]
    [NotifyCanExecuteChangedFor(nameof(AddCommand))]
    private string? _addCountryCode;

    [ObservableProperty]
    [Required(ErrorMessage = "Elevation is required")]
    [Range(MinElevation, MaxElevation)]
    [NotifyCanExecuteChangedFor(nameof(AddCommand))]
    private int? _elevation;

    [ObservableProperty] 
    private string _elevationSuffix;
    
    public string ShortDistanceSuffix => 
        Settings.Units == Units.Metric ? ShortDistance.MetricSuffix : ShortDistance.USASuffix;

    [ObservableProperty]
    private string _errorMessage;
    
    [RelayCommand(CanExecute = nameof(CanAdd))]
    private void Add()
    {
        var latitude = AngleUtils.DMSToDecimalDegrees(LatitudeDegrees.Value, LatitudeMinutes.Value,
            LatitudeSeconds.Value);

        if (LatitudeDirection == 1)
        {
            latitude = -latitude;
        }

        var longitude = AngleUtils.DMSToDecimalDegrees(LongitudeDegrees.Value, LongitudeMinutes.Value,
            LongitudeSeconds!.Value);

        if (LongitudeDirection == 1)
        {
            longitude = -longitude;
        }

        var metricElevation = Elevation!.Value;

        if (Settings.Units == Units.USA)
        {
            metricElevation = (int) UnitsNet.Length.FromFeet(Elevation!.Value).Meters;
        }

        var location = new LocationData(
            null,
            AddLocationName,
            new GeoLocation(latitude, longitude, new ShortDistance(metricElevation)),
            AddCountryCode!.Trim().ToUpper(),
            StateProvince!.Trim().ToUpper());

        location.Id = location.CalculateId();
        
        Settings.Locations[location.Id] = location;
        _selectedLocationId = location.Id;
        
        ReloadLocations();
    }

    private bool CanAdd()
    {
        return AddLocationName != null && AddLocationName.Trim().Length > 0
                                       && InRange(LatitudeDegrees, 0, MaxLatitudeDegrees)
                                       && InRange(LatitudeMinutes, 0, MaxMinutes)
                                       && InRangeSeconds(LatitudeSeconds)
                                       && InRange(LongitudeDegrees, 0, MaxLongitudeDegrees)
                                       && InRange(LongitudeMinutes, 0, MaxMinutes)
                                       && InRangeSeconds(LongitudeSeconds)
                                       && InRange(Elevation, MinElevation, MaxElevation);
    }

    public static ValidationResult ValidateSeconds(string value, ValidationContext context)
    {
        const string errorMessage = "Seconds must be at least zero and less than 60";

        if (double.TryParse(value, out var seconds))
        {
            if (seconds is < 0.0 or >= 60)
            {
                return new ValidationResult(errorMessage);
            }
            else
            {
                return ValidationResult.Success;
            }
        }
        else
        {
            return new ValidationResult(errorMessage);
        }
    }

    private static bool InRange(int? value, int min, int max)
    {
        return value >= min && value <= max;
    }

    private static bool InRangeSeconds(double? value)
    {
        return value is >= 0 and < MaxSeconds;
    }

    private void ReloadLocations()
    {
        SavedLocationsData.Clear();
        Settings.LocationsData.Locations.ForEach(locationData => SavedLocationsData.Add(locationData));
    }

    [RelayCommand]
    private void Close(Window window)
    {
        Settings.CurrentLocationIndex = GetLocationIndex(_selectedLocationId);
        
        window.Close();
    }

    private static int GetLocationIndex(string id)
    {
        return Settings.LocationsData.Locations.FindIndex(0, l => l.Id == id);
    }

    [RelayCommand]
    private async Task ChangeUnits(Window window)
    {
        await new SettingsDialog().Launch(window);
        
        UpdateSuffix();
    }
}