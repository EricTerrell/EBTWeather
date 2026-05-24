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

namespace EBTWeather.Avalonia.Views;

public partial class LicenseTermsDialog : CustomDialog<LicenseTermsDialogViewModel>
{
    public LicenseTermsDialog()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        OkButton.Focus();
    }

    // Do not allow the user to close the dialog by clicking the "x" on the dialog title bar.
    protected override void OnClosing(WindowClosingEventArgs e)
    {
        if (Tag is bool canClose)
        {
            if (!canClose)
            {
                e.Cancel = true;
            }
            else
            {
                base.OnClosing(e);
            }
        }
        else
        {
            e.Cancel = true;
        }
    }
}