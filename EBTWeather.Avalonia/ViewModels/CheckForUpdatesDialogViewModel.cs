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

using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EBTWeather.Avalonia.Misc;

namespace EBTWeather.Avalonia.ViewModels;

public partial class CheckForUpdatesDialogViewModel : ViewModelBase
{
    public CheckForUpdatesDialogViewModel(IHttpClientFactory httpClientFactory)
    {
        Task.Run(async () =>
        {
            var appVersion = new AppVersion(httpClientFactory);

            var updatesAreAvailable = await appVersion.IsUpdateAvailable();
            
            if (updatesAreAvailable)
            {
                Message = $"An updated version of {Constants.AppName} is available.\r\n\r\nClick the Download Updates button to visit the download web page.";
                EnableDownloadUpdates = true;
            }
            else
            {
                Message = $"You are running the latest version of {Constants.AppName}.\r\n\r\nPlease check again in the future.";
            }
        });
    }

    [ObservableProperty]
    private string _message = "Checking for updates...";

    [ObservableProperty] 
    private bool _enableDownloadUpdates;
    
    [RelayCommand]
    private void DownloadUpdates(Window window)
    {
        Process.Start(new ProcessStartInfo
            {
                FileName = Constants.DownloadUpdatesUrl,
                UseShellExecute = true
            }
        );
    }
    
    [RelayCommand]
    private void Close(Window window)
    {
        window.Close();
    }
}
