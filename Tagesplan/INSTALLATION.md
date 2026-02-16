# Installation auf einem neuen Rechner

## Automatische Installation (Empfohlen)

### Option 1: Mit setup.ps1 Skript

1. **ZIP-Datei extrahieren**
   ```
   Rechtsklick auf Tagesplan.zip → "Alle extrahieren..."
   ```

2. **PowerShell als Administrator öffnen**
   ```
   Windows-Taste → "PowerShell" eingeben → Rechtsklick → "Als Administrator ausführen"
   ```

3. **Zum Projektordner navigieren**
   ```powershell
   cd "C:\Pfad\Zu\Tagesplan"
   ```

4. **Setup-Skript ausführen**
   ```powershell
   .\setup.ps1
   ```

Das Skript installiert automatisch:
- ✅ .NET 8 SDK
- ✅ NuGet-Pakete
- ✅ Playwright Browser

---

## Manuelle Installation

### Schritt 1: .NET 8 SDK installieren

```powershell
# Mit winget (in Windows 11 vorinstalliert)
winget install Microsoft.DotNet.SDK.8

# ODER manuell herunterladen:
# https://dotnet.microsoft.com/download/dotnet/8.0
```

**Installation überprüfen:**
```powershell
dotnet --version
# Sollte anzeigen: 8.0.x
```

### Schritt 2: Zum Projektordner navigieren

```powershell
cd "C:\Pfad\Zu\Tagesplan"
```

### Schritt 3: NuGet-Pakete wiederherstellen

```powershell
dotnet restore
```

### Schritt 4: Projekt bauen

```powershell
dotnet build
```

### Schritt 5: Playwright installieren (für MEWS-Automation)

```powershell
pwsh .\bin\Debug\net8.0-windows\playwright.ps1 install
```

---

## Anwendung starten

### Option 1: Mit dotnet run
```powershell
dotnet run
```

### Option 2: Direkt die .exe starten
```powershell
.\bin\Debug\net8.0-windows\Tagesplan.exe
```

### Option 3: In Visual Studio
1. `Tagesplan.sln` öffnen
2. F5 drücken oder "Debuggen starten"

---

## Systemanforderungen

- **Betriebssystem:** Windows 10 oder Windows 11
- **Arbeitsspeicher:** Mindestens 4 GB RAM
- **Festplattenspeicher:** Ca. 2 GB frei
- **Internet:** Für Download von .NET SDK und NuGet-Paketen
- **Browser:** Chrome (wird von Playwright für MEWS-Automation verwendet)

---

## Benötigte Software

### .NET 8 SDK
- **Größe:** Ca. 200 MB Download
- **Download:** https://dotnet.microsoft.com/download/dotnet/8.0
- **Lizenz:** Kostenlos (MIT)

### NuGet-Pakete (werden automatisch installiert)
- **ClosedXML** (0.104.2) - Excel-Dateien verarbeiten
- **Microsoft.Playwright** (1.49.0) - Browser-Automation für MEWS
- **ExcelNumberFormat** (1.1.0) - Excel-Formatierung

### Playwright Browser
- **Größe:** Ca. 300 MB
- **Installation:** Automatisch beim ersten Start oder manuell mit Skript
- **Browser:** Chromium (für MEWS-Login)

---

## Troubleshooting

### Problem: "dotnet" Befehl nicht gefunden

**Lösung:**
1. .NET 8 SDK neu installieren
2. PowerShell neu starten
3. Überprüfen: `dotnet --version`

### Problem: "Playwright nicht gefunden"

**Lösung:**
```powershell
# Im Projektordner ausführen:
pwsh .\bin\Debug\net8.0-windows\playwright.ps1 install
```

### Problem: "NuGet restore failed"

**Lösung:**
```powershell
# NuGet-Cache leeren
dotnet nuget locals all --clear

# Erneut versuchen
dotnet restore
```

### Problem: "Access Denied" beim Setup-Skript

**Lösung:**
PowerShell als Administrator starten:
```
Windows-Taste → "PowerShell" → Rechtsklick → "Als Administrator ausführen"
```

### Problem: Execution Policy verhindert Skript-Ausführung

**Lösung:**
```powershell
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

---

## Erste Schritte nach Installation

1. **Anwendung starten**
   ```powershell
   dotnet run
   ```

2. **MEWS Login testen**
   - Klick auf "1. MEWS Login"
   - Browser öffnet sich
   - Manuell in MEWS einloggen

3. **Beispiel-Datei testen**
   - Klick auf "Datei wählen..."
   - MEWS-Reservierungsbericht auswählen
   - "3. Listen generieren" klicken

---

## Deinstallation

### .NET 8 SDK entfernen
```powershell
winget uninstall Microsoft.DotNet.SDK.8
```

### Projektordner löschen
```
Einfach den kompletten Tagesplan-Ordner löschen
```

### Playwright Browser entfernen
```powershell
# Playwright-Daten löschen
Remove-Item -Recurse -Force "$env:USERPROFILE\.cache\ms-playwright"
```

---

## Optional: Visual Studio installieren

Für Entwicklung und Debugging:

```powershell
# Visual Studio Community 2022 (kostenlos)
winget install Microsoft.VisualStudio.2022.Community

# ODER Visual Studio Code (leichter)
winget install Microsoft.VisualStudioCode
```

---

## Lizenz

Diese Software ist für den internen Gebrauch bestimmt.
Alle Abhängigkeiten (NuGet-Pakete) haben ihre eigenen Lizenzen:
- ClosedXML: MIT License
- Playwright: Apache License 2.0

---

## Support

Bei Problemen:
1. Siehe Troubleshooting-Abschnitt oben
2. Überprüfe Debug-Output in Visual Studio (Ansicht → Ausgabe)
3. Siehe `DEBUG_GUIDE.md` für detaillierte Fehlerbehebung
