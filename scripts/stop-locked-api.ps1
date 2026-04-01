# Stops processes that lock bin\Debug outputs (Windows). Run from your repo in PowerShell (same user as the app).
$ErrorActionPreference = "SilentlyContinue"

# 1) Native host (often digiEasy-backend.exe from older builds or double-click)
taskkill /F /IM digiEasy-backend.exe /T 2>$null | Out-Null
Get-Process -Name "digiEasy-backend" -ErrorAction SilentlyContinue | Stop-Process -Force

# 2) dotnet ... digiEasy-backend.dll (typical `dotnet run` with UseAppHost=false)
Get-CimInstance Win32_Process -Filter "Name = 'dotnet.exe'" -ErrorAction SilentlyContinue | ForEach-Object {
    if ($_.CommandLine -and $_.CommandLine -match "digiEasy-backend\.dll") {
        Stop-Process -Id $_.ProcessId -Force -ErrorAction SilentlyContinue
    }
}

Write-Host "Lock holders for digiEasy-backend should be stopped. If build still fails, use Task Manager (End task) on digiEasy-backend.exe or dotnet.exe for this project." -ForegroundColor Cyan
