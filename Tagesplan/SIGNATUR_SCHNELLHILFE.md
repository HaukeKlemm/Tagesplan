# ⚡ SCHNELLSTE LÖSUNG - Execution Policy Problem

## Problem
```
Die Datei "playwright.ps1" ist nicht digital signiert.
```

---

## ✅ Lösung 1: Policy ändern (EMPFOHLEN - 10 Sekunden)

**PowerShell als Administrator öffnen und ausführen:**

```powershell
Set-ExecutionPolicy RemoteSigned -Scope CurrentUser
```

**Fertig!** Jetzt funktionieren alle Skripte.

---

## ✅ Lösung 2: Skripte signieren (einmalig 2 Minuten)

1. **PowerShell als Administrator öffnen**
2. **Zum Projektordner navigieren:**
   ```powershell
   cd "C:\Pfad\Zu\Tagesplan"
   ```
3. **Signatur-Skript ausführen:**
   ```powershell
   .\sign-scripts.ps1
   ```

**Das war's!** Alle Skripte sind jetzt signiert.

---

## ✅ Lösung 3: Bypass bei jedem Befehl

**Für setup.ps1:**
```powershell
powershell -ExecutionPolicy Bypass -File .\setup.ps1
```

**Für fix-playwright.ps1:**
```powershell
powershell -ExecutionPolicy Bypass -File .\fix-playwright.ps1
```

---

## Welche Dateien müssen signiert werden?

Diese 3 Dateien:
1. ✅ **setup.ps1** (im Projektroot)
2. ✅ **fix-playwright.ps1** (im Projektroot)  
3. ✅ **playwright.ps1** (in bin\Debug\net8.0-windows\)

Das `sign-scripts.ps1` Skript findet und signiert alle automatisch!

---

## Meine Empfehlung

**Am einfachsten:**
```powershell
# Als Administrator ausführen:
Set-ExecutionPolicy RemoteSigned -Scope CurrentUser
```

Dauert 10 Sekunden, funktioniert für immer! ✨

---

## Überprüfung

**Nach Policy-Änderung:**
```powershell
Get-ExecutionPolicy
# Sollte zeigen: RemoteSigned
```

**Nach Signieren:**
```powershell
Get-AuthenticodeSignature .\setup.ps1
# Sollte zeigen: Status = Valid
```

---

## Bei Fragen

- `SIGNIEREN_ANLEITUNG.md` - Detaillierte Anleitung
- `EXECUTION_POLICY_GUIDE.md` - Hintergrundinfos
- `TROUBLESHOOTING.md` - Weitere Probleme
