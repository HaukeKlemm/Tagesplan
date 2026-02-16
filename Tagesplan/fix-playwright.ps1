# Fix für Playwright-Installation
# Verwende dieses Skript, wenn beim MEWS-Login ein "Driver not found" Fehler auftritt

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Playwright-Browser Fix" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Überprüfe Administrator-Rechte
$isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
if (-not $isAdmin) {
    Write-Host "HINWEIS: Für Installation von PowerShell Core werden Admin-Rechte empfohlen." -ForegroundColor Yellow
    Write-Host "Skript kann trotzdem fortfahren..." -ForegroundColor Yellow
    Write-Host ""
}

# Schritt 0: Projekt bauen (falls noch nicht geschehen)
Write-Host "[0/3] Überprüfe Projekt-Build..." -ForegroundColor Green
$possiblePaths = @(
    ".\bin\Debug\net8.0-windows\playwright.ps1",
    ".\bin\Release\net8.0-windows\playwright.ps1"
)

$playwrightScript = $null
foreach ($path in $possiblePaths) {
    if (Test-Path $path) {
        $playwrightScript = $path
        break
    }
}

if (-not $playwrightScript) {
    Write-Host "  ! Playwright-Skript nicht gefunden" -ForegroundColor Yellow
    Write-Host "  → Projekt wird gebaut..." -ForegroundColor Yellow
    Write-Host ""

    try {
        dotnet build
        if ($LASTEXITCODE -eq 0) {
            Write-Host "  ✓ Projekt erfolgreich gebaut" -ForegroundColor Green

            # Erneut prüfen
            foreach ($path in $possiblePaths) {
                if (Test-Path $path) {
                    $playwrightScript = $path
                    break
                }
            }
        } else {
            Write-Host "  ✗ Fehler beim Bauen des Projekts" -ForegroundColor Red
            exit 1
        }
    } catch {
        Write-Host "  ✗ Fehler beim Bauen: $_" -ForegroundColor Red
        exit 1
    }
} else {
    Write-Host "  ✓ Projekt ist gebaut" -ForegroundColor Green
}

if (-not $playwrightScript) {
    Write-Host ""
    Write-Host "✗ Fehler: playwright.ps1 konnte nicht gefunden werden!" -ForegroundColor Red
    exit 1
}

Write-Host "  Gefundenes Skript: $playwrightScript" -ForegroundColor Gray
Write-Host ""

# Überprüfe PowerShell Core
Write-Host "[1/2] Überprüfe PowerShell Core (pwsh)..." -ForegroundColor Green
$pwsh = Get-Command pwsh -ErrorAction SilentlyContinue
if (-not $pwsh) {
    Write-Host "  ! PowerShell Core nicht installiert" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Installiere PowerShell Core..." -ForegroundColor Yellow
    try {
        winget install Microsoft.PowerShell --accept-source-agreements --accept-package-agreements --silent
        Write-Host "  ✓ PowerShell Core installiert" -ForegroundColor Green
        
        # Aktualisiere PATH
        $env:Path = [System.Environment]::GetEnvironmentVariable("Path","Machine") + ";" + [System.Environment]::GetEnvironmentVariable("Path","User")
    } catch {
        Write-Host "  ✗ Installation fehlgeschlage: $_" -ForegroundColor Red
        Write-Host ""
        Write-Host "Bitte manuell installieren:" -ForegroundColor Yellow
        Write-Host "  https://github.com/PowerShell/PowerShell/releases" -ForegroundColor White
        exit 1
    }
} else {
    Write-Host "  ✓ PowerShell Core ist installiert" -ForegroundColor Green
}
Write-Host ""

# Installiere Playwright-Browser
Write-Host "[2/2] Installiere Playwright-Browser..." -ForegroundColor Green
Write-Host "  Dies kann 2-3 Minuten dauern..." -ForegroundColor Yellow
Write-Host ""

try {
    $process = Start-Process -FilePath "pwsh" `
        -ArgumentList "-ExecutionPolicy Bypass -File `"$playwrightScript`" install chromium" `
        -NoNewWindow `
        -Wait `
        -PassThru

    if ($process.ExitCode -eq 0) {
        Write-Host ""
        Write-Host "========================================" -ForegroundColor Cyan
        Write-Host "  ✓ Playwright erfolgreich installiert!" -ForegroundColor Green
        Write-Host "========================================" -ForegroundColor Cyan
        Write-Host ""
        Write-Host "Sie können jetzt die Anwendung starten und den MEWS-Login verwenden." -ForegroundColor Green
    } else {
        Write-Host ""
        Write-Host "✗ Installation fehlgeschlagen (Exit Code: $($process.ExitCode))" -ForegroundColor Red
        Write-Host ""
        Write-Host "Versuchen Sie manuell:" -ForegroundColor Yellow
        Write-Host "  pwsh -ExecutionPolicy Bypass -File `"$playwrightScript`" install chromium" -ForegroundColor White
        exit 1
    }
} catch {
    Write-Host ""
    Write-Host "✗ Fehler: $_" -ForegroundColor Red
    Write-Host ""
    Write-Host "Versuchen Sie manuell:" -ForegroundColor Yellow
    Write-Host "  pwsh $playwrightScript install chromium" -ForegroundColor White
    exit 1
}

Write-Host ""
Write-Host "Tipp: Die Anwendung installiert Playwright auch automatisch beim ersten MEWS-Login." -ForegroundColor Cyan
