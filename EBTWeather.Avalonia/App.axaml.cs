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
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using EBTWeather.Avalonia.Misc;
using EBTWeather.Avalonia.ViewModels;
using EBTWeather.Avalonia.Views;
using log4net;
using Microsoft.Extensions.DependencyInjection;
using Polly;

namespace EBTWeather.Avalonia;

public partial class App : Application
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(App));

    private MainWindowViewModel _mainWindowViewModel;
    
    public IServiceProvider ServiceProvider { get; private set; }
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        Settings.Load();

        DisplayUtils.UpdateScreenMode();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        Dispatcher.UIThread.UnhandledException += OnUnhandledException;
        
        var services = new ServiceCollection();
        services.AddCommonServices();
        
        AddServices(services);
        AddHttpClients(services);
        
        ServiceProvider = services.BuildServiceProvider();

        _mainWindowViewModel = ServiceProvider.GetRequiredService<MainWindowViewModel>();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = _mainWindowViewModel
            };

            desktop.Startup += (sender, args) => OnStartup();
            desktop.Exit += (sender, args) => OnShutdown();
            desktop.MainWindow.Activated += async (sender, args) => await Activated();

            if (!Settings.AcceptedLicenseTerms)
            {
                Dispatcher.UIThread.Post(async () =>
                {
                    await new LicenseTermsDialog().Launch(desktop.MainWindow);                    
                });
            }
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void OnStartup()
    {
        Log.Info("OnStartup");

        _mainWindowViewModel.StartTimer();
    }

    private void OnShutdown()
    {
        Log.Info("OnShutdown");
        
        Settings.Save();    
    }

    private async Task Activated()
    {
        Log.Info("Activated");
    }
    
    private static void AddServices(IServiceCollection services)
    {
        services.AddMemoryCache(options =>
        {
            options.ExpirationScanFrequency = Constants.CacheExpirationScanFrequency;
        });
    }

    private void AddHttpClients(IServiceCollection services)
    {
        const int defaultRetries = 1;
        var defaultTimeout = TimeSpan.FromSeconds(30);
        const int retryTimeout = 60;

        services.AddHttpClient(Constants.OpenMeteoForecastClientName, client =>
            {
                client.BaseAddress = new Uri("https://api.open-meteo.com");
                client.Timeout = defaultTimeout;
            })
            .AddTransientHttpErrorPolicy(builder => 
                builder.WaitAndRetryAsync(CreateWaits(defaultRetries, retryTimeout)));

        services.AddHttpClient(Constants.OpenMeteoHistoricalClientName, client =>
            {
                client.BaseAddress = new Uri("https://archive-api.open-meteo.com");
                client.Timeout = defaultTimeout;
            })
            .AddTransientHttpErrorPolicy(builder => 
                builder.WaitAndRetryAsync(CreateWaits(defaultRetries, retryTimeout)));

        services.AddHttpClient(Constants.OpenMeteoGeoCodingClientName, client =>
            {
                client.BaseAddress = new Uri("https://geocoding-api.open-meteo.com");
                client.Timeout = defaultTimeout;
            })
            .AddTransientHttpErrorPolicy(builder => 
                builder.WaitAndRetryAsync(CreateWaits(defaultRetries, retryTimeout)));

        services.AddHttpClient(Constants.MainWebsiteClientName, client =>
            {
                client.BaseAddress = new Uri(Constants.MainWebsiteUrl);
                client.Timeout = defaultTimeout;
            })
            .AddTransientHttpErrorPolicy(builder => 
                builder.WaitAndRetryAsync(CreateWaits(defaultRetries, retryTimeout)));
    }
    
    private TimeSpan[] CreateWaits(int retries, int retryTimeout)
    {
        var result = Enumerable.Range(1, retries).Select(item => 
            TimeSpan.FromSeconds(retryTimeout)).ToArray();
        
        var totalSeconds =  result.Sum(item => item.TotalSeconds);
        
        Log.Info($"\r\nApp.CreateWaits: retries: {retries} Total Seconds: {totalSeconds}");

        result.ToList().ForEach(wait =>
        {
            Log.Info($"App.CreateWaits: Wait: {wait}\r\n");
        });
        
        return result.ToArray();
    }
    
    private static void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        // Log the exception
        Log.Error($"OnUnhandledException: {e.Exception}");

        e.Handled = true;
    }
}