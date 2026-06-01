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
using Avalonia;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using EBTWeather.Avalonia.Messages;
using EBTWeather.Avalonia.Misc;
using EBTWeather.Avalonia.Models;
using EBTWeather.Avalonia.UnitValues;
using EBTWeather.Avalonia.Views;
using EBTWeather.Avalonia.WebService;
using log4net;
using Microsoft.Extensions.DependencyInjection;

namespace EBTWeather.Avalonia.ViewModels;

public partial class ManageLocationsDialogViewModel : ObservableValidator
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(ManageLocationsDialogViewModel));

    public ManageLocationsDialogViewModel(IHttpClientFactory httpClientFactory)
    {
        _openMeteo = new OpenMeteo(httpClientFactory);

        UpdateSuffix();
    }

    // Allow XAML Preview to work
    public ManageLocationsDialogViewModel()
    {
    }

    private void UpdateSuffix()
    {
        ElevationSuffix = (Settings.Units == Units.USA ? ShortDistance.USASuffix : ShortDistance.MetricSuffix).Trim();
    }

    private const int MinElevation = -1500;
    private const int MaxElevation = 30000;

    private const int MaxLatitudeDegrees = 90;
    private const int MaxLongitudeDegrees = 180;

    private const int MaxMinutes = 59;
    private const int MaxSeconds = 59;

    [ObservableProperty]
    [Required(ErrorMessage = "Location name is required")]
    [NotifyCanExecuteChangedFor(nameof(SearchCommand))]
    private string _locationName = string.Empty;

    [ObservableProperty] [Required] [Range(0, MaxLatitudeDegrees)] [NotifyCanExecuteChangedFor(nameof(AddCommand))]
    private int? _latitudeDegrees;

    [ObservableProperty] [Required] [Range(0, MaxMinutes)] [NotifyCanExecuteChangedFor(nameof(AddCommand))]
    private int? _latitudeMinutes;

    [ObservableProperty]
    [Required]
    [NotifyCanExecuteChangedFor(nameof(AddCommand))]
    [CustomValidation(typeof(ManageLocationsDialogViewModel), nameof(ValidateSeconds))]
    private int? _latitudeSeconds;

    [ObservableProperty] private int? _latitudeDirection = 0;

    [ObservableProperty] [Required] [Range(0, MaxLongitudeDegrees)] [NotifyCanExecuteChangedFor(nameof(AddCommand))]
    private int? _longitudeDegrees;

    [ObservableProperty] [Required] [Range(0, MaxMinutes)] [NotifyCanExecuteChangedFor(nameof(AddCommand))]
    private int? _longitudeMinutes;

    [ObservableProperty] private int? _longitudeDirection = 1;

    [ObservableProperty]
    [Required]
    [NotifyCanExecuteChangedFor(nameof(AddCommand))]
    [CustomValidation(typeof(ManageLocationsDialogViewModel), nameof(ValidateSeconds))]
    private int? _longitudeSeconds;

    public ObservableCollection<LocationData> Locations { get; set; } = [];

    public ObservableCollection<LocationData> SavedLocationsData { get; set; } = new(Settings.LocationsData.Locations);

    [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(SearchCommand))]
    private bool _specifyCountryCode = Settings.SpecifyCountryCode;

    partial void OnSpecifyCountryCodeChanged(bool value)
    {
        Settings.SpecifyCountryCode = value;

        ValidateProperty(SearchCountryCode, nameof(SearchCountryCode));
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SearchCommand))]
    [CustomValidation(typeof(ManageLocationsDialogViewModel), nameof(ValidateCountryCode))]
    private string _searchCountryCode = Settings.CountryCode;

    partial void OnSearchCountryCodeChanged(string? value)
    {
        Settings.CountryCode = value!;
    }

    public static ValidationResult? ValidateCountryCode(string? countryCode, ValidationContext context)
    {
        var result = ValidationResult.Success;

        if (context.ObjectInstance is ManageLocationsDialogViewModel { SpecifyCountryCode: true } 
                manageLocationsDialogViewModel)
        {
            if (string.IsNullOrWhiteSpace(manageLocationsDialogViewModel.SearchCountryCode))
            {
                result = new ValidationResult("Country code is required");
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
                _openMeteo.GetGeoLocations(LocationName, SpecifyCountryCode ? SearchCountryCode : null);

            if (result.Locations.Count == 0)
            {
                WeakReferenceMessenger.Default.Send(new ToastMessage($"No locations found for \"{LocationName}\""));
            }
            
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
        return LocationName.Trim().Length > 0 && (!SpecifyCountryCode || SearchCountryCode.Trim().Length > 0);
    }

    [RelayCommand]
    public void AddLocation(LocationData location)
    {
        Settings.Locations.Remove(location.Id);
        Settings.Locations.Add(location.Id, location);
        
        WeakReferenceMessenger.Default.Send(new ToastMessage($"Added \"{location}\""));

        ReloadLocations();

        Settings.CurrentLocationIndex = Settings.LocationIndexFromId(location.Id);
    }

    /// <summary>
    /// Delete the specified location. Try to keep Settings.CurrentLocationIndex pointing to the correct location,
    /// unless the current location is the one being deleted.
    /// </summary>
    /// <param name="location">Location to delete</param>
    [RelayCommand]
    public void DeleteLocation(LocationData location)
    {
        string? selectedLocationId = null;
        
        if (Settings.CurrentLocationIndex >= 0 &&
            Settings.CurrentLocationIndex < Settings.LocationsData.Locations.Count)
        {
            // Get the id of the currently selected location
            selectedLocationId = Settings.LocationsData.Locations[Settings.CurrentLocationIndex].Id;
        }

        Settings.Locations.Remove(location.Id);

        WeakReferenceMessenger.Default.Send(new ToastMessage($"Deleted \"{location}\""));

        ReloadLocations();

        if (selectedLocationId != null)
        {
            Settings.CurrentLocationIndex = Settings.LocationIndexFromId(selectedLocationId);
        }
    }

    [RelayCommand]
    public async Task EditLocation(object parameter)
    {
        if (parameter is object[] parameters)
        {
            var location = parameters[0] as LocationData;
            var window = parameters[1] as Window;

            var viewModel = 
                (Application.Current as App)!.ServiceProvider.GetRequiredService<ManageLocationsDialogViewModel>();
        
            viewModel.AddLocationId = location!.Id;
            viewModel.AddLocationName = location.Name;

            var latitude = AngleUtils.ConvertToDms(location.GeoLocation.Latitude);

            viewModel.LatitudeDegrees = Math.Abs(latitude.Degrees);
            viewModel.LatitudeDirection = AngleUtils.DegreesToDirection(latitude.Degrees);

            viewModel.LatitudeMinutes = latitude.Minutes;
            viewModel.LatitudeSeconds = latitude.Seconds;
        
            var longitude = AngleUtils.ConvertToDms(location.GeoLocation.Longitude);

            viewModel.LongitudeDegrees = Math.Abs(longitude.Degrees);
            viewModel.LongitudeDirection = AngleUtils.DegreesToDirection(longitude.Degrees);

            viewModel.LongitudeMinutes = longitude.Minutes;
            viewModel.LongitudeSeconds = longitude.Seconds;

            var elevation = location.GeoLocation.Elevation.MetricValue;

            if (Settings.Units == Units.USA)
            {
                elevation = UnitsNet.Length.FromMeters(elevation).Feet;
            }

            viewModel.Elevation = (int) Math.Round(elevation);

            viewModel.StateProvince = location.Admin1;
            viewModel.AddCountryCode = location.CountryCode;

            var editLocationDialog = new EditLocationDialog
            {
                DataContext = viewModel
            };

            await editLocationDialog.ShowDialog(window!);
            
            ReloadLocations();
        }
    }

    public string? AddLocationId { get; set; }
    
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

    [ObservableProperty] private string _elevationSuffix;

    public string ShortDistanceSuffix =>
        Settings.Units == Units.Metric ? ShortDistance.MetricSuffix : ShortDistance.USASuffix;

    [ObservableProperty] private string _errorMessage;

    [RelayCommand(CanExecute = nameof(CanAdd))]
    private void Add(Window? window)
    {
        var latitude = 
            AngleUtils.DmsToDecimalDegrees(LatitudeDegrees!.Value, LatitudeMinutes!.Value, 
                LatitudeSeconds!.Value);

        if (LatitudeDirection == 1)
        {
            latitude = -latitude;
        }

        var longitude = AngleUtils.DmsToDecimalDegrees(LongitudeDegrees!.Value, LongitudeMinutes!.Value,
            LongitudeSeconds!.Value);

        if (LongitudeDirection == 1)
        {
            longitude = -longitude;
        }

        var elevation = Elevation!.Value;

        if (Settings.Units == Units.USA)
        {
            elevation = (int) UnitsNet.Length.FromFeet(Elevation!.Value).Meters;
        }

        var location = new LocationData(
            null,
            AddLocationName!,
            new GeoLocation(latitude, longitude, new ShortDistance(elevation)),
            AddCountryCode!.Trim(),
            StateProvince!.Trim());

        location.Id = location.CalculateId();

        Settings.Locations[location.Id] = location;

        var message = window == null ? $"Added \"{location}\"" : $"Updated \"{location}\"";
        WeakReferenceMessenger.Default.Send(new ToastMessage(message));

        // Delete previous value if we're editing an existing value.
        if (AddLocationId != null)
        {
            if (AddLocationId != location.Id)
            {
                Settings.Locations.Remove(AddLocationId);
            }

            AddLocationId = null;

            // Close the window if a window was specified.
            window?.Close();
        }
        
        Settings.CurrentLocationIndex = Settings.LocationIndexFromId(location.Id);

        ReloadLocations();
    }

    private bool CanAdd()
    {
        return !string.IsNullOrWhiteSpace(AddLocationName)
               && !string.IsNullOrWhiteSpace(AddCountryCode)
               && InRange(LatitudeDegrees, 0, MaxLatitudeDegrees)
               && InRange(LatitudeMinutes, 0, MaxMinutes)
               && InRange(LatitudeSeconds, 0, MaxSeconds)
               && InRange(LongitudeDegrees, 0, MaxLongitudeDegrees)
               && InRange(LongitudeMinutes, 0, MaxMinutes)
               && InRange(LongitudeSeconds, 0, MaxSeconds)
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

    private void ReloadLocations()
    {
        SavedLocationsData.Clear();
        Settings.LocationsData.Locations.ForEach(locationData => SavedLocationsData.Add(locationData));
    }

    [RelayCommand]
    private void Close(Window window)
    {
        window.Close();
    }

    [RelayCommand]
    private async Task ChangeUnits(Window window)
    {
        await new SettingsDialog().LaunchAsync(window);

        UpdateSuffix();
    }
}