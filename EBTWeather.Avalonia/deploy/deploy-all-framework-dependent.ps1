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

c:
cd "~\Documents\software development\Avalonia UI\EBTWeather\EBTWeather.Avalonia\deploy"

.\deploy-linux-framework-dependent-x64.ps1

.\deploy-linux-framework-dependent-arm64.ps1

.\deploy-windows-framework-dependent.ps1

Write-Output ""
Write-Output "***************************************"
Write-Output "Now run Inno Setup (Build/Compile)"
Write-Output "Then run deploy-windows-setup.ps1"
Write-Output "***************************************"