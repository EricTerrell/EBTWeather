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
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Messaging;
using EBTWeather.Avalonia.Messages;
using log4net;

namespace EBTWeather.Avalonia.Misc;

/// <summary>
/// This menu item sends a message when the menu item is selected or deselected. The receiver of the message can update
/// the UI accordingly.
/// </summary>
public class CustomMenuItem : MenuItem
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(Settings));

    public CustomMenuItem()
    {
        SelectionChanged += (sender, args) => OnSelectionChanged(sender, args);
    }
    
    protected override Type StyleKeyOverride => typeof(MenuItem);

    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.RemovedItems.Count > 0)
        {
            var menuItem = e.RemovedItems[0] as MenuItem;
            WeakReferenceMessenger.Default.Send(new MenuMessage(menuItem.Name, false));
        }

        if (e.AddedItems.Count > 0)
        {
            var menuItem = e.AddedItems[0] as MenuItem;
            WeakReferenceMessenger.Default.Send(new MenuMessage(menuItem.Name, true));
        }
    }
}