# Automatisches Signatur-Skript für Tagesplan
# Erstellt ein Self-Signed Certificate und signiert alle PowerShell-Skripte

# Prüfe Administrator-Rechte
$isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
if (-not $isAdmin) {
    Write-Host "✗ Dieses Skript muss als Administrator ausgeführt werden!" -ForegroundColor Red
    Write-Host ""
    Write-Host "Rechtsklick auf PowerShell → 'Als Administrator ausführen'" -ForegroundColor Yellow
    pause
    exit 1
}

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  PowerShell-Skripte signieren" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Schritt 1: Prüfe ob Certificate bereits existiert
Write-Host "[1/3] Prüfe Certificate..." -ForegroundColor Green
$existingCert = Get-ChildItem Cert:\CurrentUser\My -CodeSigningCert | Where-Object { $_.Subject -like "*Tagesplan*" } | Select-Object -First 1

if ($existingCert) {
    Write-Host "  ✓ Certificate bereits vorhanden" -ForegroundColor Green
    Write-Host "  Thumbprint: $($existingCert.Thumbprint)" -ForegroundColor Gray
    $cert = $existingCert
} else {
    Write-Host "  → Erstelle neues Self-Signed Certificate..." -ForegroundColor Yellow
    
    try {
        # Certificate erstellen
        $cert = New-SelfSignedCertificate `
            -Type CodeSigningCert `
            -Subject "CN=Tagesplan Code Signing" `
            -KeyAlgorithm RSA `
            -KeyLength 2048 `
            -NotAfter (Get-Date).AddYears(5) `
            -CertStoreLocation Cert:\CurrentUser\My
        
        Write-Host "  ✓ Certificate erstellt" -ForegroundColor Green
        Write-Host "  Thumbprint: $($cert.Thumbprint)" -ForegroundColor Gray
        
        # Certificate in Trusted Root importieren
        Write-Host "  → Importiere in Trusted Root..." -ForegroundColor Yellow
        $store = New-Object System.Security.Cryptography.X509Certificates.X509Store("Root","CurrentUser")
        $store.Open("ReadWrite")
        $store.Add($cert)
        $store.Close()
        
        # Certificate in Trusted Publishers importieren
        Write-Host "  → Importiere in Trusted Publishers..." -ForegroundColor Yellow
        $store = New-Object System.Security.Cryptography.X509Certificates.X509Store("TrustedPublisher","CurrentUser")
        $store.Open("ReadWrite")
        $store.Add($cert)
        $store.Close()
        
        Write-Host "  ✓ Certificate installiert" -ForegroundColor Green
    } catch {
        Write-Host "  ✗ Fehler beim Erstellen des Certificates: $_" -ForegroundColor Red
        exit 1
    }
}
Write-Host ""

# Schritt 2: Finde alle zu signierenden Dateien
Write-Host "[2/3] Finde PowerShell-Skripte..." -ForegroundColor Green

$filesToSign = @()

# Im Projektroot
$rootScripts = @("setup.ps1", "fix-playwright.ps1")
foreach ($script in $rootScripts) {
    if (Test-Path $script) {
        $filesToSign += $script
        Write-Host "  ✓ Gefunden: $script" -ForegroundColor Green
    } else {
        Write-Host "  ! Nicht gefunden: $script" -ForegroundColor Yellow
    }
}

# In bin\Debug\net8.0-windows\
$debugPlaywright = "bin\Debug\net8.0-windows\playwright.ps1"
if (Test-Path $debugPlaywright) {
    $filesToSign += $debugPlaywright
    Write-Host "  ✓ Gefunden: $debugPlaywright" -ForegroundColor Green
} else {
    Write-Host "  ! Nicht gefunden: $debugPlaywright (Projekt noch nicht gebaut?)" -ForegroundColor Yellow
}

# In bin\Release\net8.0-windows\
$releasePlaywright = "bin\Release\net8.0-windows\playwright.ps1"
if (Test-Path $releasePlaywright) {
    $filesToSign += $releasePlaywright
    Write-Host "  ✓ Gefunden: $releasePlaywright" -ForegroundColor Green
}

if ($filesToSign.Count -eq 0) {
    Write-Host ""
    Write-Host "✗ Keine Dateien zum Signieren gefunden!" -ForegroundColor Red
    Write-Host "  Stellen Sie sicher, dass Sie im Projektroot-Ordner sind." -ForegroundColor Yellow
    exit 1
}

Write-Host ""
Write-Host "  Gefunden: $($filesToSign.Count) Datei(en)" -ForegroundColor Cyan
Write-Host ""

# Schritt 3: Signiere alle Dateien
Write-Host "[3/3] Signiere Dateien..." -ForegroundColor Green

$successCount = 0
$failCount = 0

foreach ($file in $filesToSign) {
    try {
        $signature = Set-AuthenticodeSignature -FilePath $file -Certificate $cert -ErrorAction Stop
        
        if ($signature.Status -eq "Valid") {
            Write-Host "  ✓ Signiert: $file" -ForegroundColor Green
            $successCount++
        } else {
            Write-Host "  ✗ Fehler bei: $file (Status: $($signature.Status))" -ForegroundColor Red
            $failCount++
        }
    } catch {
        Write-Host "  ✗ Fehler bei: $file - $_" -ForegroundColor Red
        $failCount++
    }
}

Write-Host ""

# Zusammenfassung
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Ergebnis" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "  Erfolgreich signiert: $successCount" -ForegroundColor Green
if ($failCount -gt 0) {
    Write-Host "  Fehlgeschlagen: $failCount" -ForegroundColor Red
}
Write-Host ""

if ($successCount -eq $filesToSign.Count) {
    Write-Host "✓ Alle Dateien erfolgreich signiert!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Sie können jetzt die Skripte ohne -ExecutionPolicy Bypass ausführen." -ForegroundColor Cyan
} else {
    Write-Host "! Einige Dateien konnten nicht signiert werden." -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Alternative: Execution Policy ändern:" -ForegroundColor Yellow
    Write-Host "  Set-ExecutionPolicy RemoteSigned -Scope CurrentUser" -ForegroundColor White
}

Write-Host ""
Write-Host "Überprüfung einer Signatur:" -ForegroundColor Cyan
Write-Host "  Get-AuthenticodeSignature .\setup.ps1" -ForegroundColor White
Write-Host ""
