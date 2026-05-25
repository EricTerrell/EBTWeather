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
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using EBTWeather.Avalonia.Misc;
using EBTWeather.Avalonia.Models;
using EBTWeather.Avalonia.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace EBTWeather.Avalonia.Views;

public partial class EditLocationDialog : CustomDialog<ManageLocationsDialogViewModel>
{
    public EditLocationDialog()
    {
        InitializeComponent();
    }
    
    public virtual async Task Launch(Window window, LocationData location)
    {
        var viewModel = 
            (Application.Current as App)!.ServiceProvider.GetRequiredService<ManageLocationsDialogViewModel>();
        
        viewModel.AddLocationId = location.Id;
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

        DataContext = viewModel;

        await ShowDialog<bool>(window);
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        LocationView.EnterLocationNameTextBox.Focus();
    }
}