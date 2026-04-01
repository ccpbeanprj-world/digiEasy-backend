param(
    [string] $BaseUrl = "http://localhost:5000",
    [string] $Email = "",
    [string] $Password = ""
)

$repoRoot = Split-Path $PSScriptRoot -Parent
$lastEmailPath = Join-Path $repoRoot ".last-register-email"

if ([string]::IsNullOrWhiteSpace($Email)) {
    if (Test-Path $lastEmailPath) {
        $Email = (Get-Content -Raw -Encoding utf8 $lastEmailPath).Trim()
    }
}

if ([string]::IsNullOrWhiteSpace($Email) -or [string]::IsNullOrWhiteSpace($Password)) {
    Write-Host "Usage: pass -Email and -Password to test against the API." -ForegroundColor Yellow
    Write-Host "  .\scripts\verify-password.ps1 -Email 'john.doe@example.com' -Password 'Abcd1234'" -ForegroundColor Yellow
    Write-Host "Or set -Password only if .last-register-email exists (uses that email)." -ForegroundColor Yellow
    exit 1
}

$body = @"
{
  "email": "$Email",
  "password": "$Password"
}
"@

$uri = ($BaseUrl.TrimEnd("/") + "/api/users/verify-password")
try {
    $r = Invoke-RestMethod -Method Post -Uri $uri -ContentType "application/json; charset=utf-8" -Body $body
    if ($r.valid) {
        Write-Host "valid: true — this password matches the account." -ForegroundColor Green
    } else {
        Write-Host "valid: false — reason: $($r.reason)" -ForegroundColor Red
        if ($r.reason -eq "password_mismatch") {
            Write-Host "Try the password from registration (e.g. Abcd1234) or the last password you set with update-password (e.g. Newpass123)." -ForegroundColor Yellow
        }
    }
    $r
} catch {
    $detail = $null
    if ($_.ErrorDetails -and $_.ErrorDetails.Message) { $detail = $_.ErrorDetails.Message }
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
