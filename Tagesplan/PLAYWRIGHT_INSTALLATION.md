# Playwright Installation Guide

## Automatische Installation (Empfohlen)

Wenn Sie die Anwendung zum ersten Mal starten und auf "1. MEWS Login" klicken, wird Playwright möglicherweise automatisch nach den fehlenden Browsern fragen.

## Manuelle Installation

Falls die automatische Installation nicht funktioniert, führen Sie folgende Schritte aus:

### Option 1: PowerShell-Skript

1. Öffnen Sie PowerShell als Administrator
2. Navigieren Sie zum Projektverzeichnis:
   ```powershell
   cd "C:\Pfad\zu\Tagesplan\bin\Debug\net8.0-windows"
   ```
3. Führen Sie das Installationsskript aus:
   ```powershell
   pwsh playwright.ps1 install chromium
   ```

### Option 2: Über NuGet Package Manager Console

1. Öffnen Sie Visual Studio
2. Öffnen Sie die "Package Manager Console": **Tools → NuGet Package Manager → Package Manager Console**
3. Führen Sie aus:
   ```powershell
   cd $env:USERPROFILE\.nuget\packages\microsoft.playwright\1.41.2\lib\net8.0
   pwsh playwright.ps1 install chromium
   ```

### Option 3: Kommandozeile nach dem Build

1. Bauen Sie das Projekt einmal in Visual Studio (F5 oder Ctrl+Shift+B)
2. Öffnen Sie eine Eingabeaufforderung
3. Navigieren Sie zum Output-Verzeichnis:
   ```cmd
   cd "C:\Pfad\zu\Tagesplan\bin\Debug\net8.0-windows"
   ```
4. Führen Sie aus:
   ```cmd
   playwright.ps1 install chromium
   ```

## Was wird installiert?

Playwright lädt folgende Komponenten herunter:
- **Chromium Browser** (~150 MB)
- Notwendige Browser-Treiber
- Abhängigkeiten für die Browser-Automation

## Speicherort

Die Browser werden installiert in:
```
%USERPROFILE%\AppData\Local\ms-playwright
```

## Fehlerbehebung

### Fehler: "pwsh" nicht gefunden
Sie benötigen PowerShell Core:
1. Laden Sie PowerShell 7+ herunter: https://github.com/PowerShell/PowerShell/releases
2. Installieren Sie PowerShell Core
3. Versuchen Sie die Installation erneut

### Fehler: Zugriff verweigert
Führen Sie die Installation mit Administrator-Rechten aus:
1. Rechtsklick auf PowerShell → "Als Administrator ausführen"
2. Führen Sie das Installationsskript aus

### Fehler: Datei nicht gefunden
Stellen Sie sicher, dass Sie das Projekt mindestens einmal gebaut haben:
1. In Visual Studio: **Build → Build Solution** (Ctrl+Shift+B)
2. Warten Sie bis der Build erfolgreich ist
3. Versuchen Sie die Installation erneut

## Verifizierung

Um zu prüfen, ob Playwright korrekt installiert ist:

```powershell
pwsh playwright.ps1 --version
```

Sie sollten die Version sehen: `Version 1.41.2` (oder höher)

## Systemanforderungen

- Windows 10/11 (64-bit)
- .NET 8.0 Runtime
- ~300 MB freier Speicherplatz
- Internetverbindung für den Download

## Proxy-Konfiguration

Falls Sie hinter einem Proxy sind:

```powershell
# Setzen Sie die Proxy-Umgebungsvariablen:
$env:HTTP_PROXY = "http://proxy.firma.de:8080"
$env:HTTPS_PROXY = "http://proxy.firma.de:8080"

# Dann Installation ausführen:
pwsh playwright.ps1 install chromium
```

## Alternative: System-Chrome verwenden

Playwright kann auch Ihren installierten Chrome-Browser verwenden, aber für beste Kompatibilität wird empfohlen, den von Playwright verwalteten Chromium-Browser zu nutzen.

---

Bei weiteren Problemen kontaktieren Sie bitte Ihren System-Administrator.
