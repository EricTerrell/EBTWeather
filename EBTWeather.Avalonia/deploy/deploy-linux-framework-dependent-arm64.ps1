<#
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
#>

Push-Location

c:
cd "~\Documents\software development\Avalonia UI\EBTWeather\EBTWeather.Avalonia"

if (Test-Path ".\bin\Release\net10.0\linux-arm64")
{
    Remove-Item ".\bin\Release\net10.0\linux-arm64" -Recurse -Force
}

dotnet publish -c Release --os linux --self-contained false -f net10.0 --arch arm64

copy ".\Assets\app_icon.png" ".\bin\Release\net10.0\linux-arm64\publish"
copy ".\log4net.config.linux" ".\bin\Release\net10.0\linux-arm64\publish\log4net.config"

if (!(Test-Path "C:\temp\EBTWeather"))
{
    New-Item -Path "C:\temp\EBTWeather" -ItemType Directory
}

Compress-Archive -Path ".\bin\Release\net10.0\linux-arm64\publish\*" -DestinationPath "C:\temp\EBTWeather\EBTWeather-linux-arm64.zip" -Force

Pop-Location