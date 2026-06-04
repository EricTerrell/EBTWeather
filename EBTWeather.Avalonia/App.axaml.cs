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
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using EBTWeather.Avalonia.Misc;
using EBTWeather.Avalonia.ViewModels;
using EBTWeather.Avalonia.Views;
using log4net;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;

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
            // Should the app launch minimized?
            var minimize = desktop.Args!.ToList().FindIndex(0, arg => 
                arg.Trim().Equals(Constants.MinimizeArg, StringComparison.CurrentCultureIgnoreCase)) != -1;

            desktop.MainWindow = new MainWindow
            {
                DataContext = _mainWindowViewModel,
                WindowState = minimize ? WindowState.Minimized : WindowState.Normal
            };

            desktop.Startup += (_, _) => OnStartup();
            desktop.Exit += (_, _) => OnShutdown();

            if (!Settings.AcceptedLicenseTerms)
            { 
                new LicenseTermsDialog().LaunchSync(desktop.MainWindow);
            }
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void OnStartup()
    {
        Log.Info("OnStartup");

        Task.Run(() => _mainWindowViewModel.StartTimer()).GetAwaiter().GetResult();
    }

    private void OnShutdown()
    {
        Log.Info("OnShutdown");
    }

    private static void AddServices(IServiceCollection services)
    {
        services.AddMemoryCache(options =>
        {
            options.ExpirationScanFrequency = Constants.CacheExpirationScanFrequency;
        });
    }

    private IAsyncPolicy<HttpResponseMessage> CreateRetryPolicy(int retries, TimeSpan sleepDuration)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .Or<TimeoutRejectedException>() // thrown by Polly's TimeoutPolicy if the inner call times out
            .WaitAndRetryAsync(
                retryCount: retries,
                sleepDurationProvider: retryAttempt => sleepDuration,
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    Log.Warn($"Retry {retryCount} after {timeSpan.TotalSeconds} seconds due to: {exception.Exception.Message}");
                });
    }

    private IAsyncPolicy<HttpResponseMessage> CreatePerTryTimeoutPolicy(int eachTryTimeoutSeconds)
    {
        return Policy.TimeoutAsync<HttpResponseMessage>(eachTryTimeoutSeconds);
    }

    /// <summary>
    /// Add pre-configured HttpClients that can be used by the app.
    /// Note: The global HttpClient.Timeout value is not specified. If you set the global HttpClient.Timeout to a value
    /// that is shorter than your Polly retry delays, the request will be canceled globally before Polly has a chance to
    /// retry.
    /// https://github.com/App-vNext/Polly/wiki/Polly-and-HttpClientFactory
    /// </summary>
    /// <param name="services"></param>
    private void AddHttpClients(IServiceCollection services)
    {
        const int eachTryTimeoutSeconds = 15;
        var sleepTime = TimeSpan.FromSeconds(1);
        
        services.AddHttpClient(Constants.OpenMeteoForecastClientName, client =>
            {
                client.BaseAddress = new Uri("https://api.open-meteo.com");
            })
            .AddPolicyHandler(CreateRetryPolicy(4, sleepTime))
            .AddPolicyHandler(CreatePerTryTimeoutPolicy(eachTryTimeoutSeconds));

        services.AddHttpClient(Constants.OpenMeteoHistoricalClientName, client =>
            {
                client.BaseAddress = new Uri("https://archive-api.open-meteo.com");
            })
            .AddPolicyHandler(CreateRetryPolicy(4, sleepTime))
            .AddPolicyHandler(CreatePerTryTimeoutPolicy(eachTryTimeoutSeconds));

        services.AddHttpClient(Constants.OpenMeteoGeoCodingClientName, client =>
            {
                client.BaseAddress = new Uri("https://geocoding-api.open-meteo.com");
            })
            .AddPolicyHandler(CreateRetryPolicy(2, sleepTime))
            .AddPolicyHandler(CreatePerTryTimeoutPolicy(eachTryTimeoutSeconds));

        services.AddHttpClient(Constants.MainWebsiteClientName, client =>
            {
                client.BaseAddress = new Uri(Constants.MainWebsiteUrl);
            })
            .AddPolicyHandler(CreateRetryPolicy(5, sleepTime))
            .AddPolicyHandler(CreatePerTryTimeoutPolicy(eachTryTimeoutSeconds));
    }
    
    private static void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        // Log the exception
        Log.Error($"OnUnhandledException: {e.Exception}");

        e.Handled = true;
    }
}