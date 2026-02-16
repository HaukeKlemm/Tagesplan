# Rate-basierte Produkterkennung - Änderungsprotokoll

## Übersicht
Das System wurde von produkt-basierter auf **rate-basierte** Erkennung umgestellt, entsprechend den ursprünglichen Anforderungen.

## Konzept

### ❌ Alter Ansatz (falsch)
- Produkte wurden nur aus der "Products"-Spalte in der MEWS-Excel-Datei gelesen
- Wenn die Spalte leer war oder nicht exportiert wurde → keine Tasks generiert
- **Problem:** MEWS exportiert oft nicht alle inkludierten Leistungen als "Produkte"

### ✅ Neuer Ansatz (korrekt)
- Jede Rate hat **vordefinierte Produkte/Leistungen** (in `Helpers/RateMapper.cs`)
- System prüft zuerst die Rate-Definition
- Zusätzliche Produkte aus der Excel-Spalte werden als **Extras** hinzugefügt
- **Vorteil:** Funktioniert auch wenn MEWS-Export keine Produkte enthält

## Geänderte Dateien

### 1. `Services/BreakfastGenerator.cs`
**Änderung:** `HasBreakfast()` prüft jetzt zuerst die Rate-Definition
```csharp
// Vorher: Nur Products-Spalte
return reservation.Products.Any(p => p.Name.Contains("frühstück"));

// Nachher: Rate-Definition + Products
var rateDefinition = RateMapper.GetRateDefinition(reservation.Rate);
if (rateDefinition != null && rate includes breakfast) return true;
// Fallback auf Products-Spalte
```

### 2. `Services/TaskGenerator.cs`
**Änderungen:**
- `GenerateHousekeepingTasks()`: Verwendet Rate-Definition + Products
- `GenerateFrontOfficeTasks()`: Verwendet Rate-Definition + Products
- Erweiterte Debug-Ausgaben zeigen Rate-Informationen
- Automatische Berechnung von KissSalis-Gutscheinen basierend auf Rate

**Neue Logik:**
```csharp
// 1. Lade Rate-Definition
var rateDefinition = RateMapper.GetRateDefinition(reservation.Rate);

// 2. Kombiniere Produkte aus Rate + Reservation
var productsToCheck = new List<string>();
productsToCheck.AddRange(rateDefinition.IncludedProducts);  // Aus Rate
productsToCheck.AddRange(reservation.Products);              // Aus Excel (Extras)

// 3. Generiere Tasks basierend auf allen Produkten
foreach (var product in productsToCheck) { ... }
```

### 3. `DEBUG_GUIDE.md`
**Aktualisiert mit:**
- Erklärung des Rate-basierten Systems
- Liste der vordefinierten Raten
- Neue Problemlösungen für Rate-Mapping

## Rate-Definitionen

Die folgenden Raten sind in `Helpers/RateMapper.cs` definiert:

### 1. ReisenAktuell
```
Inkludiert:
- Frühstück Speisen
- Frühstück Getränke
- HP inkl.
- Freigetränk
```

### 2. Kleine Auszeit
```
Inkludiert:
- Frühstück Speisen
- Frühstück Getränke
- Flasche Sekt
- Obstkorb
- Kissalis Gutschein
```

### 3. RomantikSpezial
```
Inkludiert:
- Frühstück Speisen
- Frühstück Getränke
- Flasche Sekt
- Obstkorb
- Zimmerdeko
- Rückenmassage
```

### 4. Kissalis Genießen - 2 Nächte
```
Inkludiert:
- Frühstück Speisen
- Frühstück Getränke
- HP inkl.
- Entspannungspaket
- Kissalis Gutschein (1x 2h pro Person)
```

### 5. Kissalis Genießen - 3 Nächte
```
Inkludiert:
- Frühstück Speisen
- Frühstück Getränke
- HP inkl.
- Entspannungspaket
- Kissalis Gutschein (2x 2h pro Person)
```

## Task-Generierung

### Housekeeping Tasks
Generiert für **zukünftige** Anreisen (CheckIn >= Heute):
- ✅ Sekt → wenn Rate/Products "sekt" enthält
- ✅ Obstkorb → wenn Rate/Products "obstkorb" enthält
- ✅ Zimmerdeko → wenn Rate/Products "deko"/"dekoration" enthält
- ✅ Entspannungspaket → wenn Rate/Products "entspannung" enthält

### Front Office Tasks
Generiert für alle Aufenthalte:
- ✅ Halbpension → wenn Rate/Products "HP inkl"/"halbpension" enthält (täglich)
- ✅ Candlelight Dinner → wenn Rate "romantik" enthält
- ✅ 3-Gang-Abendmenü → wenn Rate "kleine auszeit" enthält (täglich)
- ✅ KissSalis Gutscheine → wenn Rate/Products "kissalis" enthält
  - 1x pro Person für "2 Nächte"
  - 2x pro Person für "3 Nächte"
- ✅ Massage → wenn Rate/Products "massage" enthält
- ✅ Freigetränk → wenn Rate/Products "freigetränk" enthält (täglich)

### Breakfast Overview
Zeigt alle Tage mit Frühstück:
- ✅ Prüft zuerst Rate-Definition
- ✅ Fallback auf Products-Spalte
- ✅ Gruppiert nach Erwachsene/Kinder
- ✅ Zeigt Zimmernummern

## Debug-Ausgaben

Das System gibt jetzt detaillierte Informationen aus:

```
=== GenerateHousekeepingTasks ===
Processing 25 reservations
Checking reservation: Max Mustermann (Room 101), Rate: ReisenAktuell 2024
  Rate 'ReisenAktuell' includes: Frühstück Speisen, Frühstück Getränke, HP inkl., Freigetränk
  Checking product: 'Frühstück Speisen'
  Checking product: 'HP inkl.'
Generated 15 housekeeping tasks
```

## Vorteile

1. ✅ **Robustheit:** Funktioniert auch ohne Products-Spalte in MEWS
2. ✅ **Konsistenz:** Alle Leistungen einer Rate werden garantiert erkannt
3. ✅ **Flexibilität:** Zusätzliche Produkte aus Excel werden als Extras hinzugefügt
4. ✅ **Wartbarkeit:** Rate-Definitionen sind zentral in einer Datei
5. ✅ **Transparenz:** Debug-Ausgaben zeigen genau, was erkannt wurde

## Testen

Um das neue System zu testen:

1. Starten Sie die Anwendung (F5)
2. Öffnen Sie Output-Fenster (Ansicht → Ausgabe → Debug)
3. Laden Sie eine MEWS-Excel-Datei
4. Prüfen Sie die Debug-Ausgaben:
   - Werden die Raten erkannt?
   - Werden die inkludierten Produkte aufgelistet?
   - Werden Tasks generiert?

## Nächste Schritte

Falls Listen immer noch leer sind:
1. Prüfen Sie die **Rate-Namen** in Ihrer Excel-Datei
2. Vergleichen Sie mit den vordefinierten Raten in `RateMapper.cs`
3. Bei Abweichungen müssen die Rate-Namen in `RateMapper.cs` angepasst werden
4. Teilen Sie die Debug-Ausgaben mit, um das Problem zu identifizieren
