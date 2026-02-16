# Playwright Installation - Anleitung

## Problem: "please ensure to build your project before running playwright"

Dieser Fehler tritt auf, wenn Playwright installiert werden soll, aber das Projekt noch nicht gebaut wurde.

---

## Lösung: 3-Schritt-Prozess

### Schritt 1: PowerShell Core installieren
Playwright benötigt PowerShell Core (pwsh):

```powershell
winget install Microsoft.PowerShell
```

Nach der Installation: **PowerShell NEU STARTEN!**

### Schritt 2: Projekt bauen
```powershell
cd "C:\Pfad\Zu\Tagesplan"
dotnet build
```

Das erzeugt die notwendige `playwright.ps1` Datei in:
- `bin\Debug\net8.0-windows\playwright.ps1`

### Schritt 3: Playwright installieren
```powershell
pwsh .\bin\Debug\net8.0-windows\playwright.ps1 install chromium
```

---

## Automatische Lösung: Fix-Skript verwenden

Das mitgelieferte Skript erledigt alles automatisch:

```powershell
.\fix-playwright.ps1
```

Das Skript:
1. ✅ Überprüft ob Projekt gebaut ist
2. ✅ Baut Projekt falls nötig
3. ✅ Installiert PowerShell Core falls nötig
4. ✅ Installiert Playwright-Browser

---

## Warum ist die Reihenfolge wichtig?

1. **Playwright ist ein NuGet-Paket**
   - Wird über `dotnet restore` heruntergeladen
   - Enthält das `playwright.ps1` Installationsskript

2. **Build-Prozess kopiert Dateien**
   - `dotnet build` kopiert alle Dependencies nach `bin\`
   - Erst danach existiert `playwright.ps1`

3. **PowerShell Core wird benötigt**
   - Standard Windows PowerShell (powershell.exe) ist zu alt
   - Playwright-Skript benötigt pwsh.exe (PowerShell 7+)

---

## Überprüfung: Ist alles installiert?

### .NET 8 SDK:
```powershell
dotnet --version
# Sollte zeigen: 8.0.x
```

### PowerShell Core:
```powershell
pwsh --version
# Sollte zeigen: PowerShell 7.x oder höher
```

### Projekt gebaut:
```powershell
Test-Path ".\bin\Debug\net8.0-windows\playwright.ps1"
# Sollte zeigen: True
```

### Playwright installiert:
```powershell
Test-Path "$env:USERPROFILE\.cache\ms-playwright\chromium-*"
# Sollte zeigen: True (wenn installiert)
```

---

## Alternative: Setup-Skript verwenden

Das Setup-Skript macht alles automatisch:

```powershell
.\setup.ps1
```

Schritte im Setup:
1. Installiert .NET 8 SDK (falls nötig)
2. Stellt NuGet-Pakete wieder her
3. **Baut das Projekt** ← wichtig!
4. Installiert PowerShell Core (falls nötig)
5. Installiert Playwright-Browser

---

## Häufige Fehlerquellen

### ❌ "pwsh: command not found"
**Problem:** PowerShell Core nicht installiert  
**Lösung:** `winget install Microsoft.PowerShell` + PowerShell neu starten

### ❌ "playwright.ps1 cannot be loaded"
**Problem:** Execution Policy blockiert Skript  
**Lösung:** `Set-ExecutionPolicy RemoteSigned -Scope CurrentUser`

### ❌ "Driver not found: node.exe"
**Problem:** Playwright-Installation unvollständig  
**Lösung:** Cache löschen und neu installieren:
```powershell
Remove-Item -Recurse -Force "$env:USERPROFILE\.cache\ms-playwright"
pwsh .\bin\Debug\net8.0-windows\playwright.ps1 install chromium
```

### ❌ Playwright installiert, aber App startet nicht
**Problem:** Falscher Browser-Channel  
**Überprüfen:** Ist Microsoft Edge installiert?
```powershell
winget install Microsoft.Edge
```

---

## In Visual Studio

Wenn du Visual Studio verwendest:

1. **Projekt bauen:** Drücke `F6` oder `Strg+Shift+B`
2. **Package Manager Console öffnen:** Ansicht → Andere Fenster → Paket-Manager-Konsole
3. **Playwright installieren:**
   ```powershell
   & "$PSScriptRoot\bin\Debug\net8.0-windows\playwright.ps1" install chromium
   ```

---

## Automatische Installation in der App

Die Anwendung versucht automatisch, Playwright beim ersten Klick auf "1. MEWS Login" zu installieren:

1. Klick auf "1. MEWS Login"
2. App prüft ob Playwright installiert ist
3. Falls nicht: Startet automatische Installation (2-3 Min)
4. Status wird im Anwendungsfenster angezeigt
5. Nach Installation öffnet sich der Browser

**Voraussetzungen für automatische Installation:**
- ✅ Projekt ist gebaut (App läuft bereits = Projekt ist gebaut!)
- ✅ PowerShell Core ist installiert
- ✅ Internetverbindung vorhanden

---

## Manuelle Installation (ohne Skripte)

Falls alle Automatismen fehlschlagen:

1. **PowerShell Core manuell herunterladen:**
   - https://github.com/PowerShell/PowerShell/releases
   - Installiere `PowerShell-7.x.x-win-x64.msi`

2. **Projekt manuell bauen:**
   - Öffne `Tagesplan.sln` in Visual Studio
   - Build → Build Solution (F6)

3. **Playwright manuell installieren:**
   - Öffne PowerShell Core (nicht Windows PowerShell!)
   - Navigiere zum Projektordner
   - Führe aus:
     ```powershell
     & ".\bin\Debug\net8.0-windows\playwright.ps1" install chromium
     ```

---

## Support

Bei Problemen:
- Siehe `TROUBLESHOOTING.md` für weitere Lösungen
- Verwende `fix-playwright.ps1` für automatische Reparatur
- Debug-Output in Visual Studio ansehen (Ansicht → Ausgabe)
