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
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using EBTWeather.Avalonia.Misc;
using EBTWeather.Avalonia.Views;

namespace EBTWeather.Avalonia.ViewModels;

public partial class AboutDialogViewModel : ViewModelBase
{
    public AboutDialogViewModel()
    {
        var version = AppVersion.RunningVersion();
        
        Version = $"{version.Major}.{version.Minor:D2}";
    }
    
    public string Version { get; private set; }
    
    [RelayCommand]
    private void Close(Window window)
    {
        window.Close();
    }

    [RelayCommand]
    private async Task ReadLicenseTerms(Window window)
    {
        await new LicenseTermsDialog().Launch(window);
    }

    [RelayCommand]
    private async Task CheckForUpdates(Window window)
    {
        await new CheckForUpdatesDialog().Launch(window);
    }
}
