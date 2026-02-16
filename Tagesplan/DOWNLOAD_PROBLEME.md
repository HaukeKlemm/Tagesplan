# Download-Probleme - Weißes Fenster / Datei nicht gefunden

## Problem: Download wird als weißes Fenster angezeigt

### Mögliche Ursachen:
1. MEWS-Download ist ein Pop-up, das blockiert wird
2. Browser-Download-Einstellungen blockieren automatische Downloads
3. Playwright kann den Download nicht abfangen

---

## Wo wird die Datei gespeichert?

### Standard-Download-Pfad:
```
C:\Users\[IhrBenutzername]\Documents\MEWS_Downloads\
```

### In der Anwendung anzeigen:
Die Anwendung zeigt jetzt beim Download:
```
Download-Ziel: C:\Users\...\Documents\MEWS_Downloads\
✓ Download-Ordner bereit: ...
Speichere Datei: Reservierungsbericht_2024.xlsx...
✓ Datei gespeichert: C:\...\MEWS_Downloads\Reservierungsbericht_2024.xlsx
```

**Der Ordner öffnet sich automatisch nach erfolgreichem Download!**

---

## 🔍 Datei finden

### Option 1: In der Anwendung
Die Anwendung zeigt jetzt den kompletten Pfad an und öffnet den Ordner automatisch.

### Option 2: Manuell suchen
```powershell
# Im Explorer:
%USERPROFILE%\Documents\MEWS_Downloads\

# Oder PowerShell:
explorer "$env:USERPROFILE\Documents\MEWS_Downloads"
```

### Option 3: Windows-Suche
1. Windows-Taste drücken
2. Suchen: `MEWS_Downloads`
3. Ordner öffnen

---

## Lösungen für "weißes Fenster" Problem

### Lösung 1: Pop-up-Blocker deaktivieren (in MEWS)

**In Edge (Playwright-Browser):**
1. Klicken Sie auf das **Puzzle-Symbol** (Erweiterungen) in der Browser-Leiste
2. Oder: Edge-Einstellungen → Cookies und Websiteberechtigungen → Pop-ups und Weiterleitungen
3. Für `https://app.mews.com` → **Zulassen**

### Lösung 2: Download manuell starten

Wenn der automatische Download nicht funktioniert:

1. **Im MEWS-Browser:**
   - Gehen Sie zu: Berichte → Reservierungsbericht
   - Konfigurieren Sie den Bericht (Detaillierte Ansicht, Produkte, Notizen)
   - Klicken Sie auf **Export → Excel**

2. **Warten Sie:**
   - Die Anwendung wartet 5 Minuten auf den Download
   - Status wird angezeigt: "Warte auf Download..."

