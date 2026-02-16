# Rebuild und Playwright Installation
# Löst das "missing required assets" Problem

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Rebuild + Playwright Installation" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Prüfe ob wir im richtigen Ordner sind
$projectFile = Get-ChildItem -Filter "*.csproj" | Select-Object -First 1
$solutionFile = Get-ChildItem -Filter "*.sln" | Select-Object -First 1

if (-not $projectFile -and -not $solutionFile) {
    Write-Host "✗ Fehler: Weder .csproj noch .sln gefunden!" -ForegroundColor Red
    Write-Host ""
    Write-Host "Bitte navigieren Sie zu einem dieser Ordner:" -ForegroundColor Yellow
    Write-Host "  • Tagesplan\Tagesplan-main\ (wenn .sln dort ist)" -ForegroundColor White
    Write-Host "  • Tagesplan\Tagesplan-main\Tagesplan\ (wenn nur .csproj dort ist)" -ForegroundColor White
    Write-Host ""
    Write-Host "Aktueller Pfad: $PWD" -ForegroundColor Gray
    Write-Host ""
    Write-Host "Suche in Unterordnern..." -ForegroundColor Yellow

    # Versuche .sln oder .csproj in Unterordnern zu finden
    $foundProjects = Get-ChildItem -Recurse -Filter "*.csproj" -ErrorAction SilentlyContinue | Select-Object -First 5
    $foundSolutions = Get-ChildItem -Recurse -Filter "*.sln" -ErrorAction SilentlyContinue | Select-Object -First 5

    if ($foundSolutions) {
        Write-Host ""
        Write-Host "Gefundene .sln Dateien:" -ForegroundColor Cyan
        foreach ($sln in $foundSolutions) {
            Write-Host "  • $($sln.DirectoryName)" -ForegroundColor White
        }
    }

    if ($foundProjects) {
        Write-Host ""
        Write-Host "Gefundene .csproj Dateien:" -ForegroundColor Cyan
        foreach ($proj in $foundProjects) {
            Write-Host "  • $($proj.DirectoryName)" -ForegroundColor White
        }
    }

    Write-Host ""
    pause
    exit 1
}

if ($solutionFile) {
    Write-Host "✓ Solution gefunden: $($solutionFile.Name)" -ForegroundColor Green
    $buildTarget = $solutionFile.FullName
} else {
    Write-Host "✓ Projekt gefunden: $($projectFile.Name)" -ForegroundColor Green
    $buildTarget = $projectFile.FullName
}
Write-Host ""

# Schritt 1: Clean
Write-Host "[1/4] Bereinige alte Build-Dateien..." -ForegroundColor Green
try {
    dotnet clean "$buildTarget" --configuration Debug
    if ($LASTEXITCODE -eq 0) {
        Write-Host "  ✓ Bereinigung erfolgreich" -ForegroundColor Green
    } else {
        Write-Host "  ! Warnung bei Bereinigung (Code: $LASTEXITCODE)" -ForegroundColor Yellow
    }
} catch {
    Write-Host "  ! Fehler bei Bereinigung: $_" -ForegroundColor Yellow
}
Write-Host ""

# Schritt 2: Restore
Write-Host "[2/4] Stelle NuGet-Pakete wieder her..." -ForegroundColor Green
try {
    dotnet restore "$buildTarget"
    if ($LASTEXITCODE -ne 0) {
        Write-Host "  ✗ Fehler beim Wiederherstellen der Pakete" -ForegroundColor Red
        exit 1
    }
    Write-Host "  ✓ Pakete wiederhergestellt" -ForegroundColor Green
} catch {
    Write-Host "  ✗ Fehler: $_" -ForegroundColor Red
    exit 1
}
Write-Host ""

