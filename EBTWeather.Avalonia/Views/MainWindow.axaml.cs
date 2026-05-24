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

using Avalonia.Controls;
using Avalonia.Interactivity;
using EBTWeather.Avalonia.ViewModels;
using log4net;

namespace EBTWeather.Avalonia.Views;

public partial class MainWindow : Window
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(MainWindow));

    public MainWindow()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        
        // Set focus to something so that keyboard accelerators will show when user clicks Alt key.
        TabControl.Focus();
    }

    private void Calendar_OnSelectedDatesChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (DataContext is MainWindowViewModel vm && sender is Calendar calendar)
        {
            Log.Info($"Calendar_OnSelectedDatesChanged: Selected Dates: {calendar.SelectedDates}");

            vm.HistoricalDates = calendar.SelectedDates;
        }
    }
}