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
using Avalonia.Controls.Notifications;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.Messaging;
using EBTWeather.Avalonia.Messages;
using EBTWeather.Avalonia.ViewModels;

namespace EBTWeather.Avalonia.Views;

public partial class ManageLocationsDialog : CustomDialog<ManageLocationsDialogViewModel>
{
    public ManageLocationsDialog()
    {
        InitializeComponent();

        _notificationManager = new WindowNotificationManager(this)
        {
            Position = NotificationPosition.BottomCenter,
            MaxItems = 1
        };
        
        // Display information about menu items as each one is highlighted
        WeakReferenceMessenger.Default.Register<ManageLocationsDialog, ToastMessage>
        (this, (_, msg) => { ShowToast(msg.ToastText); }
        );
    }

    private readonly WindowNotificationManager _notificationManager;
    
    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        LocationNameTextBox.Focus();
    }

    private void ShowToast(string message)
    {
        _notificationManager.Show(new Notification(
            title: null, 
            message: message, 
            type: NotificationType.Information, 
            expiration: TimeSpan.FromSeconds(2.5)
        ));
    }
}