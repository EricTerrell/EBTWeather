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

using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using EBTWeather.Avalonia.Misc;

namespace EBTWeather.Avalonia.ViewModels;

public partial class SettingsDialogViewModel : ViewModelBase
{
    public Units[] AllUnits { get; set; } = [Units.Metric, Units.USA];

    public Units Units { get; set; } = Settings.Units;

    public ScreenMode[] AllScreenModes { get; set; } = [ScreenMode.System, ScreenMode.Light, ScreenMode.Dark];

    public ScreenMode ScreenMode { get; set; } = Settings.ScreenMode;
    
    public bool CheckForUpdates { get; set; } = Settings.AutomaticallyCheckForUpdates;

    public string? AirPollutionApiKey { get; set; } = Settings.AirPollutionApiKey;
    
    [RelayCommand]
    private async Task Ok(Window window)
    {
        Settings.Units = Units;
        Settings.ScreenMode = ScreenMode;
        Settings.AutomaticallyCheckForUpdates = CheckForUpdates;
        Settings.AirPollutionApiKey = AirPollutionApiKey;
            
        DisplayUtils.UpdateScreenMode();
        
        window.Close();

        var app = Application.Current as App;
        await app!.MainWindowViewModel.UpdateWeatherData(true);
    }

    [RelayCommand]
    private void Cancel(Window window)
    {
        window.Close();
    }
}
