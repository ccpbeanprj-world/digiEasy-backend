# Stops a previous instance, then runs the API.
#   .\scripts\run.ps1                    # build + run (slow if MSBuild runs every time)
#   .\scripts\run.ps1 -NoBuild           # skip compile — fast if bin\Debug DLL is up to date
#   .\scripts\run.ps1 --urls "http://localhost:5080"
param(
    [switch] $NoBuild,
    [Parameter(ValueFromRemainingArguments = $true)]
    [string[]] $PassArgs
)

$repoRoot = Split-Path $PSScriptRoot -Parent
$csproj = Join-Path $repoRoot "digiEasy-backend.csproj"
$dll = Join-Path $repoRoot "bin\Debug\net8.0\digiEasy-backend.dll"

& "$PSScriptRoot\stop-locked-api.ps1"
Start-Sleep -Milliseconds 400

Set-Location $repoRoot

if ($NoBuild) {
    if (-not (Test-Path $dll)) {
        Write-Host "No DLL at $dll — run `dotnet build` once, or run without -NoBuild." -ForegroundColor Yellow
        exit 1
    }
    if ($PassArgs.Count -gt 0) {
        dotnet exec $dll @PassArgs
    } else {
        dotnet exec $dll
    }
} elseif ($PassArgs.Count -gt 0) {
    dotnet run --project $csproj @PassArgs
} else {
    dotnet run --project $csproj
}