3. **Datei wird automatisch gespeichert:**
   - Nach dem Klick auf Excel in MEWS
   - Wird die Datei automatisch nach `Documents\MEWS_Downloads\` gespeichert

### Lösung 3: Alternative - Datei manuell wählen

Wenn der automatische Download nicht funktioniert:

1. **In MEWS:**
   - Export starten wie gewohnt
   - Datei wird in Browser-Downloads gespeichert (z.B. `Downloads\`)

2. **In der Anwendung:**
   - Klicken Sie auf **"Datei wählen..."** Button
   - Navigieren Sie zu Ihrem Downloads-Ordner
   - Wählen Sie die heruntergeladene Excel-Datei

3. **Listen generieren:**
   - Klicken Sie auf **"3. Listen generieren"**
   - Funktioniert genauso wie mit automatischem Download

---

## Debug: Überprüfen Sie den Download-Pfad

### In der Anwendung:
Die Statusmeldungen zeigen jetzt:
```
Download-Ziel: C:\Users\offic\Documents\MEWS_Downloads\
✓ Download-Ordner bereit
```

### Manuell überprüfen:
```powershell
# PowerShell öffnen und ausführen:
$downloadPath = Join-Path $env:USERPROFILE "Documents\MEWS_Downloads"
Write-Host "Download-Pfad: $downloadPath"
Test-Path $downloadPath  # Sollte True zeigen
explorer $downloadPath   # Öffnet den Ordner
```

---

## Typische Fehlerquellen

### 1. "Datei nicht gefunden" nach Download

**Ursache:** Datei wurde woanders gespeichert

**Lösung:** Suchen Sie nach `*.xlsx` in:
```
C:\Users\[IhrName]\Downloads\
C:\Users\[IhrName]\Documents\
C:\Users\[IhrName]\Desktop\
```

### 2. "Weißes Fenster" bleibt hängen

**Ursache:** MEWS-Pop-up wird blockiert

**Lösung:**
1. Pop-up-Blocker für mews.com deaktivieren
2. Oder: Verwenden Sie "Datei wählen..." Button stattdessen

### 3. Download startet nicht

**Ursache:** Browser wartet auf Benutzeraktion

**Lösung:**
1. Überprüfen Sie, ob ein Download-Dialog erscheint
2. Klicken Sie auf "Speichern" falls nötig
3. Warten Sie, bis "Download erfolgreich" erscheint

---

## Erweiterte Diagnose

### Debug-Ausgabe ansehen:

1. **Starten Sie die App in Visual Studio** (F5)
2. **Öffnen Sie:** Ansicht → Ausgabe
3. **Wählen Sie:** "Debug" in der Dropdown-Liste
4. **Suchen Sie nach:**
   ```
   MewsAutomation initialized:
     Download-Pfad: C:\Users\...\Documents\MEWS_Downloads\
     Browser-Profil: C:\Users\...\AppData\Local\Rechnungen_MEWS\EdgeProfile
   ```

### Download-Verhalten testen:

```powershell
# Erstellen Sie eine Test-Datei im Download-Ordner:
$testFile = Join-Path $env:USERPROFILE "Documents\MEWS_Downloads\test.txt"
"Test" | Out-File $testFile
Write-Host "Test-Datei erstellt: $testFile"
Test-Path $testFile  # Sollte True zeigen
explorer (Split-Path $testFile)  # Öffnet den Ordner
```

---

## Bekannte Browser-Einstellungen

### Edge Download-Einstellungen:

1. **Öffnen Sie Edge-Einstellungen** (im Playwright-Browser)
2. **Downloads:**
   - "Downloads vor dem Speichern fragen" → **AUS**
   - Download-Speicherort → Überprüfen

3. **Pop-ups und Weiterleitungen:**
   - `https://app.mews.com` → Zulassen

---

## Alternative Arbeitsweise

Wenn der automatische Download nicht zuverlässig funktioniert:

### Empfohlener Workflow:

1. **Login in MEWS** (Button "1. MEWS Login")
2. **Manuell downloaden:**
   - In MEWS: Berichte → Reservierungsbericht → Export → Excel
   - Datei wird in Downloads-Ordner gespeichert
3. **In der App:** Button "Datei wählen..." verwenden
4. **Wählen Sie die Datei** aus Downloads
5. **Klicken Sie:** "3. Listen generieren"

**Vorteil:**
- Zuverlässiger
- Sie sehen den Download-Fortschritt
- Funktioniert immer

---

## Zusammenfassung

| Problem | Lösung |
|---------|--------|
| **Weißes Fenster** | Pop-up-Blocker deaktivieren für mews.com |
| **Datei nicht gefunden** | Siehe `%USERPROFILE%\Documents\MEWS_Downloads\` |
| **Download startet nicht** | "Datei wählen..." Button verwenden |
| **Unsicher wo Datei ist** | App zeigt jetzt den kompletten Pfad an |
| **Ordner öffnen** | Wird automatisch nach Download geöffnet |

---

## Verbesserungen in der aktuellen Version

✅ Download-Pfad wird vor Download angezeigt  
✅ Ordner wird automatisch nach Download geöffnet  
✅ Vollständiger Dateipfad wird in Statusmeldungen angezeigt  
✅ Debug-Ausgabe zeigt alle Pfade beim Start  
✅ Bessere Fehlermeldungen mit Pfadangaben  

**Sie sehen jetzt immer genau, wo die Datei gespeichert wird!** 🎯
