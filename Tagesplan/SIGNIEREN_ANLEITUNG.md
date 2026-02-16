# PowerShell-Skripte digital signieren

## Welche Dateien müssen signiert werden?

Diese 3 PowerShell-Skripte:
1. **setup.ps1** (im Projektroot)
2. **fix-playwright.ps1** (im Projektroot)
3. **playwright.ps1** (in bin\Debug\net8.0-windows\ - nach Build)

---

## Schnelle Lösung: Self-Signed Certificate erstellen und signieren

### Schritt 1: Certificate erstellen (einmalig)

Öffnen Sie PowerShell **als Administrator** und führen Sie aus:

```powershell
# Self-Signed Certificate erstellen
$cert = New-SelfSignedCertificate -Type CodeSigningCert -Subject "CN=Tagesplan Code Signing" -CertStoreLocation Cert:\CurrentUser\My

# Certificate in Trusted Root und Trusted Publishers kopieren
$store = New-Object System.Security.Cryptography.X509Certificates.X509Store("Root","CurrentUser")
$store.Open("ReadWrite")
$store.Add($cert)
$store.Close()

$store = New-Object System.Security.Cryptography.X509Certificates.X509Store("TrustedPublisher","CurrentUser")
$store.Open("ReadWrite")
$store.Add($cert)
$store.Close()

Write-Host "✓ Certificate erstellt!" -ForegroundColor Green
Write-Host "Thumbprint: $($cert.Thumbprint)" -ForegroundColor Yellow
```

### Schritt 2: Dateien signieren

```powershell
# Certificate laden (verwenden Sie den Thumbprint von oben)
$cert = Get-ChildItem Cert:\CurrentUser\My -CodeSigningCert | Select-Object -First 1

# Zum Projektordner navigieren
cd "C:\Pfad\Zu\Tagesplan"

# Alle .ps1 Dateien signieren
Set-AuthenticodeSignature -FilePath ".\setup.ps1" -Certificate $cert
Set-AuthenticodeSignature -FilePath ".\fix-playwright.ps1" -Certificate $cert

# Wenn das Projekt gebaut ist:
Set-AuthenticodeSignature -FilePath ".\bin\Debug\net8.0-windows\playwright.ps1" -Certificate $cert

Write-Host "✓ Alle Dateien signiert!" -ForegroundColor Green
```

---

## Automatisches Signatur-Skript

Ich erstelle Ihnen ein Skript, das alles automatisch macht:

**sign-scripts.ps1** (siehe unten)

Dann einfach ausführen als Administrator:
```powershell
.\sign-scripts.ps1
```

---

## Alternative: Execution Policy dauerhaft ändern (einfacher!)

Statt zu signieren, können Sie die Policy ändern:

```powershell
# Als Administrator ausführen:
Set-ExecutionPolicy RemoteSigned -Scope CurrentUser

# Oder für alle Benutzer:
Set-ExecutionPolicy RemoteSigned -Scope LocalMachine
```

Dann funktionieren alle Skripte ohne Signatur!

---

## Was ist besser?

| Methode | Vorteil | Nachteil |
|---------|---------|----------|
| **Self-Signed Cert** | Maximale Kontrolle | Einmalig etwas Aufwand |
| **RemoteSigned Policy** | Einfach & schnell | Weniger sicher für fremde Skripte |
| **-ExecutionPolicy Bypass** | Funktioniert sofort | Muss bei jedem Befehl angegeben werden |

**Meine Empfehlung:** `Set-ExecutionPolicy RemoteSigned -Scope CurrentUser`

---

## Überprüfung

**Prüfen ob signiert:**
```powershell
Get-AuthenticodeSignature .\setup.ps1
```

**Sollte zeigen:**
```
Status      : Valid
SignerCertificate : CN=Tagesplan Code Signing
```

**Prüfen welche Policy aktiv ist:**
```powershell
Get-ExecutionPolicy -List
```

---

## Bei Problemen

Wenn Signieren zu kompliziert ist:

**Option 1 - Policy ändern (empfohlen):**
```powershell
Set-ExecutionPolicy RemoteSigned -Scope CurrentUser
```

**Option 2 - Immer Bypass verwenden:**
```powershell
powershell -ExecutionPolicy Bypass -File .\setup.ps1
powershell -ExecutionPolicy Bypass -File .\fix-playwright.ps1
```

**Option 3 - Skript-Inhalt direkt ausführen:**
Öffnen Sie setup.ps1 in Editor, kopieren Sie den Inhalt und fügen Sie ihn direkt in PowerShell ein.
