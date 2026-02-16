# Häufige Probleme und Lösungen

## Problem 1: "Driver not found" beim MEWS-Login

### Symptom
```
Fehler: Driver not found: Pfad ... \bin\.playwright\node\win32_x64\node.exe
✗ Fehler beim Öffnen des Browsers
```

### Ursache
Playwright-Browser sind nicht installiert.

### Lösung - Automatisch (Empfohlen)
```powershell
.\fix-playwright.ps1
```

### Lösung - Manuell

**Schritt 1:** PowerShell Core installieren (falls nicht vorhanden)
```powershell
winget install Microsoft.PowerShell
```

**Schritt 2:** PowerShell neu starten und Playwright installieren
```powershell
pwsh .\bin\Debug\net8.0-windows\playwright.ps1 install chromium
```

### Lösung - Automatische Installation in der App
Die Anwendung versucht automatisch, Playwright beim ersten Klick auf "1. MEWS Login" zu installieren:
1. Klicke auf "1. MEWS Login"
2. Warte 2-3 Minuten während der Installation
3. Status wird im Fenster angezeigt
4. Nach erfolgreicher Installation öffnet sich der Browser

---

## Problem 2: PowerShell Execution Policy

### Symptom
```
Die Datei "setup.ps1" kann nicht geladen werden...
```

### Lösung
```powershell
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

---

## Problem 3: .NET SDK nicht gefunden

### Symptom
```
'dotnet' is not recognized as an internal or external command
```

### Lösung

**Option 1 - Mit winget:**
```powershell
winget install Microsoft.DotNet.SDK.8
```

**Option 2 - Manueller Download:**
1. Besuche: https://dotnet.microsoft.com/download/dotnet/8.0
2. Lade ".NET 8.0 SDK" herunter
3. Installiere die heruntergeladene Datei
4. Starte PowerShell neu

**Verifizierung:**
```powershell
dotnet --version
# Sollte anzeigen: 8.0.x
```

---

## Problem 4: NuGet-Pakete können nicht wiederhergestellt werden

### Symptom
```
error: Unable to load the service index for source...
```

### Lösung

**Cache leeren:**
```powershell
dotnet nuget locals all --clear
dotnet restore
```

**NuGet-Quelle zurücksetzen:**
```powershell
dotnet nuget remove source nuget.org
dotnet nuget add source https://api.nuget.org/v3/index.json -n nuget.org
dotnet restore
```

---

## Problem 5: Browser öffnet nicht / bleibt leer

### Mögliche Ursachen & Lösungen

**1. Antivirus blockiert Playwright**
- Füge den Projektordner zur Ausnahmeliste hinzu
- Füge `%USERPROFILE%\.cache\ms-playwright` zur Ausnahmeliste hinzu

**2. Edge ist nicht installiert**
Die App verwendet Microsoft Edge (Channel: msedge). Falls Edge fehlt:
```powershell
winget install Microsoft.Edge
```

**3. Persistent Context Ordner beschädigt**
Lösche das Browser-Profil:
```powershell
Remove-Item -Recurse -Force "$env:LOCALAPPDATA\Rechnungen_MEWS\EdgeProfile"
```
Dann die App neu starten.

---

## Problem 6: Download wird nicht erkannt

### Symptom
```
Timeout: Kein Download erkannt. Bitte versuchen Sie es erneut.
```

### Lösung

**Stelle sicher, dass:**
1. Du in MEWS eingeloggt bist
2. Du zum Reservierungsbericht navigiert hast
3. Du auf "Export" → "Excel" klickst
4. Die App läuft und auf den Download wartet

**Alternative:**
1. Lade die Datei manuell in MEWS herunter
2. Klicke in der App auf "Datei wählen..."
3. Wähle die heruntergeladene Excel-Datei
4. Klicke auf "3. Listen generieren"

---

## Problem 7: Excel-Listen werden nicht generiert

### Symptom
```
Fehler beim Verarbeiten der Datei
```

### Debugging-Schritte

**1. Überprüfe Excel-Datei:**
- Ist es eine .xlsx-Datei?
- Enthält sie ein Blatt "Reservierungen"?
- Wurde "Detaillierte Ansicht" in MEWS aktiviert?
- Sind "Produkte" und "Notizen" sichtbar?

**2. Debug-Output ansehen:**
1. Starte die App in Visual Studio (F5)
2. Öffne: Ansicht → Ausgabe
3. Wähle "Debug" in der Dropdown-Liste
4. Führe den Import aus
5. Sieh die detaillierte Ausgabe

**3. Test-Export in MEWS:**
```
Berichte → Reservierungsbericht
✓ Detaillierte Ansicht
✓ Produkte einblenden
✓ Notizen einblenden (Gästenotizen, Reservierungsnotizen, Channel-Notizen)
Zeitraum: Heute + 14 Tage
```

---

## Problem 8: Personenanzahl ist falsch

### Symptom
- Alle Reservierungen zeigen "0 Personen"
- Oder: Zahlen viel zu hoch

### Lösung
Stelle sicher, dass der MEWS-Export das Blatt "Alterskategorien" enthält:

1. In MEWS: Reservierungsbericht öffnen
2. Klicke auf Zahnrad-Symbol (Einstellungen)
3. Aktiviere: "Alterskategorien anzeigen"
4. Exportiere erneut

---

## Problem 9: Nächte-Anzahl ist falsch

### Symptom
- Anzeige: 0 Nächte (sollte 1 sein)
- Oder: -1 von der richtigen Anzahl

### Status
✅ Wurde in Version 1.1 behoben
- Problem war: DateTime-Subtraktion mit Uhrzeit
- Lösung: Nur Datums-Teil verwenden (`.Date`)

Stelle sicher, dass du die neueste Version verwendest:
```powershell
git pull
dotnet build
```

---

## Weitere Hilfe

### Debug-Informationen sammeln

Wenn du Support benötigst, sammle folgende Informationen:

1. **Version:**
   ```powershell
   dotnet --version
   ```

2. **Build-Log:**
   ```powershell
   dotnet build > build-log.txt 2>&1
   ```

3. **Debug-Output:**
   - Starte App in Visual Studio
   - Ansicht → Ausgabe → "Debug"
   - Kopiere relevante Zeilen

4. **Playwright-Status:**
   ```powershell
   pwsh .\bin\Debug\net8.0-windows\playwright.ps1 install --dry-run
   ```

### Nützliche Befehle

**Komplett neu bauen:**
```powershell
dotnet clean
dotnet restore
dotnet build
```

**Alle NuGet-Pakete aktualisieren:**
```powershell
dotnet list package --outdated
dotnet add package ClosedXML
dotnet add package Microsoft.Playwright
```

**Playwright komplett neu installieren:**
```powershell
# Alte Installation löschen
Remove-Item -Recurse -Force "$env:USERPROFILE\.cache\ms-playwright"

# Neu installieren
pwsh .\bin\Debug\net8.0-windows\playwright.ps1 install chromium
```

---

## Kontakt & Support

Bei anhaltenden Problemen:
1. Siehe `DEBUG_GUIDE.md` für erweiterte Diagnose
2. Prüfe die Visual Studio Ausgabe (Debug-Fenster)
3. Überprüfe die MEWS-Export-Konfiguration in `MEWS_EXPORT_GUIDE.md`
