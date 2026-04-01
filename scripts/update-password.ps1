param(
    [string] $BaseUrl = "http://localhost:5000",
    [string] $Email = "",
    [string] $CurrentPassword = "Abcd1234",
    [string] $NewPassword = "Newpass123"
)

$repoRoot = Split-Path $PSScriptRoot -Parent
$lastEmailPath = Join-Path $repoRoot ".last-register-email"

if ([string]::IsNullOrWhiteSpace($Email)) {
    if (Test-Path $lastEmailPath) {
        $Email = (Get-Content -Raw -Encoding utf8 $lastEmailPath).Trim()
    }
}

if ([string]::IsNullOrWhiteSpace($Email)) {
    Write-Host "No -Email and no .last-register-email file." -ForegroundColor Yellow
    Write-Host "Run .\scripts\register.ps1 first, or pass the account explicitly, e.g.:" -ForegroundColor Yellow
    Write-Host "  .\scripts\update-password.ps1 -Email 'john.doe@example.com' -CurrentPassword 'Abcd1234'" -ForegroundColor Yellow
    Write-Host "(Defaults assumed john.doe only if you registered with examples\register.json.)" -ForegroundColor Yellow
    exit 1
}

$body = @"
{
  "email": "$Email",
  "currentPassword": "$CurrentPassword",
  "newPassword": "$NewPassword",
  "newPasswordConfirmation": "$NewPassword"
}
"@

$uri = ($BaseUrl.TrimEnd("/") + "/api/users/password")
try {
    Write-Host "Updating password for: $Email" -ForegroundColor Cyan
    Invoke-RestMethod -Method Patch -Uri $uri -ContentType "application/json; charset=utf-8" -Body $body
} catch {
    $detail = $null
    if ($_.ErrorDetails -and $_.ErrorDetails.Message) {
        $detail = $_.ErrorDetails.Message
    }
    if (-not $detail -and $_.Exception.Response) {
        try {
            $stream = $_.Exception.Response.GetResponseStream()
            $reader = New-Object System.IO.StreamReader($stream)
            $detail = $reader.ReadToEnd()
        } catch { }
    }
    if (-not $detail) { $detail = $_.Exception.Message }
    Write-Host "API error: $detail" -ForegroundColor Red
    exit 1
}
