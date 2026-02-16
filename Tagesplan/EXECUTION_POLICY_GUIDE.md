# PowerShell Execution Policy - Digitale Signatur Problem

## Problem: "playwright.ps1 ist nicht digital signiert"

### Vollständige Fehlermeldung:
```
Die Datei "C:\...\playwright.ps1" kann nicht geladen werden.
Die Datei ist nicht digital signiert.
Sie können dieses Skript im aktuellen System nicht ausführen.
```

---

## ✅ Gute Nachrichten: Die Anwendung löst das automatisch!

Die C#-Anwendung verwendet **automatisch** `-ExecutionPolicy Bypass` beim Ausführen von playwright.ps1.

**Das bedeutet:** Normalerweise tritt dieses Problem **NICHT** auf!

---

## Wann tritt das Problem auf?

Das Problem tritt nur auf, wenn Sie **manuell** in PowerShell arbeiten:

```powershell
# ❌ Funktioniert nicht (wenn Execution Policy strikt ist):
pwsh .\bin\Debug\net8.0-windows\playwright.ps1 install chromium

# ✅ Funktioniert immer:
pwsh -ExecutionPolicy Bypass -File .\bin\Debug\net8.0-windows\playwright.ps1 install chromium
```

---

## Lösungen

### Lösung 1: -ExecutionPolicy Bypass verwenden (Empfohlen)

**Für Playwright:**
```powershell
pwsh -ExecutionPolicy Bypass -File .\bin\Debug\net8.0-windows\playwright.ps1 install chromium
```

**Für Setup-Skript:**
```powershell
powershell -ExecutionPolicy Bypass -File .\setup.ps1
```

**Für Fix-Skript:**
```powershell
powershell -ExecutionPolicy Bypass -File .\fix-playwright.ps1
```

**Vorteil:** 
- Funktioniert immer
- Nur für diesen einen Befehl
- Keine permanente Änderung der System-Einstellungen

---

### Lösung 2: Execution Policy dauerhaft ändern

**Für aktuellen Benutzer (empfohlen):**
```powershell
Set-ExecutionPolicy RemoteSigned -Scope CurrentUser
```

**Was das bedeutet:**
- `RemoteSigned`: Lokale Skripte dürfen ausgeführt werden
- Heruntergeladene Skripte müssen signiert sein
- Nur für Ihren Benutzer, nicht systemweit

**Dann können Sie normal ausführen:**
```powershell
.\setup.ps1
pwsh .\bin\Debug\net8.0-windows\playwright.ps1 install chromium
```

---

### Lösung 3: Nur für aktuelles PowerShell-Fenster

```powershell
Set-ExecutionPolicy Bypass -Scope Process
```

**Was das bedeutet:**
- Gilt nur für das aktuelle PowerShell-Fenster
- Nach Schließen wieder Standard
- Sicher für temporäre Nutzung

---

## Was macht die Anwendung automatisch?

In `MewsAutomation.cs`:
```csharp
var processStartInfo = new System.Diagnostics.ProcessStartInfo
{
    FileName = "pwsh",
    Arguments = $"-ExecutionPolicy Bypass -File \"{playwrightScriptPath}\" install chromium",
    // ...
};
```

**Ergebnis:** Die Anwendung umgeht automatisch die Execution Policy!

---

## Vergleich: Welche Policy ist gesetzt?

**Aktuell anzeigen:**
```powershell
Get-ExecutionPolicy -List
```

**Typische Ausgabe:**
```
Scope          ExecutionPolicy
-----          ---------------
MachinePolicy  Undefined
UserPolicy     Undefined
Process        Undefined
CurrentUser    RemoteSigned
LocalMachine   AllSigned
```

---

## Unterschiede zwischen Policies

| Policy | Bedeutung | Empfohlen für |
|--------|-----------|---------------|
| **Restricted** | Keine Skripte | Maximale Sicherheit |
| **AllSigned** | Alle müssen signiert sein | Firmen-Umgebungen |
| **RemoteSigned** | Lokale OK, Remote signiert | Standard-Empfehlung |
| **Unrestricted** | Alle erlaubt, Warnung | Entwickler |
| **Bypass** | Alle erlaubt, keine Warnung | Temporär, Automation |

---

## Für Setup-Skripte

Unsere Skripte verwenden bereits Bypass intern:

**setup.ps1** verwendet:
```powershell
Start-Process -FilePath "pwsh" -ArgumentList "-ExecutionPolicy Bypass -File ..."
```

**fix-playwright.ps1** verwendet:
```powershell
Start-Process -FilePath "pwsh" -ArgumentList "-ExecutionPolicy Bypass -File ..."
```

**Daher:** Normalerweise kein Problem!

---

## Wenn gar nichts funktioniert

### Manuelle Installation ohne Skripte:

1. **PowerShell Core installieren:**
   ```powershell
   winget install Microsoft.PowerShell
   ```

2. **Projekt bauen:**
   ```powershell
   dotnet build
   ```

3. **Playwright MANUELL installieren:**
   
   a) Öffne das playwright.ps1 Skript in Editor
   
   b) Kopiere den Inhalt
   
   c) Führe die Befehle manuell aus (ohne Skript-Datei)

---

## Best Practices

1. **In der Anwendung:** Kein Problem, läuft automatisch
2. **Setup-Skript ausführen:** `powershell -ExecutionPolicy Bypass -File .\setup.ps1`
3. **Manuell troubleshooten:** Immer `-ExecutionPolicy Bypass` verwenden
4. **Dauerhaft arbeiten:** `Set-ExecutionPolicy RemoteSigned -Scope CurrentUser`

---

## Zusammenfassung

| Situation | Lösung |
|-----------|--------|
| **Anwendung verwenden** | ✅ Funktioniert automatisch |
| **Setup-Skript** | `powershell -ExecutionPolicy Bypass -File .\setup.ps1` |
| **Fix-Skript** | `powershell -ExecutionPolicy Bypass -File .\fix-playwright.ps1` |
| **Manuell Playwright** | `pwsh -ExecutionPolicy Bypass -File .\bin\...\playwright.ps1 install chromium` |
| **Dauerhaft ändern** | `Set-ExecutionPolicy RemoteSigned -Scope CurrentUser` |

---

## Sicherheitshinweis

**Ist `-ExecutionPolicy Bypass` sicher?**

✅ **Ja**, wenn Sie **wissen**, was das Skript macht (wie bei Playwright)

❌ **Nein**, bei **unbekannten** Skripten aus dem Internet

**Unser Fall:** 
- playwright.ps1 kommt von Microsoft Playwright (vertrauenswürdig)
- setup.ps1 und fix-playwright.ps1 sind Teil dieses Projekts (Sie können den Code sehen)

**Daher:** Sicher zu verwenden!

---

## Weitere Hilfe

- Siehe `TROUBLESHOOTING.md` für weitere Probleme
- Siehe `INSTALLATION.md` für vollständige Setup-Anleitung
- Die Anwendung zeigt hilfreiche Fehlermeldungen mit Lösungen
