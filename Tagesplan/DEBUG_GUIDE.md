# Debug-Anleitung für leere Excel-Listen

## Problem
Die generierten Excel-Listen sind leer, obwohl die MEWS-Excel-Datei Daten enthält.

## Debug-Schritte

### 1. Output-Fenster öffnen
1. Starten Sie die Anwendung im Debug-Modus (F5)
2. Öffnen Sie das **Output-Fenster** in Visual Studio:
   - Menü: **Ansicht → Ausgabe** (oder `Ctrl+Alt+O`)
   - Wählen Sie in der Dropdown-Liste: **Debug**

### 2. Excel-Datei verarbeiten
1. Wählen Sie Ihre MEWS-Excel-Datei aus
2. Klicken Sie auf "Listen generieren"
3. Beobachten Sie das Output-Fenster

### 3. Was Sie im Output-Fenster sehen sollten

#### A) Worksheet-Erkennung
```
Using worksheet: [Name des Blattes]
Found header row at: [Zeilennummer] with [Anzahl] indicators
```
✅ **Gut:** Worksheet und Header wurden gefunden
❌ **Problem:** "No header row found" → Header wird nicht erkannt

#### B) Spalten-Mapping
```
=== Column Map for worksheet '[Name]' ===
Found X columns:
  GuestName -> Column 3
  CheckIn -> Column 5
  CheckOut -> Column 6
  RoomNumber -> Column 7
  Rate -> Column 10
  Products -> Column 15
====================================
```
✅ **Gut:** Alle wichtigen Spalten gefunden (GuestName, CheckIn, Products)
❌ **Problem:** "Products" fehlt → Produkte werden nicht gelesen

#### C) Geparste Reservierungen
```
Reservation Row 2:
  Guest: [Name]
  Room: [Zimmernummer]
  Check-in: 2024-01-15
  Check-out: 2024-01-17
  Rate: [Ratenname]
  Products: Frühstück Speisen; Obstkorb
  Parsed Products: 2
    - Frühstück Speisen (Qty: 1)
    - Obstkorb (Qty: 1)
```
✅ **Gut:** Reservierungen mit Produkten werden geparst
❌ **Problem:** "Products: " (leer) → Keine Produkte in der Excel-Datei
❌ **Problem:** "Parsed Products: 0" → Produkte werden nicht erkannt

#### D) Generierte Tasks
```
=== GenerateHousekeepingTasks ===
Processing 25 reservations
Checking reservation: [Name] (Room 101), Products: 2
  Product: 'Obstkorb'
Generated 15 housekeeping tasks
================================
```
✅ **Gut:** Tasks werden generiert
❌ **Problem:** "Generated 0 tasks" → Keine Products oder falsche Produkt-Namen

## Häufige Probleme und Lösungen

### Problem 1: "Could not find header row"
**Ursache:** Der Header in der Excel-Datei wird nicht erkannt

**Lösung:**
1. Öffnen Sie die MEWS-Excel-Datei
2. Prüfen Sie, in welcher Zeile die Spalten-Überschriften stehen
3. Die erste Zeile sollte Begriffe wie "Gastname", "Anreise", "Abreise" enthalten
4. Wenn der Header in Zeile 30+ ist, müssen wir den Code anpassen

### Problem 2: Spalte "Rate" oder "RateCode" wird nicht gefunden
**WICHTIG:** Die Anwendung verwendet Rate-Definitionen, um Produkte zu bestimmen!

**Ursache:** Die Rate-Spalte fehlt oder hat einen anderen Namen

**Lösung:**
1. Öffnen Sie die MEWS-Excel-Datei
2. Suchen Sie nach einer Spalte mit Namen wie:
   - "Rate" / "Ratenname"
   - "RateCode" / "Tarifcode"
3. Diese Spalte ist ESSENTIELL, da alle Produkte aus den Rate-Definitionen kommen

### Problem 3: Listen sind leer obwohl Daten vorhanden sind
**Ursache:** Rate-Namen stimmen nicht mit den vordefinierten Raten überein

**Vordefinierte Raten:**
- **ReisenAktuell** → Frühstück, Halbpension, Freigetränk
- **Kleine Auszeit** → Frühstück, 3-Gang-Menü, Sekt, Obstkorb, KissSalis
- **RomantikSpezial** → Frühstück, Candlelight Dinner, Deko, Sekt, Obstkorb, Massage
- **Kissalis Genießen - 2 Nächte** → Frühstück, HP, Entspannungspaket, KissSalis Gutschein
- **Kissalis Genießen - 3 Nächte** → Frühstück, HP, Entspannungspaket, 2x KissSalis Gutschein

**Lösung:**
1. Prüfen Sie im Output-Fenster, welche Rate-Namen tatsächlich in der Excel-Datei stehen
2. Beispiel-Output: "Checking reservation: Max Mustermann (Room 101), Rate: ReisenAktuell 2024"
3. Wenn die Rate-Namen abweichen, müssen diese in `Helpers/RateMapper.cs` hinzugefügt werden

### Problem 4: Produkte sind leer (aber sollten aus Rate kommen)
**Ursache:** Das ist NORMAL! Die Produkte werden aus den Rate-Definitionen bestimmt

**Hinweis:**
- Die "Products"-Spalte in MEWS kann leer sein oder nur Extras enthalten
- Die Haupt-Produkte (Frühstück, HP, etc.) werden aus der Rate abgeleitet
- Prüfen Sie im Output-Fenster:
  ```
  Rate 'ReisenAktuell' includes: Frühstück Speisen, Frühstück Getränke, HP inkl., Freigetränk
  ```

### Problem 5: Tasks werden nicht generiert (0 tasks)
**Ursache:** Die Produktnamen in den Rate-Definitionen stimmen nicht mit der Erkennungslogik überein

**Erkannte Produkt-Keywords:**
- **Housekeeping:** "sekt", "obstkorb", "deko", "dekoration", "entspannung"
- **Breakfast:** "frühstück", "breakfast"
- **Front Office:** "hp inkl", "halbpension", "kissalis", "massage", "freigetränk"

**Lösung:**
1. Prüfen Sie im Output-Fenster die Debug-Meldungen:
   ```
   Checking product: 'Frühstück Speisen'
   → Added Sekt task
   ```
2. Wenn Tasks fehlen, prüfen Sie, ob die Keywords in den Produktnamen vorkommen

### Problem 5: Datum-Filter
**Ursache:** Nur zukünftige Reservierungen werden verarbeitet

**Hinweis:** 
- Housekeeping-Tasks werden nur für `CheckIn >= Heute` erstellt
- Breakfast wird für alle Daten im Zeitraum erstellt
- Prüfen Sie, ob Ihre Reservierungen das richtige Check-in-Datum haben

## Nächste Schritte

1. ✅ Starten Sie die Anwendung im Debug-Modus
2. ✅ Öffnen Sie das Output-Fenster (Ansicht → Ausgabe)
3. ✅ Verarbeiten Sie Ihre Excel-Datei
4. ✅ Kopieren Sie die Debug-Ausgaben
5. ✅ Teilen Sie mir die Ausgaben mit, damit ich das Problem identifizieren kann

## Beispiel-Ausgabe kopieren

Um mir zu helfen, kopieren Sie bitte folgende Abschnitte aus dem Output-Fenster:

```
Using worksheet: ...
Found header row at: ...

=== Column Map ===
[gesamte Spalten-Liste]

Reservation Row 2:
[erste paar Reservierungen]

=== GenerateHousekeepingTasks ===
[Zusammenfassung]
```

Mit diesen Informationen kann ich genau sehen, wo das Problem liegt!
