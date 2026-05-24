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

using System.Linq;
using System.Reflection;
using EBTWeather.Avalonia.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace EBTWeather.Avalonia.Misc;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Register view models so that they can be dynamically instantiated with dependency injection.
    /// </summary>
    /// <param name="collection"></param>
    public static void AddCommonServices(this IServiceCollection collection)
    {
        collection.AddTransient<MainWindowViewModel>();
        
        Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.Namespace == "EBTWeather.Avalonia.ViewModels" && t.Name.EndsWith("DialogViewModel"))
            .ToList()
            .ForEach(t =>
            {
                collection.AddTransient(t);
            });
    }
}