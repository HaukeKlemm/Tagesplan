# Schnellstart-Anleitung

## Installation auf neuem Rechner - 2 Minuten Setup! ⚡

### Automatische Installation (Empfohlen)

1. **ZIP entpacken**
2. **PowerShell als Administrator öffnen**
   - Windows-Taste → "PowerShell" → Rechtsklick → "Als Administrator"
3. **Zum Ordner navigieren & Setup starten**
   ```powershell
   cd "C:\Pfad\Zu\Tagesplan"
   .\setup.ps1
   ```

Das war's! Das Skript installiert automatisch:
- ✅ .NET 8 SDK
- ✅ PowerShell Core (für Playwright)
- ✅ NuGet-Pakete (ClosedXML, Playwright)
- ✅ Browser für MEWS-Automation

**WICHTIG:** Das Projekt wird automatisch gebaut. Nach dem Setup ist alles bereit!

---

## Manuelle Installation (falls benötigt)

```powershell
# 1. .NET 8 installieren
winget install Microsoft.DotNet.SDK.8

# 2. PowerShell Core installieren (WICHTIG für Playwright!)
winget install Microsoft.PowerShell

# 3. PowerShell NEU STARTEN, dann:
cd "C:\Pfad\Zu\Tagesplan"

# 4. Pakete installieren
dotnet restore

# 5. Projekt bauen (WICHTIG - muss VOR Playwright passieren!)
dotnet build

# 6. Playwright installieren
pwsh .\bin\Debug\net8.0-windows\playwright.ps1 install chromium
```

⚠️ **Reihenfolge beachten:** Build MUSS vor Playwright-Installation erfolgen!

---

## Häufigster Fehler & Lösung

### ❌ "please ensure to build your project before running playwright"

**Ursache:** Projekt wurde noch nicht gebaut oder PowerShell Core fehlt

**Lösung:**
```powershell
# PowerShell Core installieren (falls noch nicht vorhanden)
winget install Microsoft.PowerShell

# PowerShell neu starten, dann Projekt bauen
dotnet build

# DANN Playwright installieren
pwsh .\bin\Debug\net8.0-windows\playwright.ps1 install chromium

# Oder verwende das Fix-Skript:
.\fix-playwright.ps1
```

---

## Anwendung starten

```powershell
# Einfach ausführen:
dotnet run

# ODER die .exe direkt:
.\bin\Release\net8.0-windows\Tagesplan.exe
```

---

## Was wird installiert?

| Software | Größe | Zweck |
|----------|-------|-------|
| .NET 8 SDK | ~200 MB | Laufzeitumgebung |
| ClosedXML | ~5 MB | Excel-Dateien lesen/schreiben |
| Playwright | ~300 MB | MEWS Browser-Automation |
| **Gesamt** | **~500 MB** | |

---

## Erste Schritte nach Installation

1. **Anwendung starten** → `dotnet run`
2. **MEWS Login testen** → Button "1. MEWS Login" klicken
3. **Beispiel-Export testen** → MEWS-Datei auswählen → "3. Listen generieren"

---

## Dokumentation

📄 **INSTALLATION.md** - Detaillierte Installationsanleitung  
📄 **README.md** - Vollständige Funktionsbeschreibung  
📄 **MEWS_EXPORT_GUIDE.md** - MEWS Export richtig konfigurieren  
📄 **DEBUG_GUIDE.md** - Probleme lösen  

---

## Support & Fehlerbehebung

**Problem:** "dotnet nicht gefunden"  
→ PowerShell neu starten nach .NET-Installation

**Problem:** "Playwright fehlt"  
→ `pwsh .\bin\Debug\net8.0-windows\playwright.ps1 install`

**Problem:** "Execution Policy Error"  
→ `Set-ExecutionPolicy RemoteSigned -Scope CurrentUser`

Mehr Details in **INSTALLATION.md**!
