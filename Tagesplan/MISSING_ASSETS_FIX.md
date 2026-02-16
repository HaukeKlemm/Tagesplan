# ⚠️ "missing required assets" Fehler

## Fehlermeldung
```
Microsoft.Playwright assembly was found, but is missing required assets.
Please ensure to build your project before running Playwright tool.
```

---

## 🎯 SCHNELLSTE LÖSUNG für Ihren Rechner

Sie sind hier: `C:\Users\offic\Desktop\Tagesplan\Tagesplan-main\Tagesplan`

### Option A: Quick-Fix Skript (1 Befehl!)

```powershell
.\quick-fix.ps1
```

**Macht automatisch:**
- ✅ Navigiert zum richtigen Ordner
- ✅ Clean + Restore + Build
- ✅ Playwright installieren
- ✅ Anwendung starten

**FERTIG in 2-3 Minuten!** ⚡

---

### Option B: Manuell (5 Befehle)

```powershell
cd "C:\Users\offic\Desktop\Tagesplan\Tagesplan-main\Tagesplan"
dotnet clean
dotnet restore
dotnet build
pwsh -ExecutionPolicy Bypass bin\Debug\net8.0-windows\playwright.ps1 install chromium
.\bin\Debug\net8.0-windows\Tagesplan.exe
```

---

## 🔧 Für andere Rechner

### Option 1: Automatisches Skript (EMPFOHLEN)

1. **Navigieren Sie zum Projekt-Ordner** (wo .csproj oder .sln ist)
2. **Öffnen Sie PowerShell**
3. **Ausführen:**
   ```powershell
   .\rebuild-and-install-playwright.ps1
   ```

Das Skript findet automatisch .sln oder .csproj!

---

### Option 2: Manuell in Visual Studio

1. **Schließen Sie die laufende Anwendung** (ALT+F4)
2. **In Visual Studio:**
   - Build → Projektmappe bereinigen
   - Build → Projektmappe neu erstellen (STRG+SHIFT+B)
3. **Warten bis Build fertig**
4. **PowerShell öffnen im Projektordner:**
   ```powershell
   pwsh -ExecutionPolicy Bypass bin\Debug\net8.0-windows\playwright.ps1 install chromium
   ```
5. **Anwendung neu starten** (F5)

---

### Option 3: Manuell in PowerShell

```powershell
# 1. Zum Projektordner navigieren
cd "C:\Users\offic\Desktop\Tagesplan\Tagesplan-main\Tagesplan"

# 2. Clean
dotnet clean

# 3. Restore
dotnet restore

# 4. Build
dotnet build

# 5. Playwright installieren
pwsh -ExecutionPolicy Bypass bin\Debug\net8.0-windows\playwright.ps1 install chromium

# 6. Anwendung starten
.\bin\Debug\net8.0-windows\Tagesplan.exe
```

---

## ⚡ Schnellste Lösung

```powershell
# Im Projektordner ausführen:
.\rebuild-and-install-playwright.ps1
```

Dauert 2-3 Minuten, macht alles automatisch! ✨

---

## Warum passiert das?

**Problem:** Die Anwendung wurde mit F5 (Debugging) gestartet, aber Playwright benötigt einen vollständigen Build mit allen Assets.

**Unterschied:**
- **F5 (Debug)**: Schneller Start, minimaler Build
- **Full Build**: Kopiert alle Dependencies, Playwright-Skripte, etc.

**Lösung:** Vollständiger Rebuild behebt das Problem.

---

## Nach dem Fix

1. **Anwendung neu starten**
2. **Klick auf "1. MEWS Login"**
3. **Browser sollte sich öffnen** ✓

Jetzt funktioniert alles! 🎉

---

## Falls es immer noch nicht funktioniert

### Überprüfen Sie:

**1. Ist das Projekt gebaut?**
```powershell
Test-Path "bin\Debug\net8.0-windows\Tagesplan.exe"
Test-Path "bin\Debug\net8.0-windows\playwright.ps1"
# Beide sollten True zeigen
```

**2. Ist PowerShell Core installiert?**
```powershell
pwsh --version
# Sollte Version anzeigen
```

**3. Build-Fehler?**
```powershell
dotnet build > build-log.txt 2>&1
# Öffne build-log.txt und suche nach Fehlern
```

---

## Weitere Hilfe

- `PLAYWRIGHT_BUILD_GUIDE.md` - Detaillierte Build-Anleitung
- `TROUBLESHOOTING.md` - Alle Probleme & Lösungen
- `INSTALLATION.md` - Komplette Installation

---

## Zusammenfassung

| Problem | Lösung | Dauer |
|---------|--------|-------|
| Missing required assets | `.\rebuild-and-install-playwright.ps1` | 2-3 Min |
| Manuell in VS | Clean → Rebuild → playwright.ps1 install | 3-5 Min |
| PowerShell | dotnet clean, build, pwsh playwright.ps1 | 2-3 Min |

**Am einfachsten: Das rebuild-Skript verwenden!** ⚡
