# Setup-Zusammenfassung für neue Rechner

## ✅ Vollständige Installations-Checkliste

### Vor der Installation bereitstellen:
- [ ] ZIP-Datei des Projekts
- [ ] Internetverbindung (für Downloads)
- [ ] Windows 10 oder 11
- [ ] Administrator-Rechte (empfohlen)

---

## 🚀 Schnellste Methode: Automatisches Setup

```powershell
# 1. ZIP entpacken
# 2. PowerShell als Administrator öffnen
# 3. Navigieren & ausführen:
cd "C:\Pfad\Zu\Tagesplan"
.\setup.ps1
```

**Was passiert automatisch:**
1. ✅ .NET 8 SDK Installation
2. ✅ NuGet-Pakete Installation
3. ✅ **Projekt wird gebaut** ← wichtig!
4. ✅ PowerShell Core Installation
5. ✅ Playwright-Browser Installation

**Dauer:** Ca. 5-10 Minuten (abhängig von Internet)

---

## 📋 Detaillierte Schritte (manuell)

### Schritt 1: .NET 8 SDK
```powershell
winget install Microsoft.DotNet.SDK.8
```
Überprüfen:
```powershell
dotnet --version  # Sollte 8.0.x zeigen
```

### Schritt 2: PowerShell Core (WICHTIG!)
```powershell
winget install Microsoft.PowerShell
```
**POWERSHELL NEU STARTEN!**

Überprüfen:
```powershell
pwsh --version  # Sollte 7.x oder höher zeigen
```

### Schritt 3: Projekt vorbereiten
```powershell
cd "C:\Pfad\Zu\Tagesplan"
dotnet restore
```

### Schritt 4: Projekt bauen (MUSS vor Playwright!)
```powershell
dotnet build
```

Überprüfen:
```powershell
Test-Path ".\bin\Debug\net8.0-windows\playwright.ps1"
# Muss True zeigen!
```

### Schritt 5: Playwright installieren
```powershell
pwsh .\bin\Debug\net8.0-windows\playwright.ps1 install chromium
```

---

## ⚠️ Häufigster Fehler

### "please ensure to build your project before running playwright"

**Ursache:** Reihenfolge falsch - Playwright vor Build installiert

**Lösung:**
```powershell
# Fix-Skript verwenden:
.\fix-playwright.ps1

# Oder manuell:
dotnet build
pwsh .\bin\Debug\net8.0-windows\playwright.ps1 install chromium
```

---

## 🎯 Anwendung starten

```powershell
# Option 1: Mit dotnet
dotnet run

# Option 2: Direkt die EXE
.\bin\Debug\net8.0-windows\Tagesplan.exe

# Option 3: In Visual Studio
# Tagesplan.sln öffnen → F5 drücken
```

---

## 🔧 Bei Problemen

### Problem beim Setup?
```powershell
.\fix-playwright.ps1
```

### Browser öffnet nicht?
1. Überprüfe ob Edge installiert ist: `winget install Microsoft.Edge`
2. Lösche Browser-Profil: `Remove-Item -Recurse -Force "$env:LOCALAPPDATA\Rechnungen_MEWS\EdgeProfile"`
3. App neu starten

### Detaillierte Fehlerbehebung:
- **PLAYWRIGHT_BUILD_GUIDE.md** - Build & Playwright Details
- **TROUBLESHOOTING.md** - Alle bekannten Probleme
- **INSTALLATION.md** - Vollständige Installationsanleitung

---

## 📦 Benötigte Downloads (ca. Größen)

| Software | Größe | Zweck |
|----------|-------|-------|
| .NET 8 SDK | ~200 MB | Laufzeitumgebung |
| PowerShell Core | ~100 MB | Für Playwright |
| ClosedXML (NuGet) | ~5 MB | Excel-Verarbeitung |
| Playwright (NuGet) | ~10 MB | Browser-Automation |
| Chromium-Browser | ~300 MB | Für MEWS-Login |
| **Gesamt** | **~615 MB** | |

---

## ✅ Installations-Verifizierung

Alle Checks müssen True/Version zeigen:

```powershell
# .NET SDK
dotnet --version

# PowerShell Core
pwsh --version

# Projekt gebaut
Test-Path ".\bin\Debug\net8.0-windows\Tagesplan.exe"

# Playwright installiert
Test-Path "$env:USERPROFILE\.cache\ms-playwright\chromium-*"
```

---

## 🎓 Erste Schritte nach Installation

1. **App starten:** `dotnet run`
2. **MEWS Login testen:**
   - Klick auf "1. MEWS Login"
   - Browser öffnet sich automatisch
   - Manuell in MEWS einloggen
3. **Test-Export:**
   - In MEWS: Berichte → Reservierungsbericht
   - Detaillierte Ansicht aktivieren
   - Export → Excel
   - In App: "2. Bericht herunterladen" oder "Datei wählen..."
   - "3. Listen generieren"

---

## 📖 Weitere Dokumentation

- **SCHNELLSTART.md** - 2-Minuten-Kurzanleitung
- **README.md** - Funktionsübersicht
- **MEWS_EXPORT_GUIDE.md** - MEWS richtig konfigurieren
- **DEBUG_GUIDE.md** - Debug-Ausgaben lesen

---

## 💾 Deinstallation

```powershell
# .NET SDK entfernen
winget uninstall Microsoft.DotNet.SDK.8

# PowerShell Core entfernen (optional)
winget uninstall Microsoft.PowerShell

# Projektordner löschen
Remove-Item -Recurse -Force "C:\Pfad\Zu\Tagesplan"

# Playwright-Cache löschen
Remove-Item -Recurse -Force "$env:USERPROFILE\.cache\ms-playwright"

# Browser-Profil löschen
Remove-Item -Recurse -Force "$env:LOCALAPPDATA\Rechnungen_MEWS"
```

---

## 📞 Support-Checkliste

Wenn du Hilfe brauchst, sammle diese Infos:

1. **Systeminfo:**
   ```powershell
   [System.Environment]::OSVersion.Version
   dotnet --version
   pwsh --version
   ```

2. **Build-Status:**
   ```powershell
   dotnet build > build-log.txt 2>&1
   ```

3. **Playwright-Status:**
   ```powershell
   Test-Path "$env:USERPROFILE\.cache\ms-playwright"
   ```

4. **Debug-Output:**
   - App in Visual Studio starten (F5)
   - Ansicht → Ausgabe → "Debug" auswählen
   - Fehler reproduzieren
   - Output kopieren
