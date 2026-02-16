# MEWS Reservierungsbericht Export - Anleitung

## Wichtig: Korrekte Export-Einstellungen

Damit alle Listen korrekt generiert werden können, muss der MEWS Reservierungsbericht mit den richtigen Einstellungen exportiert werden.

## Schritt-für-Schritt-Anleitung

### 1. In MEWS anmelden
- Öffnen Sie MEWS: https://app.mews.com
- Melden Sie sich mit Ihren Zugangsdaten an

### 2. Zum Reservierungsbericht navigieren
- Klicken Sie auf **"Berichte"** (Reports) im Hauptmenü
- Wählen Sie **"Reservierungsbericht"** (Reservation Report)

### 3. Zeitraum einstellen
Wählen Sie den gewünschten Zeitraum:
- **Empfehlung:** Mindestens 14 Tage ab heute
- Stellen Sie sicher, dass alle relevanten Reservierungen enthalten sind

### 4. Ansicht auf "Detailliert" umschalten
⚠️ **Sehr wichtig!**
- Klicken Sie auf **"Detaillierte Ansicht"** oder **"Detailed View"**
- Nur in dieser Ansicht sind alle benötigten Daten verfügbar

### 5. Spalten aktivieren
Stellen Sie sicher, dass folgende Spalten sichtbar sind:

#### Pflichtfelder (müssen vorhanden sein):
- ✅ Reservierungsnummer
- ✅ Gastname
- ✅ Anreise (Check-in)
- ✅ Abreise (Check-out)
- ✅ Zimmerkategorie
- ✅ Raumnummer
- ✅ Rate / Ratenname
- ✅ RateCode
- ✅ Anzahl Erwachsene
- ✅ Anzahl Kinder
- ✅ Status

#### Wichtige optionale Felder:
- ✅ **Produkte** (Products) - **SEHR WICHTIG!**
- ✅ **Gästenotizen** (Guest Notes)
- ✅ **Reservierungsnotizen** (Reservation Notes)
- ✅ **Channel-Notizen** (Channel Notes)
- ✅ Firma / Company
- ✅ Channel / Kanal
- ✅ Preis / Total

### 6. Produkte einblenden
⚠️ **KRITISCH für korrekte Listen-Generierung!**

So aktivieren Sie Produkte:
1. Klicken Sie auf das **"Spalten"-Symbol** (Column Settings)
2. Scrollen Sie zu **"Produkte"** oder **"Products"**
3. Aktivieren Sie das Kontrollkästchen
4. Stellen Sie sicher, dass folgende Produkte sichtbar sind:
   - Frühstück Speisen
   - Frühstück Getränke
   - HP inkl.
   - Flasche Sekt
   - Obstkorb
   - Zimmerdeko
   - Entspannungspaket
   - Kissalis Gutschein
   - Rückenmassage
   - Freigetränk

### 7. Notizen einblenden
⚠️ **Wichtig für Sonderwünsche!**

Aktivieren Sie alle Notiz-Typen:
1. **Gästenotizen** - Enthält Wünsche und Anmerkungen des Gastes
2. **Reservierungsnotizen** - Interne Notizen zur Reservierung
3. **Channel-Notizen** - Notizen von Buchungsportalen

### 8. Filter prüfen
- Entfernen Sie Filter, die Reservierungen ausschließen könnten
- Status sollte mindestens enthalten:
  - ✅ Confirmed
  - ✅ Checked-in
  - Optional auch: Optional, Cancelled (je nach Bedarf)

### 9. Export starten
1. Klicken Sie auf **"Export"** oder **"Exportieren"**
2. Wählen Sie **"Excel"** als Format
3. Der Download startet automatisch

### 10. Datei überprüfen
Öffnen Sie die heruntergeladene Excel-Datei kurz und prüfen Sie:
- ✅ Produkte sind in separaten Zeilen oder Spalten sichtbar
- ✅ Notizen sind ausgefüllt (wo vorhanden)
- ✅ Alle Reservierungen sind enthalten

## Häufige Fehler und Lösungen

### Problem: Keine Housekeeping-Aufgaben werden generiert
**Ursache:** Produkte sind nicht im Export enthalten
**Lösung:** Produkt-Spalte aktivieren (siehe Schritt 6)

### Problem: Keine Front Office-Aufgaben
**Ursache:** Produkte oder Rateninfos fehlen
**Lösung:** 
- Produkt-Spalte aktivieren
- Rate/RateCode-Spalten prüfen

### Problem: Frühstückszahlen sind 0
**Ursache:** Frühstück-Produkte fehlen
**Lösung:** 
- Stellen Sie sicher, dass "Frühstück Speisen" und "Frühstück Getränke" als Produkte exportiert werden
- In MEWS: Prüfen Sie, ob Frühstück in den Rates korrekt hinterlegt ist

### Problem: Sonderwünsche werden nicht erkannt
**Ursache:** Notizen-Spalten fehlen
**Lösung:** Alle drei Notiz-Typen aktivieren (siehe Schritt 7)

### Problem: Falsche Belegungszahlen
**Ursache:** Zeitraum falsch gewählt oder Status-Filter zu restriktiv
**Lösung:**
- Zeitraum erweitern
- Status-Filter überprüfen

## Export-Template speichern (Empfohlen)

Um diese Einstellungen nicht jedes Mal neu vornehmen zu müssen:

1. Stellen Sie alle Einstellungen wie oben beschrieben ein
2. Klicken Sie auf **"Ansicht speichern"** oder **"Save View"**
3. Benennen Sie die Ansicht z.B. "Tagesplan Export"
4. Beim nächsten Export wählen Sie einfach diese gespeicherte Ansicht

## Checkliste vor dem Export

✅ Detaillierte Ansicht aktiviert  
✅ Produkte-Spalte sichtbar  
✅ Alle Notiz-Spalten aktiviert  
✅ Zeitraum korrekt (min. 14 Tage)  
✅ Status-Filter geprüft  
✅ Alle Pflichtfelder vorhanden  

## MEWS-Versionen

Diese Anleitung gilt für MEWS Commander (aktuelle Version).
Bei älteren Versionen können die Menü-Bezeichnungen leicht abweichen.

## Excel-Export Struktur

Der MEWS-Export enthält mehrere Arbeitsblätter (Tabs). Die Anwendung erkennt automatisch das Hauptblatt mit den Reservierungsdaten, indem sie nach folgenden Kriterien sucht:
- Blätter mit Namen wie "Reservierungen", "Reservations", "Maarming", "Buchungen"
- Blätter mit typischen Spalten wie "Gastname", "Anreise", "Abreise", "Zimmernummer"

## Generierte Listen

Die Anwendung erstellt folgende Excel-Arbeitsblätter (in dieser Reihenfolge):

1. **Belegung** - Übersicht der täglichen Zimmerbelegung
2. **Frühstück** - Frühstückszahlen pro Tag
3. **Hauswirtschaft** - Housekeeping-Aufgaben (Reinigung, Dekoration, etc.)
4. **Vorbereiten** - Front Office-Aufgaben (Check-in Vorbereitungen)
5. **Ankünfte** - Detaillierte Liste der heutigen Anreisen

## Support

Bei Problemen mit dem MEWS-Export wenden Sie sich bitte an:
- Ihren MEWS-Administrator
- MEWS Support: support@mews.com

---

**Tipp:** Machen Sie einen Screenshot Ihrer korrekten Export-Einstellungen für zukünftige Referenz!
