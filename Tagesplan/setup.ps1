# MEWS Tagesplan Generator - Automatisches Setup-Skript
# Führt die Installation auf einem neuen Rechner durch

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  MEWS Tagesplan Generator - Setup" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Überprüfe Administrator-Rechte
$isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
if (-not $isAdmin) {
    Write-Host "WARNUNG: Dieses Skript sollte als Administrator ausgeführt werden!" -ForegroundColor Yellow
    Write-Host "Einige Installationen könnten fehlschlagen." -ForegroundColor Yellow
    Write-Host ""
    $continue = Read-Host "Trotzdem fortfahren? (j/n)"
    if ($continue -ne "j") {
        exit
    }
}

# Schritt 1: .NET 8 SDK überprüfen/installieren
Write-Host "[1/4] Überprüfe .NET 8 SDK..." -ForegroundColor Green
$dotnetVersion = & dotnet --version 2>$null
if ($LASTEXITCODE -eq 0 -and $dotnetVersion -like "8.*") {
    Write-Host "  ✓ .NET 8 SDK ist bereits installiert (Version: $dotnetVersion)" -ForegroundColor Green
} else {
    Write-Host "  → .NET 8 SDK wird installiert..." -ForegroundColor Yellow
    try {
        winget install Microsoft.DotNet.SDK.8 --accept-source-agreements --accept-package-agreements --silent
        Write-Host "  ✓ .NET 8 SDK erfolgreich installiert" -ForegroundColor Green
        
        # PowerShell neu laden für PATH-Updates
        $env:Path = [System.Environment]::GetEnvironmentVariable("Path","Machine") + ";" + [System.Environment]::GetEnvironmentVariable("Path","User")
        
        # Version erneut prüfen
        $dotnetVersion = & dotnet --version 2>$null
        Write-Host "  ✓ Installierte Version: $dotnetVersion" -ForegroundColor Green
    } catch {
        Write-Host "  ✗ Fehler bei der Installation von .NET 8 SDK" -ForegroundColor Red
        Write-Host "  Bitte manuell installieren: https://dotnet.microsoft.com/download/dotnet/8.0" -ForegroundColor Yellow
        exit 1
    }
}
Write-Host ""

# Schritt 2: NuGet-Pakete wiederherstellen
Write-Host "[2/4] Stelle NuGet-Pakete wieder her..." -ForegroundColor Green
try {
    dotnet restore
    if ($LASTEXITCODE -eq 0) {
        Write-Host "  ✓ NuGet-Pakete erfolgreich wiederhergestellt" -ForegroundColor Green
    } else {
        Write-Host "  ✗ Fehler beim Wiederherstellen der NuGet-Pakete" -ForegroundColor Red
        exit 1
    }
} catch {
    Write-Host "  ✗ Fehler beim Wiederherstellen der NuGet-Pakete" -ForegroundColor Red
    Write-Host "  Fehler: $_" -ForegroundColor Red
    exit 1
}
Write-Host ""

# Schritt 3: Projekt bauen
Write-Host "[3/4] Baue Projekt..." -ForegroundColor Green
try {
    dotnet build --configuration Release
    if ($LASTEXITCODE -eq 0) {
        Write-Host "  ✓ Projekt erfolgreich gebaut" -ForegroundColor Green
    } else {
        Write-Host "  ✗ Fehler beim Bauen des Projekts" -ForegroundColor Red
        exit 1
    }
} catch {
    Write-Host "  ✗ Fehler beim Bauen des Projekts" -ForegroundColor Red
    Write-Host "  Fehler: $_" -ForegroundColor Red
    exit 1
}
Write-Host ""

# Schritt 4: Playwright installieren
Write-Host "[4/4] Installiere Playwright-Browser..." -ForegroundColor Green

# Prüfe ob pwsh verfügbar ist
$pwshAvailable = Get-Command pwsh -ErrorAction SilentlyContinue
if (-not $pwshAvailable) {
    Write-Host "  ! PowerShell Core (pwsh) nicht gefunden" -ForegroundColor Yellow
    Write-Host "  Playwright wird beim ersten MEWS-Login automatisch installiert." -ForegroundColor Yellow
} else {
    $playwrightPath = ".\bin\Release\net8.0-windows\playwright.ps1"
    if (Test-Path $playwrightPath) {
        try {
            Write-Host "  → Installiere Chromium-Browser..." -ForegroundColor Yellow
            $installProcess = Start-Process -FilePath "pwsh" -ArgumentList "-File `"$playwrightPath`" install chromium" -NoNewWindow -Wait -PassThru

            if ($installProcess.ExitCode -eq 0) {
                Write-Host "  ✓ Playwright-Browser erfolgreich installiert" -ForegroundColor Green
            } else {
                Write-Host "  ! Playwright-Installation mit Warnung beendet (Exit Code: $($installProcess.ExitCode))" -ForegroundColor Yellow
                Write-Host "  Beim ersten MEWS-Login wird automatisch nachinstalliert." -ForegroundColor Yellow
            }
        } catch {
            Write-Host "  ! Playwright konnte nicht installiert werden: $_" -ForegroundColor Yellow
            Write-Host "  Beim ersten MEWS-Login wird automatisch nachinstalliert." -ForegroundColor Yellow
        }
    } else {
        Write-Host "  ! Playwright-Skript nicht gefunden: $playwrightPath" -ForegroundColor Yellow
        Write-Host "  Beim ersten MEWS-Login wird automatisch installiert." -ForegroundColor Yellow
    }
}
Write-Host ""

# Erfolgsmeldung
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  ✓ Setup erfolgreich abgeschlossen!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Anwendung starten:" -ForegroundColor Yellow
Write-Host "  1. Mit dotnet:   dotnet run" -ForegroundColor White
Write-Host "  2. Direkt:       .\bin\Release\net8.0-windows\Tagesplan.exe" -ForegroundColor White
Write-Host ""
Write-Host "Dokumentation:" -ForegroundColor Yellow
Write-Host "  - README.md             - Funktionsübersicht" -ForegroundColor White
Write-Host "  - INSTALLATION.md       - Detaillierte Installation" -ForegroundColor White
Write-Host "  - MEWS_EXPORT_GUIDE.md  - MEWS Export-Anleitung" -ForegroundColor White
Write-Host "  - DEBUG_GUIDE.md        - Fehlerbehebung" -ForegroundColor White
Write-Host ""

# Optional: Anwendung starten
$startApp = Read-Host "Möchten Sie die Anwendung jetzt starten? (j/n)"
if ($startApp -eq "j") {
    Write-Host ""
    Write-Host "Starte Anwendung..." -ForegroundColor Green
    Start-Process ".\bin\Release\net8.0-windows\Tagesplan.exe"
}
