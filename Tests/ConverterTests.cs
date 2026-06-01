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
using EBTWeather.Avalonia.Converters;
using log4net;

namespace Tests;

public class ConverterUtilsTests
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(DateTimeUtilsTests));

    [Test]
    public void TestAngleConverter()
    {
        const int angle = 185;
        var conversion = new AngleConverter().Convert(angle, typeof(string), null, new CultureInfo("en-US"));
        
        Assert.That(conversion, Is.EqualTo("S"));
    }
}