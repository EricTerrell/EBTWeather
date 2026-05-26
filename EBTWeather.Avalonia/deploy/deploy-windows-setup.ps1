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

Write-Output ""
Write-Output "Creating Windows Setup Zip File"

c:
cd "~\Documents\software development\Avalonia UI\EBTWeather\EBTWeather.Avalonia"

if (!(Test-Path "C:\temp\EBTWeather"))
{
    New-Item -Path "C:\temp\EBTWeather" -ItemType Directory
}

Compress-Archive -Path ".\setup\Output\*" -DestinationPath "C:\temp\EBTWeather\EBTWeather-win32-x64.zip" -Force

Write-Output ""
Write-Output "Finished creating Windows Setup Zip File"

Pop-Location