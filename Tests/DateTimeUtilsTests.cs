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

using System.Globalization;
using EBTWeather.Avalonia.Misc;
using log4net;

namespace Tests;

public class DateTimeUtilsTests
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(DateTimeUtilsTests));

    [Test]
    public void TestToUtcMountainTime()
    {
        const string midnightText = "2026-05-12T00:00";
        var midnight = DateTime.Parse(midnightText);
        
        var localTime = DateTimeUtils.ToUtc(midnight, "America/Denver");
        var localTimeText = localTime.ToString(CultureInfo.InstalledUICulture);
        
        Assert.That(localTimeText, Is.EqualTo("5/12/2026 6:00:00 AM"));
    }

    [Test]
    public void TestIsToday()
    {
        var today = DateTime.Today;
        
        var result =  DateTimeUtils.IsToday(today);
        Assert.That(result, Is.True);
        
        result = DateTimeUtils.IsToday(DateTime.Now.AddDays(1));
        
        Assert.That(result, Is.False);
    }
}