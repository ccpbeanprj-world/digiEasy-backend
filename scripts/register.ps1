param(
    [string] $BaseUrl = "http://localhost:5000"
)

$repoRoot = Split-Path $PSScriptRoot -Parent
$lastEmailPath = Join-Path $repoRoot ".last-register-email"

# New email every run — avoids "Email has already been registered" while testing.
$email = "user.{0}@example.com" -f ([Guid]::NewGuid().ToString("N"))
$body = @"
{
  "firstName": "John",
  "lastName": "Doe",
  "email": "$email",
  "password": "Abcd1234",
  "passwordConfirmation": "Abcd1234",
  "gender": "M",
  "birthday": "2000-01-15",
  "mobilePhoneNumber": "12345678"
}
"@

$uri = ($BaseUrl.TrimEnd("/") + "/api/users/register")
try {
    $result = Invoke-RestMethod -Method Post -Uri $uri -ContentType "application/json; charset=utf-8" -Body $body
    Set-Content -Path $lastEmailPath -Value $email -Encoding utf8 -NoNewline
    Write-Host "Email used: $email (saved to .last-register-email for update-password.ps1)" -ForegroundColor Green
    $result
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
