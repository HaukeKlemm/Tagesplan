# Schnelle Lösung für "missing required assets"
# Speziell für: C:\Users\offic\Desktop\Tagesplan\Tagesplan-main\Tagesplan

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Quick Fix - Rebuild & Install" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$projectPath = "C:\Users\offic\Desktop\Tagesplan\Tagesplan-main\Tagesplan"

# Navigiere zum Projekt
Write-Host "Navigiere zu: $projectPath" -ForegroundColor Yellow
if (-not (Test-Path $projectPath)) {
    Write-Host "✗ Pfad nicht gefunden!" -ForegroundColor Red
    Write-Host ""
    Write-Host "Bitte passen Sie den Pfad in diesem Skript an!" -ForegroundColor Yellow
    pause
    exit 1
}

Set-Location $projectPath
Write-Host "✓ Projekt gefunden" -ForegroundColor Green
Write-Host ""

# 1. Clean
Write-Host "[1/4] Clean..." -ForegroundColor Green
dotnet clean --configuration Debug
Write-Host ""

# 2. Restore
Write-Host "[2/4] Restore..." -ForegroundColor Green
dotnet restore
if ($LASTEXITCODE -ne 0) {
    Write-Host "✗ Restore fehlgeschlagen!" -ForegroundColor Red
    pause
    exit 1
}
Write-Host ""

# 3. Build
Write-Host "[3/4] Build..." -ForegroundColor Green
dotnet build --configuration Debug
if ($LASTEXITCODE -ne 0) {
    Write-Host "✗ Build fehlgeschlagen!" -ForegroundColor Red
    pause
    exit 1
}
Write-Host "✓ Build erfolgreich" -ForegroundColor Green
Write-Host ""

# 4. Playwright installieren
Write-Host "[4/4] Playwright installieren..." -ForegroundColor Green

$playwrightScript = "bin\Debug\net8.0-windows\playwright.ps1"

if (-not (Test-Path $playwrightScript)) {
    Write-Host "✗ playwright.ps1 nicht gefunden!" -ForegroundColor Red
    Write-Host "Erwartet: $playwrightScript" -ForegroundColor Yellow
    pause
    exit 1
}

# Prüfe PowerShell Core
$pwsh = Get-Command pwsh -ErrorAction SilentlyContinue
if (-not $pwsh) {
    Write-Host "→ Installiere PowerShell Core..." -ForegroundColor Yellow
    winget install Microsoft.PowerShell --silent
    $env:Path = [System.Environment]::GetEnvironmentVariable("Path","Machine") + ";" + [System.Environment]::GetEnvironmentVariable("Path","User")
}

Write-Host "→ Installiere Chromium (2-3 Minuten)..." -ForegroundColor Yellow
$process = Start-Process -FilePath "pwsh" `
    -ArgumentList "-ExecutionPolicy Bypass -File `"$playwrightScript`" install chromium" `
    -NoNewWindow `
    -Wait `
    -PassThru

if ($process.ExitCode -eq 0) {
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "  ✓ FERTIG!" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Anwendung starten:" -ForegroundColor Cyan
    Write-Host "  .\bin\Debug\net8.0-windows\Tagesplan.exe" -ForegroundColor White
    Write-Host ""
    
    $start = Read-Host "Anwendung jetzt starten? (j/n)"
    if ($start -eq "j") {
        Start-Process ".\bin\Debug\net8.0-windows\Tagesplan.exe"
    }
} else {
    Write-Host ""
    Write-Host "✗ Playwright-Installation fehlgeschlagen!" -ForegroundColor Red
    Write-Host ""
    Write-Host "Versuchen Sie manuell:" -ForegroundColor Yellow
    Write-Host "  pwsh -ExecutionPolicy Bypass -File `"$playwrightScript`" install chromium" -ForegroundColor White
    pause
}
