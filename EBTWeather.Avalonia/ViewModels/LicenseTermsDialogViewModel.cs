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
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using EBTWeather.Avalonia.Messages;
using EBTWeather.Avalonia.Misc;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace EBTWeather.Avalonia.ViewModels;

public partial class LicenseTermsDialogViewModel : ViewModelBase
{
    public LicenseTermsDialogViewModel()
    {
    }

    public string LicenseTermsText { get; private set; } = ReadLicenseTermsText();

    public bool DecisionAccept { get; set; } = Settings.AcceptedLicenseTerms;

    public bool DecisionReject { get; set; } = !Settings.AcceptedLicenseTerms;
    
    private static string ReadLicenseTermsText()
    {
        var uri = new Uri("avares://EBTWeather/Assets/LicenseTerms.txt");
        
        using var stream = AssetLoader.Open(uri);
        using var reader = new StreamReader(stream);
        
        return reader.ReadToEnd();        
    }
    
    [RelayCommand]
    private async Task Close(Window window)
    {
        // Allow dialog box to be closed. We don't allow the user to click the cancel "x" on the dialog box title
        // to close it.
        window.Tag = true;

        if (DecisionReject)
        {
            var box = MessageBoxManager
                .GetMessageBoxStandard(
                    $"{Constants.AppName} License Terms", 
                    $"You have rejected the licensing terms for {Constants.AppName}.\r\n\r\nPlease uninstall {Constants.AppName} and stop using it immediately.",
                    ButtonEnum.OkCancel,
                    Icon.Warning);

            var result = await box.ShowWindowDialogAsync(window);

            if (result == ButtonResult.Ok)
            {
                Settings.AcceptedLicenseTerms = false;
                
                ApplicationUtils.Shutdown();
            }
        }
        else
        {
            Settings.AcceptedLicenseTerms = true;

            WeakReferenceMessenger.Default.Send(new ClosedLicenseTermsDialog());

            window.Close();
        }
    }
}