# MEWS Tagesplan Generator

Eine vollautomatische Windows-Anwendung zur Generierung von Hotellisten aus MEWS-Reservierungsberichten.

## Funktionen

### 1. MEWS-Integration
- Automatischer Login via Playwright (manuell durch Benutzer)
- Download des Reservierungsberichts direkt aus MEWS
- Unterstützung für detaillierte Berichte mit Produkten und Notizen

### 2. Generierte Listen

#### Housekeeping-Aufgaben
Automatisch generierte Aufgaben basierend auf Produkten:
- **Sekt** - Flasche Sekt im Zimmer bereitstellen
- **Obstkorb** - Obstkorb im Zimmer bereitstellen
- **Zimmerdeko** - Zimmer dekorieren (Romantik)
- **Entspannungspaket** - Entspannungspaket bereitstellen

Farb-Codierung:
- Präsente (Sekt, Obstkorb, Deko) → Gold
- Wellness/Entspannung → Blau

#### Front Office-Aufgaben
Basierend auf Ratenpaketen und Produkten:
- **Halbpension** - Täglich für gesamten Aufenthalt
- **Candlelight Dinner** - Reservierung und Information an Restaurant
- **KissSalis Gutscheine** - Gutscheine bereitstellen (pro Person)
- **Massage** - Terminvereinbarung mit Gast
- **Freigetränke** - Pro Person und Abend

Farb-Codierung:
- HP/Dinner → Rot
- Wellness/Massage → Blau
- KissSalis → Türkis

#### Frühstücks-Übersicht (14 Tage)
- Tägliche Frühstückszahlen
- Aufschlüsselung: Erwachsene / Kinder
- Zimmernummern-Liste
- Wochenenden hervorgehoben

#### Belegungs-Übersicht (7 Tage)
- Belegte Zimmer pro Tag
- Personenanzahl (Erwachsene/Kinder)
- Ratencodes mit Anzahl
- Wochenenden hervorgehoben

#### Anreisen-Liste (Heute)
- Alle Gäste mit Anreise heute
- Zimmernummer und Personenanzahl
- **Automatische Erkennung von Sonderwünschen:**
  - Glutenfrei, laktosefrei, vegan, vegetarisch
  - Allergie
  - Parkplatz, Garage
  - Hund, Haustier
  - Firmenrechnung
  - Früher Check-in, später Check-out
  - Geburtstag, Hochzeitstag, Jubiläum
  - Baby, Kinderbett
  - Rollstuhl, barrierefrei
  - Nichtraucher
- Alle Notizen vollständig angezeigt

## Ratendefinitionen

### ReisenAktuell
**Inkludiert:**
- Frühstück
- Halbpension
- Freigetränk pro Person und Abend

**Farbe:** Orange

### Kleine Auszeit
**Inkludiert:**
- Frühstück
- 3-Gang-Abendmenü
- Flasche Sekt
- Obstkorb
- KissSalis Gutschein (2h)

**Farbe:** Rosa

### RomantikSpezial
**Inkludiert:**
- Frühstück
- Candlelight Dinner
- Zimmerdekoration
- Flasche Sekt
- Obstkorb
- Rückenmassage pro Person

**Farbe:** Rot

### Kissalis Genießen - 2 Nächte
**Inkludiert:**
- Frühstück
- Halbpension
- Entspannungspaket
- 2-Stunden-Gutschein pro Person

**Farbe:** Grün

### Kissalis Genießen - 3 Nächte
**Inkludiert:**
- Frühstück
- Halbpension
- Entspannungspaket
- Zwei 2-Stunden-Gutscheine pro Person

**Farbe:** Grün

## Verwendung

### Schritt 1: MEWS Login
1. Klicken Sie auf **"1. MEWS Login"**
2. Ein Chrome-Browser wird geöffnet
3. Melden Sie sich manuell in MEWS an
4. Navigieren Sie zu Ihrem Dashboard

### Schritt 2: Bericht herunterladen
1. Klicken Sie auf **"2. Bericht herunterladen"**
2. Navigieren Sie in MEWS zu: **Berichte → Reservierungsbericht**
3. Stellen Sie sicher:
   - ✓ Detaillierte Ansicht ist aktiviert
   - ✓ Produkte sind sichtbar
   - ✓ Notizen sind sichtbar (Gästenotizen, Reservierungsnotizen, Channel-Notizen)
4. Klicken Sie auf **"Export"** und wählen Sie **Excel**
5. Die Anwendung erkennt den Download automatisch

