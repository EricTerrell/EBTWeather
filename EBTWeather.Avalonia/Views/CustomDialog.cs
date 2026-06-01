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
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

namespace EBTWeather.Avalonia.Views;

public class CustomDialog<TViewModel> : Window where TViewModel : ObservableObject
{
    private void PrepareForLaunch()
    {
        DataContext = (Application.Current as App)!.ServiceProvider.GetRequiredService<TViewModel>();

        Focus();
    }
    
    public virtual async Task LaunchAsync(Window window)
    {
        PrepareForLaunch();
        
        await ShowDialog<bool>(window);
    }

    public virtual void LaunchSync(Window window)
    {
        PrepareForLaunch();
        
        Dispatcher.UIThread.Post(() =>
        {
            ShowDialog<bool>(window);
        });
    }
}