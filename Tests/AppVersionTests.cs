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

using EBTWeather.Avalonia.Misc;
using log4net;
using Moq;

namespace Tests;

public class AppVersionTests
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(AppVersionTests));

    private IHttpClientFactory  _httpClientFactory;

    private AppVersion _appVersion;
    
    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        var defaultTimeout = TimeSpan.FromMinutes(1);
        
        var mockIHttpClientFactory = new Mock<IHttpClientFactory>();
        mockIHttpClientFactory.Setup(factory => factory.CreateClient(Constants.MainWebsiteClientName))
            .Returns(new HttpClient
            {
                BaseAddress = new Uri(Constants.MainWebsiteUrl),
                Timeout = defaultTimeout
            });
        
        _httpClientFactory = mockIHttpClientFactory.Object;
        
        _appVersion = new(_httpClientFactory);
    }

    [OneTimeTearDown]
    public void OnTimeTearDown()
    {
    }
    
    [Test]
    public async Task TestRetrieveCurrentVersion()
    {
        var latestVersion = await _appVersion.GetLatestVersion(_httpClientFactory);
        
        Assert.Multiple(() =>
        {
            Assert.That(latestVersion!.Major, Is.EqualTo(1));
            Assert.That(latestVersion.Minor, Is.EqualTo(0));
        });
    }
}