### Schritt 3: Listen generieren
1. Klicken Sie auf **"3. Listen generieren"**
2. Die Anwendung verarbeitet die Excel-Datei
3. Alle Listen werden in einer neuen Excel-Datei gespeichert
4. Die Datei wird automatisch im Dokumentenordner gespeichert

### Alternative: Manuelle Dateiauswahl
- Klicken Sie auf **"Datei wählen..."** um eine bereits heruntergeladene MEWS-Datei zu verwenden
- Nützlich wenn Sie den Bericht bereits manuell exportiert haben

## Systemanforderungen

- Windows 10/11
- .NET 8.0
- Microsoft Excel (zum Öffnen der generierten Listen)
- Chrome Browser (für MEWS-Automation)

## Erste Verwendung

Beim ersten Start der MEWS-Automation muss Playwright installiert werden:

```powershell
# Im Projektverzeichnis ausführen:
pwsh bin/Debug/net8.0-windows/playwright.ps1 install chromium
```

Alternativ wird beim ersten Login automatisch gefragt, ob Playwright installiert werden soll.

## Output-Dateien

Alle generierten Dateien werden gespeichert in:
```
C:\Users\[IhrName]\Documents\MEWS_Downloads\
```

Dateiformat:
```
MEWS_Listen_20240115_143022.xlsx
```

## Excel-Struktur der generierten Datei

Die Ausgabe-Datei enthält folgende Arbeitsblätter:

1. **Housekeeping** - Aufgaben für Housekeeping
2. **Front Office** - Aufgaben für Front Office
3. **Frühstück** - 14-Tage-Übersicht
4. **Belegung** - 7-Tage-Übersicht
5. **Anreisen** - Heutige Anreisen mit Sonderwünschen

## Produktdefinitionen (aus MEWS)

Die folgenden Produkte werden automatisch erkannt:
- Entspannungspaket
- Flasche Sekt
- Freigetränk
- Frühstück Getränke
- Frühstück Speisen
- HP inkl.
- Kissalis Gutschein
- Obstkorb
- Rückenmassage
- Zimmerdeko

## Farb-Legende

### Task-Farben
- 🟡 **Gold** - Präsente (Sekt, Obstkorb, Deko)
- 🔴 **Rot** - Halbpension / Dinner
- 🔵 **Blau** - Wellness / Massage
- 🩵 **Türkis** - KissSalis
- 🟣 **Lila** - Sonderwünsche

### Raten-Farben
- 🟠 **Orange** - ReisenAktuell
- 🩷 **Rosa** - Kleine Auszeit
- 🔴 **Rot** - RomantikSpezial
- 🟢 **Grün** - Kissalis Genießen
- ⚫ **Grau** - Standard
- 🔵 **Blau** - OTA (Booking, Expedia, etc.)

## Fehlerbehandlung

Die Anwendung ist robust konzipiert:
- Fehlerhafte Zeilen im Excel werden übersprungen
- Detaillierte Statusmeldungen für jeden Schritt
- Automatisches Logging in der Status-Box
- Fehlende Daten werden durch Standardwerte ersetzt

## Technische Details

### Verwendete Bibliotheken
- **ClosedXML** - Excel-Parsing und -Generierung
- **Microsoft.Playwright** - Browser-Automation

### Architektur
- **Models** - Datenmodelle (Reservation, Product, Tasks, etc.)
- **Services** - Business-Logik (Parser, Generatoren, Exporter)
- **Helpers** - Hilfsfunktionen (Farben, Rate-Mapping)

### Datenfluss
1. MEWS Excel → Parser → Reservation-Objekte
2. Reservations → TaskGenerator → Housekeeping/FO-Tasks
3. Reservations → BreakfastGenerator → Frühstücks-Übersicht
4. Reservations → OccupancyGenerator → Belegungs-Übersicht
5. Reservations → ArrivalGenerator → Anreisen-Liste
6. Alle Listen → ExcelExporter → Output-Excel

## Erweiterungsmöglichkeiten

Die Anwendung ist modular aufgebaut und kann leicht erweitert werden:
- Neue Ratendefinitionen in `Helpers/RateMapper.cs`
- Zusätzliche Task-Typen in `Services/TaskGenerator.cs`
- Neue Listen-Typen durch neue Generator-Services
- Anpassbare Farben in `Helpers/ColorHelper.cs`

## Support & Anpassungen

Bei Fragen oder Anpassungswünschen kontaktieren Sie bitte Ihren Administrator.

---

**Version:** 1.0  
**Erstellt:** Januar 2024  
**Framework:** .NET 8.0 WinForms