# Schritt 3: Build
Write-Host "[3/4] Baue Projekt neu..." -ForegroundColor Green
try {
    dotnet build "$buildTarget" --configuration Debug --no-restore
    if ($LASTEXITCODE -ne 0) {
        Write-Host "  ✗ Build fehlgeschlagen" -ForegroundColor Red
        Write-Host ""
        Write-Host "Bitte überprüfen Sie die Fehlermeldungen oben." -ForegroundColor Yellow
        pause
        exit 1
    }
    Write-Host "  ✓ Build erfolgreich" -ForegroundColor Green
} catch {
    Write-Host "  ✗ Fehler: $_" -ForegroundColor Red
    exit 1
}
Write-Host ""

# Schritt 4: Playwright installieren
Write-Host "[4/4] Installiere Playwright-Browser..." -ForegroundColor Green

$playwrightScript = "bin\Debug\net8.0-windows\playwright.ps1"

if (-not (Test-Path $playwrightScript)) {
    Write-Host "  ✗ playwright.ps1 nicht gefunden: $playwrightScript" -ForegroundColor Red
    Write-Host "  Build war möglicherweise nicht erfolgreich." -ForegroundColor Yellow
    pause
    exit 1
}

# Prüfe PowerShell Core
$pwsh = Get-Command pwsh -ErrorAction SilentlyContinue
if (-not $pwsh) {
    Write-Host "  ! PowerShell Core nicht gefunden" -ForegroundColor Yellow
    Write-Host "  Installiere PowerShell Core..." -ForegroundColor Yellow
    try {
        winget install Microsoft.PowerShell --accept-source-agreements --accept-package-agreements --silent
        Write-Host "  ✓ PowerShell Core installiert" -ForegroundColor Green
        # PATH aktualisieren
        $env:Path = [System.Environment]::GetEnvironmentVariable("Path","Machine") + ";" + [System.Environment]::GetEnvironmentVariable("Path","User")
    } catch {
        Write-Host "  ✗ PowerShell Core Installation fehlgeschlagen" -ForegroundColor Red
        Write-Host "  Bitte manuell installieren: winget install Microsoft.PowerShell" -ForegroundColor Yellow
        pause
        exit 1
    }
}

Write-Host "  → Installiere Chromium (ca. 2-3 Minuten)..." -ForegroundColor Yellow
Write-Host ""

try {
    $process = Start-Process -FilePath "pwsh" `
        -ArgumentList "-ExecutionPolicy Bypass -File `"$playwrightScript`" install chromium" `
        -NoNewWindow `
        -Wait `
        -PassThru
    
    if ($process.ExitCode -eq 0) {
        Write-Host ""
        Write-Host "  ✓ Playwright erfolgreich installiert!" -ForegroundColor Green
    } else {
        Write-Host ""
        Write-Host "  ✗ Playwright-Installation fehlgeschlagen (Exit Code: $($process.ExitCode))" -ForegroundColor Red
        Write-Host ""
        Write-Host "Versuchen Sie manuell:" -ForegroundColor Yellow
        Write-Host "  pwsh -ExecutionPolicy Bypass -File `"$playwrightScript`" install chromium" -ForegroundColor White
        pause
        exit 1
    }
} catch {
    Write-Host ""
    Write-Host "  ✗ Fehler: $_" -ForegroundColor Red
    pause
    exit 1
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  ✓ Installation abgeschlossen!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Sie können jetzt die Anwendung starten:" -ForegroundColor Green
Write-Host "  • In Visual Studio: F5 drücken" -ForegroundColor White
Write-Host "  • Oder: .\bin\Debug\net8.0-windows\Tagesplan.exe" -ForegroundColor White
Write-Host ""
Write-Host "Dann auf 'MEWS Login' klicken - jetzt sollte es funktionieren!" -ForegroundColor Cyan
Write-Host ""

$start = Read-Host "Möchten Sie die Anwendung jetzt starten? (j/n)"
if ($start -eq "j") {
    Start-Process ".\bin\Debug\net8.0-windows\Tagesplan.exe"
}
