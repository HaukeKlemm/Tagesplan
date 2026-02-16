# Pfade und Ordnerstruktur

## Automatische Pfaderkennung

Die Anwendung erkennt automatisch alle Pfade zur Laufzeit. Sie müssen keine Pfade manuell anpassen!

---

## Wichtige Pfade (werden automatisch ermittelt)

### 1. Anwendungs-Pfad (Assembly Location)
Wo die ausgeführte .exe liegt:
```
Beispiel: C:\Users\[Name]\source\repos\Tagesplan\bin\Debug\net8.0-windows\
```

Wird verwendet für:
- Playwright-Skript: `playwright.ps1` im selben Ordner
- Dependencies (DLLs)

### 2. Download-Ordner
Standardmäßig:
```
C:\Users\[Name]\Documents\MEWS_Downloads\
```

Konfigurierbar in `Configuration\AppConfig.cs`

### 3. Browser-Profil
```
C:\Users\[Name]\AppData\Local\Rechnungen_MEWS\EdgeProfile\
```

Speichert MEWS Login-Session

### 4. Playwright-Cache
```
C:\Users\[Name]\.cache\ms-playwright\
```

Hier werden die Browser-Binaries gespeichert

---

## Pfade in Fehlermeldungen

Die Anwendung zeigt **immer den tatsächlichen Pfad** an, wo sie nach Dateien sucht.

### Beispiel-Fehlermeldung:
```
✗ Playwright-Skript nicht gefunden!
  Erwarteter Pfad: C:\Users\Max\source\repos\Tagesplan\bin\Debug\net8.0-windows\playwright.ps1
```

→ Der gezeigte Pfad ist **genau der Pfad auf Ihrem Rechner**!

---

## Projekt-Struktur (relativ)

```
Tagesplan\                          ← Projektroot
├── setup.ps1                       ← Setup-Skript
├── fix-playwright.ps1              ← Playwright-Fix
├── INSTALLATION.md                 ← Dokumentation
├── Tagesplan.sln                   ← Solution-Datei
│
├── Models\                         ← Datenmodelle
├── Services\                       ← Business Logic
├── Helpers\                        ← Hilfsfunktionen
├── Configuration\                  ← Konfiguration
│
└── bin\                            ← Build-Output
    └── Debug\                      ← Debug-Build
        └── net8.0-windows\         ← Target Framework
            ├── Tagesplan.exe       ← Anwendung
            ├── playwright.ps1      ← Playwright-Skript (nach Build!)
            └── *.dll               ← Dependencies
```

---

## Relative Pfade von der Anwendung aus

Wenn die App läuft in:
```
C:\Users\Max\Desktop\Tagesplan\bin\Debug\net8.0-windows\Tagesplan.exe
```

Dann ist der Projektroot:
```
..\..\..\..\ → C:\Users\Max\Desktop\Tagesplan\
```

Die App berechnet das automatisch!

---

## Warum Pfade wichtig sind

### Problem: Playwright-Skript nicht gefunden

**Ursache:** Projekt wurde nicht gebaut

**Erklärung:**
1. Playwright ist ein NuGet-Paket
2. Beim Build wird `playwright.ps1` nach `bin\` kopiert
3. Ohne Build existiert das Skript nicht
4. Die App zeigt den erwarteten Pfad an

**Lösung:**
```powershell
# Option 1: In Visual Studio
F6 drücken (Build)

# Option 2: PowerShell (im Projektroot!)
dotnet build
```

---

## Pfade in Fehlermeldungen verwenden

### ✅ So verwenden Sie die Pfade richtig:

Wenn die App zeigt:
```
Erwarteter Pfad: C:\Users\Max\...\playwright.ps1
Manuell installieren mit:
pwsh "C:\Users\Max\...\playwright.ps1" install chromium
```

→ **Kopieren Sie den kompletten Befehl** aus der Fehlermeldung!

### ❌ Nicht verwenden:

Befehle aus der Dokumentation wie:
```powershell
pwsh .\bin\Debug\net8.0-windows\playwright.ps1 install chromium
```

→ Funktioniert nur, wenn Sie im richtigen Ordner sind!

---

## Ordner-Navigation in PowerShell

### Zum Projektroot navigieren:

```powershell
# Wenn Sie die .exe kennen:
cd "C:\Users\Max\Desktop\Tagesplan"

# Oder relativ von bin\Debug\net8.0-windows\:
cd ..\..\..\..
```

### Testen ob Sie im richtigen Ordner sind:

```powershell
# Sollte Tagesplan.sln zeigen:
dir *.sln

# Sollte setup.ps1 und fix-playwright.ps1 zeigen:
dir *.ps1
```

---

## Fix-Skript findet Pfade automatisch

Das `fix-playwright.ps1` Skript sucht automatisch nach:
- `bin\Debug\net8.0-windows\playwright.ps1`
- `bin\Release\net8.0-windows\playwright.ps1`

**Deshalb:** Fix-Skript immer **im Projektroot** ausführen!

```powershell
cd "C:\Pfad\Zu\Tagesplan"   # Projektroot
.\fix-playwright.ps1         # Findet alles automatisch
```

---

## Zusammenfassung

| Was | Wo | Automatisch? |
|-----|-----|--------------|
| **Anwendungs-Pfad** | Wird zur Laufzeit ermittelt | ✅ Ja |
| **Playwright-Skript** | Im selben Ordner wie .exe | ✅ Ja |
| **Download-Ordner** | Dokumente\MEWS_Downloads | ✅ Ja |
| **Browser-Profil** | AppData\Local\Rechnungen_MEWS | ✅ Ja |
| **Fehlermeldung-Pfade** | Zeigt echte Pfade | ✅ Ja |
| **Fix-Skript Ausführung** | Muss im Projektroot sein | ❌ Manuell |
| **Build vor Playwright** | Muss manuell gemacht werden | ❌ Manuell |

---

## Tipps

1. **Fehlermeldungen genau lesen** - Die App zeigt immer den korrekten Pfad
2. **Befehle aus Fehlermeldungen kopieren** - Nicht aus Dokumentation
3. **Projektroot merken** - Wo `Tagesplan.sln` liegt
4. **Build zuerst** - Dann erst Playwright installieren

---

## Bei Problemen

Die App zeigt jetzt:
- ✅ Tatsächliche Pfade (nicht Platzhalter)
- ✅ Copy-paste-fähige Befehle
- ✅ Mehrere Lösungsvorschläge

**Beispiel:**
```
✗ Playwright-Skript nicht gefunden!
  Erwarteter Pfad: C:\Users\Max\Desktop\Tagesplan\bin\Debug\net8.0-windows\playwright.ps1
  
  Lösungsvorschläge:
  1. In Visual Studio: Drücken Sie F6 zum Bauen
  2. In PowerShell: dotnet build
  3. Schließen Sie die App, bauen Sie das Projekt, starten Sie neu
```

→ Wählen Sie eine Lösung und folgen Sie den Anweisungen!
