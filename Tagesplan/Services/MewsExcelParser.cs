using ClosedXML.Excel;
using Tagesplan.Models;
using System.Globalization;

namespace Tagesplan.Services
{
    public class MewsExcelParser
    {
        public List<Reservation> ParseReservations(string filePath, Action<string>? statusCallback = null)
        {
            var reservations = new List<Reservation>();

            System.Diagnostics.Debug.WriteLine($"\n=== ParseReservations ===");
            System.Diagnostics.Debug.WriteLine($"File: {filePath}");
            statusCallback?.Invoke("  Öffne Excel-Datei...");

            using var workbook = new XLWorkbook(filePath);

            System.Diagnostics.Debug.WriteLine($"Total worksheets: {workbook.Worksheets.Count}");
            statusCallback?.Invoke($"  Gefundene Arbeitsblätter: {workbook.Worksheets.Count}");

            foreach (var ws in workbook.Worksheets)
            {
                System.Diagnostics.Debug.WriteLine($"  - {ws.Name} (rows: {ws.RowsUsed().Count()})");
                statusCallback?.Invoke($"    • {ws.Name} ({ws.RowsUsed().Count()} Zeilen)");
            }

            // Find the correct worksheet with reservation data
            var worksheet = FindReservationWorksheet(workbook);
            if (worksheet == null)
            {
                var sheetNames = string.Join(", ", workbook.Worksheets.Select(ws => ws.Name));
                throw new Exception($"Could not find reservation data worksheet in Excel file.\n\nAvailable sheets: {sheetNames}\n\nPlease check that the MEWS export contains a sheet with reservation data.");
            }

            System.Diagnostics.Debug.WriteLine($"Using worksheet: {worksheet.Name}");
            statusCallback?.Invoke($"  Verwende Arbeitsblatt: {worksheet.Name}");

            // Find header row
            var headerRow = FindHeaderRow(worksheet);
            if (headerRow == null)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR: Could not find header row!");
                statusCallback?.Invoke($"  ✗ FEHLER: Keine Header-Zeile gefunden!");
                statusCallback?.Invoke($"  Erste 10 Zeilen von '{worksheet.Name}':");

                for (int i = 1; i <= Math.Min(10, worksheet.RowsUsed().Count()); i++)
                {
                    var rowPreview = string.Join(" | ", worksheet.Row(i).CellsUsed().Take(5).Select(c => c.GetString()));
                    System.Diagnostics.Debug.WriteLine($"  Row {i}: {rowPreview}");
                    statusCallback?.Invoke($"    Zeile {i}: {rowPreview}");
                }
                throw new Exception($"Could not find header row in worksheet '{worksheet.Name}'.\n\nPlease make sure the Excel file is a valid MEWS Reservation Report export with proper column headers.\n\nExpected columns: Guest Name, Check-in, Check-out, Room Number, etc.");
            }

            System.Diagnostics.Debug.WriteLine($"Found header row: {headerRow}");
            statusCallback?.Invoke($"  ✓ Header-Zeile gefunden: Zeile {headerRow}");

            var columnMap = BuildColumnMap(worksheet, headerRow.Value);
            statusCallback?.Invoke($"  ✓ {columnMap.Count} Spalten erkannt");

            // Show ALL column names for debugging
            statusCallback?.Invoke($"  Alle Spalten-Namen:");
            var lastColumn = worksheet.Row(headerRow.Value).LastCellUsed()?.Address.ColumnNumber ?? 50;
            for (int col = 1; col <= Math.Min(lastColumn, 50); col++)
            {
                var header = worksheet.Cell(headerRow.Value, col).GetString().Trim();
                if (!string.IsNullOrWhiteSpace(header))
                {
                    statusCallback?.Invoke($"    Spalte {col}: {header}");
                }
            }

            // Show key columns
            statusCallback?.Invoke($"\n  Wichtige Spalten:");
            var keyColumns = new[] { "GuestName", "LastName", "FirstName", "CheckIn", "CheckOut", "RoomNumber", "Rate", "Products", "NumberOfNights", "TotalPersons", "Adults", "Children" };
            foreach (var key in keyColumns)
            {
                if (columnMap.ContainsKey(key))
                {
                    statusCallback?.Invoke($"    ✓ {key}: Spalte {columnMap[key]}");
                }
                else
                {
                    statusCallback?.Invoke($"    ✗ {key}: NICHT GEFUNDEN!");
                }
            }

            // Parse data rows
            var currentRow = headerRow.Value + 1;
            var maxRows = worksheet.LastRowUsed()?.RowNumber() ?? 1000;
            System.Diagnostics.Debug.WriteLine($"Parsing data rows from {currentRow} to {maxRows}");
            statusCallback?.Invoke($"  Lese Datenzeilen {currentRow} bis {maxRows}...");

            int parsedCount = 0;
            int skippedCount = 0;

            while (currentRow <= maxRows && !worksheet.Row(currentRow).IsEmpty())
            {
                try
                {
                    var reservation = ParseReservationRow(worksheet, currentRow, columnMap);
                    if (reservation != null)
                    {
                        reservations.Add(reservation);
                        parsedCount++;
                    }
                    else
                    {
                        skippedCount++;
                    }
                }
                catch (Exception ex)
                {
                    // Log error but continue parsing
                    System.Diagnostics.Debug.WriteLine($"Error parsing row {currentRow}: {ex.Message}");
                    statusCallback?.Invoke($"    Fehler Zeile {currentRow}: {ex.Message}");
                    skippedCount++;
                }

                currentRow++;
            }

            System.Diagnostics.Debug.WriteLine($"Parsing complete: {parsedCount} reservations, {skippedCount} rows skipped");
            statusCallback?.Invoke($"  ✓ Parsing abgeschlossen: {parsedCount} Reservierungen, {skippedCount} Zeilen übersprungen");

            // Try to enhance reservations with age category data (adults/children breakdown)
            try
            {
                EnhanceWithAgeCategoryData(workbook, reservations, statusCallback);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Warning: Could not parse age category data: {ex.Message}");
                statusCallback?.Invoke($"  ℹ Hinweis: Alterskategorien-Daten konnten nicht gelesen werden");
            }

            // Check for duplicate reservations (same guest, room, overlapping dates)
            System.Diagnostics.Debug.WriteLine($"\n=== Checking for duplicate reservations ===");
            var duplicates = reservations
                .GroupBy(r => new { r.GuestName, r.RoomNumber, r.CheckIn, r.CheckOut })
                .Where(g => g.Count() > 1)
                .ToList();

            if (duplicates.Any())
            {
                System.Diagnostics.Debug.WriteLine($"WARNING: Found {duplicates.Count} groups of duplicate reservations!");
                foreach (var group in duplicates)
                {
                    System.Diagnostics.Debug.WriteLine($"  Duplicate: {group.Key.GuestName}, Room {group.Key.RoomNumber}, {group.Key.CheckIn:dd.MM} - {group.Key.CheckOut:dd.MM} ({group.Count()} times)");
                }
                statusCallback?.Invoke($"  ⚠ WARNUNG: {duplicates.Count} Gruppen von Duplikaten gefunden!");
            }

            // Remove exact duplicates (keep only one)
            var uniqueReservations = reservations
                .GroupBy(r => new { r.GuestName, r.RoomNumber, r.CheckIn, r.CheckOut, r.Rate })
                .Select(g => g.First())
                .ToList();

            if (uniqueReservations.Count < reservations.Count)
            {
                var removedCount = reservations.Count - uniqueReservations.Count;
                System.Diagnostics.Debug.WriteLine($"Removed {removedCount} exact duplicate reservations");
                statusCallback?.Invoke($"  → {removedCount} exakte Duplikate entfernt");
                reservations = uniqueReservations;
            }

            return reservations;
        }

        private IXLWorksheet? FindReservationWorksheet(XLWorkbook workbook)
        {
            // Try to find the main reservation sheet
            // Common names: First sheet, "Reservierungen", "Reservations", "Maarming", etc.

            // First, try sheets with specific names
            var sheetNames = new[] { "reservierungen", "reservations", "buchungen", "bookings", "maarming" };
            foreach (var sheetName in sheetNames)
            {
                foreach (var ws in workbook.Worksheets)
                {
                    if (ws.Name.ToLower().Contains(sheetName))
                        return ws;
                }
            }

            // If not found, look for a sheet with the expected columns
            foreach (var ws in workbook.Worksheets)
            {
                if (HasReservationColumns(ws))
                    return ws;
            }

            // Fallback: return first worksheet
            return workbook.Worksheet(1);
        }

        private bool HasReservationColumns(IXLWorksheet worksheet)
        {
            // Check if worksheet has typical reservation columns
            for (int row = 1; row <= 20; row++)
            {
                var headerRow = FindHeaderRow(worksheet);
                if (headerRow.HasValue)
                {
                    var columnMap = BuildColumnMap(worksheet, headerRow.Value);
                    // Check if we have essential columns
                    return columnMap.ContainsKey("GuestName") && 
                           (columnMap.ContainsKey("CheckIn") || columnMap.ContainsKey("RoomNumber"));
                }
            }
            return false;
        }
        
        private int? FindHeaderRow(IXLWorksheet worksheet)
        {
            // Search for header row by looking for typical column headers
            for (int row = 1; row <= 30; row++)
            {
                try
                {
                    if (worksheet.Row(row).IsEmpty())
                        continue;

                    // Check multiple columns for header indicators
                    int headerIndicators = 0;
                    var lastCol = Math.Min(worksheet.Row(row).LastCellUsed()?.Address.ColumnNumber ?? 20, 50);

                    for (int col = 1; col <= lastCol; col++)
                    {
                        var cellValue = worksheet.Cell(row, col).GetString().ToLower();
                        if (string.IsNullOrWhiteSpace(cellValue))
                            continue;

                        // Count how many typical header keywords we find
                        if (cellValue.Contains("reservierung") || cellValue.Contains("reservation") ||
                            cellValue.Contains("buchung") || cellValue.Contains("booking") ||
                            cellValue.Contains("gast") || cellValue.Contains("guest") ||
                            cellValue.Contains("name") && (cellValue.Contains("gast") || cellValue.Contains("guest")) ||
                            cellValue.Contains("anreise") || cellValue.Contains("arrival") ||
                            cellValue.Contains("check-in") || cellValue.Contains("checkin") ||
                            cellValue.Contains("abreise") || cellValue.Contains("departure") ||
                            cellValue.Contains("check-out") || cellValue.Contains("checkout") ||
                            cellValue.Contains("zimmer") || cellValue.Contains("room") ||
                            cellValue.Contains("raum") || cellValue.Contains("kategorie") ||
                            cellValue.Contains("status") || cellValue.Contains("rate"))
                        {
                            headerIndicators++;
                        }
                    }

                    // If we found at least 3 header keywords in this row, it's likely the header
                    if (headerIndicators >= 3)
                    {
                        System.Diagnostics.Debug.WriteLine($"Found header row at: {row} with {headerIndicators} indicators");
                        return row;
                    }
                }
                catch
                {
                    // Continue searching if there's an error reading this row
                    continue;
                }
            }

            System.Diagnostics.Debug.WriteLine("No header row found in worksheet");
            return null;
        }
        
        private Dictionary<string, int> BuildColumnMap(IXLWorksheet worksheet, int headerRow)
        {
            var map = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var lastColumn = worksheet.Row(headerRow).LastCellUsed()?.Address.ColumnNumber ?? 50;
            
            for (int col = 1; col <= lastColumn; col++)
            {
                var header = worksheet.Cell(headerRow, col).GetString().Trim().ToLower();
                if (string.IsNullOrWhiteSpace(header)) continue;
                
                // Map common column names
                // Reservation Number - various formats
                if (header.Contains("nummer") && (header.Contains("reservierung") || header.Contains("buchung")))
                {
                    map["ReservationNumber"] = col;
                }
                else if (header == "nummer" && col == 1) // Often first column
                {
                    map["ReservationNumber"] = col;
                }

                // Group Name (MEWS identifier for matching with age categories)
                if (header == "gruppenname" || header == "groupname")
                {
                    map["GroupName"] = col;
                    System.Diagnostics.Debug.WriteLine($"Found GroupName at column {col}: '{header}'");
                }

                // Guest Name - can be split into LastName and FirstName
                if (header == "nachname" || header == "lastname" || header == "familienname")
                {
                    map["LastName"] = col;
                    System.Diagnostics.Debug.WriteLine($"Found LastName at column {col}: '{header}'");
                }

                if (header == "vorname" || header == "firstname" || header == "givenname")
                {
                    map["FirstName"] = col;
                    System.Diagnostics.Debug.WriteLine($"Found FirstName at column {col}: '{header}'");
                }

                // Guest Name - multiple language variants (full name in one column)
                if ((header.Contains("gast") || header.Contains("guest")) && header.Contains("name")) 
                {
                    map["GuestName"] = col;
                    System.Diagnostics.Debug.WriteLine($"Found GuestName at column {col}: '{header}'");
                }
                else if (header == "name" || header == "gastname" || header == "guestname" || 
                         header == "kunde" || header == "customer")
                {
                    map["GuestName"] = col;
                    System.Diagnostics.Debug.WriteLine($"Found GuestName (alt) at column {col}: '{header}'");
                }

                // Check-in
                if (header == "anreise" || header == "arrival" || 
                    header.Contains("check-in") || header.Contains("checkin") || 
                    header.Contains("von") && header.Contains("datum"))
                {
                    map["CheckIn"] = col;
                }

                // Check-out
                if (header == "abreise" || header == "departure" || 
                    header.Contains("check-out") || header.Contains("checkout") || 
                    header.Contains("bis") && header.Contains("datum"))
                {
                    map["CheckOut"] = col;
                }

                // Room Category
                if (header.Contains("zimmer") && header.Contains("kategorie")) 
                    map["RoomCategory"] = col;
                else if (header.Contains("room") && header.Contains("category"))
                    map["RoomCategory"] = col;
                else if (header.Contains("raumkategorie"))
                    map["RoomCategory"] = col;

                // Room Number
                if ((header.Contains("raum") || header.Contains("zimmer") || header.Contains("room")) && 
                    !header.Contains("kategorie") && !header.Contains("category"))
                    map["RoomNumber"] = col;

                // Rate
                if (header.Contains("rate") && !header.Contains("code")) 
                    map["Rate"] = col;
                else if (header.Contains("tarif"))
                    map["Rate"] = col;

                // Rate Code
                if (header.Contains("ratecode") || header.Contains("rate code") || header.Contains("tarifcode")) 
                    map["RateCode"] = col;

                // Adults
                if (header.Contains("erwachsene") || header.Contains("adults") || header == "erw")
                    map["Adults"] = col;

                // Children
                if (header.Contains("kinder") || header.Contains("children") || header == "kind")
                    map["Children"] = col;

                // Number of Nights
                if (header.Contains("anzahl") && header.Contains("nächte"))
                {
                    map["NumberOfNights"] = col;
                    System.Diagnostics.Debug.WriteLine($"Found NumberOfNights at column {col}: '{header}'");
                }
                else if (header.Contains("anzahl") && header.Contains("naechte"))
                {
                    map["NumberOfNights"] = col;
                    System.Diagnostics.Debug.WriteLine($"Found NumberOfNights (alt) at column {col}: '{header}'");
                }
                else if (header == "nights" || header == "number of nights")
                {
                    map["NumberOfNights"] = col;
                    System.Diagnostics.Debug.WriteLine($"Found NumberOfNights (eng) at column {col}: '{header}'");
                }

                // Total Persons
                if (header == "personenzahl" || header == "anzahl personen")
                {
                    map["TotalPersons"] = col;
                    System.Diagnostics.Debug.WriteLine($"Found TotalPersons at column {col}: '{header}'");
                }
                else if (header.Contains("personen") && 
                         !header.Contains("erwachsene") && 
                         !header.Contains("begleit") && 
                         !header.Contains("saldo") &&
                         !header.Contains("balance"))
                {
                    map["TotalPersons"] = col;
                    System.Diagnostics.Debug.WriteLine($"Found TotalPersons (alt) at column {col}: '{header}'");
                }
                else if (header.Contains("persons") || header.Contains("pax") || header.Contains("guests"))
                {
                    map["TotalPersons"] = col;
                    System.Diagnostics.Debug.WriteLine($"Found TotalPersons (eng) at column {col}: '{header}'");
                }

                // Status
                if (header.Contains("status") || header.Contains("state"))
                    map["Status"] = col;

                // Company
                if (header.Contains("firma") || header.Contains("company") || header.Contains("unternehmen"))
                    map["Company"] = col;

                // Channel
                if (header.Contains("channel") || header.Contains("kanal") || header.Contains("quelle") || header.Contains("source"))
                    map["Channel"] = col;

                // Price
                if (header.Contains("preis") || header.Contains("price") || header.Contains("total") || header.Contains("betrag"))
                    map["Price"] = col;

                // Guest Notes
                if (header == "gästenotizen" || header == "guest notes" || header.Contains("gästeanmerkung"))
                    map["GuestNotes"] = col;
                else if (header == "notizen" && !header.Contains("reservierung"))
                    map["GuestNotes"] = col;

                // Reservation Notes
                if (header.Contains("reservierungsnotizen") || header.Contains("reservation notes") || header.Contains("buchungsnotizen"))
                    map["ReservationNotes"] = col;

                // Channel Notes
                if (header.Contains("channelnotizen") || header.Contains("channel notes") || header.Contains("kanalnotizen"))
                    map["ChannelNotes"] = col;

                // Products
                if (header == "produkte" || header == "products")
                    map["Products"] = col;
                else if (header.Contains("produkt") || header.Contains("product") || header.Contains("artikel") || header.Contains("item"))
                    map["Products"] = col;
            }

            // Debug output
            System.Diagnostics.Debug.WriteLine($"\n=== Column Map for worksheet '{worksheet.Name}' ===");
            System.Diagnostics.Debug.WriteLine($"Found {map.Count} columns:");
            foreach (var kvp in map)
            {
                System.Diagnostics.Debug.WriteLine($"  {kvp.Key} -> Column {kvp.Value}");
            }
            System.Diagnostics.Debug.WriteLine("====================================\n");

            return map;
        }
        
        private Reservation? ParseReservationRow(IXLWorksheet worksheet, int row, Dictionary<string, int> columnMap)
        {
            var reservation = new Reservation();

            // Build GuestName from LastName + FirstName or use GuestName column
            string guestName = "";

            if (columnMap.ContainsKey("GuestName"))
            {
                guestName = GetCellValue(worksheet, row, columnMap, "GuestName");
            }
            else if (columnMap.ContainsKey("LastName") || columnMap.ContainsKey("FirstName"))
            {
                var lastName = GetCellValue(worksheet, row, columnMap, "LastName");
                var firstName = GetCellValue(worksheet, row, columnMap, "FirstName");

                if (!string.IsNullOrWhiteSpace(lastName) && !string.IsNullOrWhiteSpace(firstName))
                {
                    guestName = $"{firstName} {lastName}";
                }
                else if (!string.IsNullOrWhiteSpace(lastName))
                {
                    guestName = lastName;
                }
                else if (!string.IsNullOrWhiteSpace(firstName))
                {
                    guestName = firstName;
                }
            }

            if (string.IsNullOrWhiteSpace(guestName))
            {
                // Empty row, skip silently
                return null;
            }

            reservation.GuestName = guestName;

            reservation.GroupName = GetCellValue(worksheet, row, columnMap, "GroupName");
            reservation.ReservationNumber = GetCellValue(worksheet, row, columnMap, "ReservationNumber");
            reservation.CheckIn = ParseDate(GetCellValue(worksheet, row, columnMap, "CheckIn"));
            reservation.CheckOut = ParseDate(GetCellValue(worksheet, row, columnMap, "CheckOut"));
            reservation.RoomCategory = GetCellValue(worksheet, row, columnMap, "RoomCategory");
            reservation.RoomNumber = GetCellValue(worksheet, row, columnMap, "RoomNumber");
            reservation.Rate = GetCellValue(worksheet, row, columnMap, "Rate");
            reservation.RateCode = GetCellValue(worksheet, row, columnMap, "RateCode");
            reservation.Status = GetCellValue(worksheet, row, columnMap, "Status");
            reservation.Company = GetCellValue(worksheet, row, columnMap, "Company");
            reservation.Channel = GetCellValue(worksheet, row, columnMap, "Channel");

            reservation.AdultsCount = ParseInt(GetCellValue(worksheet, row, columnMap, "Adults"));
            reservation.ChildrenCount = ParseInt(GetCellValue(worksheet, row, columnMap, "Children"));
            reservation.TotalPrice = ParseDecimal(GetCellValue(worksheet, row, columnMap, "Price"));

            // DEBUG: Show RAW values from Excel for first row
            if (row == 2)
            {
                System.Diagnostics.Debug.WriteLine($"\n=== DEBUGGING ROW 2 ===");
                if (columnMap.ContainsKey("TotalPersons"))
                {
                    var col = columnMap["TotalPersons"];
                    var rawValue = worksheet.Cell(row, col).Value;
                    var stringValue = worksheet.Cell(row, col).GetString();
                    System.Diagnostics.Debug.WriteLine($"TotalPersons (Spalte {col}):");
                    System.Diagnostics.Debug.WriteLine($"  Raw Value Type: {rawValue.GetType().Name}");
                    System.Diagnostics.Debug.WriteLine($"  Raw Value: '{rawValue}'");
                    System.Diagnostics.Debug.WriteLine($"  String Value: '{stringValue}'");
                }

                if (columnMap.ContainsKey("NumberOfNights"))
                {
                    var col = columnMap["NumberOfNights"];
                    var rawValue = worksheet.Cell(row, col).Value;
                    var stringValue = worksheet.Cell(row, col).GetString();
                    System.Diagnostics.Debug.WriteLine($"NumberOfNights (Spalte {col}):");
                    System.Diagnostics.Debug.WriteLine($"  Raw Value Type: {rawValue.GetType().Name}");
                    System.Diagnostics.Debug.WriteLine($"  Raw Value: '{rawValue}'");
                    System.Diagnostics.Debug.WriteLine($"  String Value: '{stringValue}'");
                }
                System.Diagnostics.Debug.WriteLine($"========================\n");
            }

            // If Adults/Children are not available, use TotalPersons
            if (reservation.AdultsCount == 0 && reservation.ChildrenCount == 0 && columnMap.ContainsKey("TotalPersons"))
            {
                var totalPersonsValue = GetCellValue(worksheet, row, columnMap, "TotalPersons");
                var totalPersons = ParseInt(totalPersonsValue);

                if (row <= 5)  // Show for first 5 rows
                {
                    System.Diagnostics.Debug.WriteLine($"  Row {row}: Guest={reservation.GuestName}, Room={reservation.RoomNumber}");
                    System.Diagnostics.Debug.WriteLine($"    TotalPersons RAW value: '{totalPersonsValue}' -> Parsed: {totalPersons}");
                }

                if (totalPersons > 0)
                {
                    // IMPORTANT: The "Personenzahl" column in MEWS export contains total person-nights, not persons per day
                    // Example: 2 persons × 3 nights = 6 in the column
                    // We need to divide by the number of nights to get persons per day

                    // Try to get number of nights from the "Anzahl (nächte)" column first
                    int numberOfNights = 0;
                    if (columnMap.ContainsKey("NumberOfNights"))
                    {
                        var nightsValue = GetCellValue(worksheet, row, columnMap, "NumberOfNights");
                        numberOfNights = ParseInt(nightsValue);
                        if (row <= 5)
                        {
                            System.Diagnostics.Debug.WriteLine($"    NumberOfNights RAW value: '{nightsValue}' -> Parsed: {numberOfNights}");
                        }
                    }

                    // Fallback: calculate from CheckIn/CheckOut
                    if (numberOfNights == 0)
                    {
                        numberOfNights = (reservation.CheckOut - reservation.CheckIn).Days;
                        if (row <= 5)
                        {
                            System.Diagnostics.Debug.WriteLine($"    Calculated nights from dates: {numberOfNights} ({reservation.CheckIn:dd.MM} - {reservation.CheckOut:dd.MM})");
                        }
                    }

                    if (numberOfNights > 0)
                    {
                        var personsPerDay = totalPersons / numberOfNights;
                        reservation.AdultsCount = personsPerDay;
                        if (row <= 5)
                        {
                            System.Diagnostics.Debug.WriteLine($"    ✓ Result: {totalPersons} / {numberOfNights} = {personsPerDay} persons per day");
                        }
                    }
                    else
                    {
                        // Fallback: if nights calculation fails, use total as-is (likely single night or same-day)
                        reservation.AdultsCount = totalPersons;
                        System.Diagnostics.Debug.WriteLine($"    ⚠ WARNING: numberOfNights=0! Using TotalPersons ({totalPersons}) as Adults directly");
                    }
                }
                else
                {
                    if (row <= 5)
                    {
                        System.Diagnostics.Debug.WriteLine($"    ⚠ WARNING: TotalPersons is 0 or empty!");
                    }
                }
            }

            reservation.GuestNotes = GetCellValue(worksheet, row, columnMap, "GuestNotes");
            reservation.ReservationNotes = GetCellValue(worksheet, row, columnMap, "ReservationNotes");
            reservation.ChannelNotes = GetCellValue(worksheet, row, columnMap, "ChannelNotes");

            // Parse products
            var productsText = GetCellValue(worksheet, row, columnMap, "Products");
            reservation.Products = ParseProducts(productsText, reservation.CheckIn);

            // FALLBACK: If still no person count, try to derive from breakfast products
            if (reservation.AdultsCount == 0 && reservation.ChildrenCount == 0)
            {
                var breakfastProducts = reservation.Products.Where(p => 
                    p.Name.ToLower().Contains("frühstück") || 
                    p.Name.ToLower().Contains("breakfast")).ToList();

                if (breakfastProducts.Any())
                {
                    var totalBreakfasts = breakfastProducts.Sum(p => p.Quantity);
                    var numberOfNights = (reservation.CheckOut - reservation.CheckIn).Days;

                    if (numberOfNights > 0 && totalBreakfasts > 0)
                    {
                        var personsFromBreakfast = totalBreakfasts / numberOfNights;
                        reservation.AdultsCount = personsFromBreakfast;

                        if (row <= 5)
                        {
                            System.Diagnostics.Debug.WriteLine($"    ℹ Calculated persons from breakfast products:");
                            System.Diagnostics.Debug.WriteLine($"      Total breakfasts: {totalBreakfasts}, Nights: {numberOfNights}");
                            System.Diagnostics.Debug.WriteLine($"      → {personsFromBreakfast} persons per day");
                        }
                    }
                }
            }

            // Debug output for first few reservations (rows 2-6 after header)
            if (row <= 10)
            {
                System.Diagnostics.Debug.WriteLine($"\nReservation Row {row}:");
                System.Diagnostics.Debug.WriteLine($"  Guest: {reservation.GuestName}");
                System.Diagnostics.Debug.WriteLine($"  Room: {reservation.RoomNumber}");
                System.Diagnostics.Debug.WriteLine($"  Check-in: {reservation.CheckIn:yyyy-MM-dd}");
                System.Diagnostics.Debug.WriteLine($"  Check-out: {reservation.CheckOut:yyyy-MM-dd}");
                System.Diagnostics.Debug.WriteLine($"  Rate: {reservation.Rate}");
                System.Diagnostics.Debug.WriteLine($"  Adults: {reservation.AdultsCount}, Children: {reservation.ChildrenCount}, Total: {reservation.TotalPersons}");
                System.Diagnostics.Debug.WriteLine($"  Products: {productsText}");
                System.Diagnostics.Debug.WriteLine($"  Parsed Products: {reservation.Products.Count}");
                foreach (var p in reservation.Products)
                {
                    System.Diagnostics.Debug.WriteLine($"    - {p.Name} (Qty: {p.Quantity})");
                }
            }

            return reservation;
        }
        
        private string GetCellValue(IXLWorksheet worksheet, int row, Dictionary<string, int> columnMap, string key)
        {
            if (!columnMap.ContainsKey(key))
                return string.Empty;
            
            return worksheet.Cell(row, columnMap[key]).GetString().Trim();
        }
        
        private DateTime ParseDate(string dateString)
        {
            if (DateTime.TryParse(dateString, out var date))
                return date;
            
            return DateTime.MinValue;
        }
        
        private int ParseInt(string value)
        {
            if (int.TryParse(value, out var result))
                return result;
            
            return 0;
        }
        
        private decimal ParseDecimal(string value)
        {
            value = value.Replace("€", "").Replace(",", ".").Trim();
            if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
                return result;
            
            return 0;
        }
        
        private List<Product> ParseProducts(string productsText, DateTime defaultDate)
        {
            var products = new List<Product>();

            if (string.IsNullOrWhiteSpace(productsText))
            {
                System.Diagnostics.Debug.WriteLine("    ParseProducts: Empty products text");
                return products;
            }

            System.Diagnostics.Debug.WriteLine($"    ParseProducts input: '{productsText}'");

            // Split by common separators
            var lines = productsText.Split(new[] { '\n', ';', '|', ',' }, StringSplitOptions.RemoveEmptyEntries);
            System.Diagnostics.Debug.WriteLine($"    Split into {lines.Length} parts");

            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                if (string.IsNullOrWhiteSpace(trimmed))
                    continue;

                var product = new Product
                {
                    Name = trimmed,
                    Quantity = ExtractQuantity(trimmed),
                    Date = defaultDate
                };

                products.Add(product);
                System.Diagnostics.Debug.WriteLine($"    Added product: {product.Name} (Qty: {product.Quantity})");
            }

            return products;
        }
        
        private int ExtractQuantity(string text)
        {
            var match = System.Text.RegularExpressions.Regex.Match(text, @"(\d+)\s*x");
            if (match.Success && int.TryParse(match.Groups[1].Value, out var qty))
                return qty;

            return 1;
        }

        private void EnhanceWithAgeCategoryData(XLWorkbook workbook, List<Reservation> reservations, Action<string>? statusCallback)
        {
            // Find "Alterskategorien" sheet
            IXLWorksheet? ageSheet = null;
            foreach (var ws in workbook.Worksheets)
            {
                if (ws.Name.ToLower().Contains("alters"))
                {
                    ageSheet = ws;
                    break;
                }
            }

            if (ageSheet == null)
            {
                System.Diagnostics.Debug.WriteLine("Age category sheet not found");
                return;
            }

            System.Diagnostics.Debug.WriteLine($"\n=== Parsing Age Category Data ===");
            System.Diagnostics.Debug.WriteLine($"Using sheet: {ageSheet.Name}");
            statusCallback?.Invoke($"  📊 Lese Alterskategorien aus Sheet '{ageSheet.Name}'...");

            // Find header row
            var headerRow = FindHeaderRow(ageSheet);
            if (!headerRow.HasValue)
            {
                System.Diagnostics.Debug.WriteLine("Could not find header in age category sheet");
                return;
            }

            // Build column map for age sheet
            var columnMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var lastColumn = ageSheet.Row(headerRow.Value).LastCellUsed()?.Address.ColumnNumber ?? 50;

            for (int col = 1; col <= lastColumn; col++)
            {
                var header = ageSheet.Cell(headerRow.Value, col).GetString().Trim().ToLower();
                if (string.IsNullOrWhiteSpace(header)) continue;

                // Map columns
                if (header == "gruppenname" || header.Contains("gruppe") && header.Contains("name"))
                {
                    columnMap["GroupName"] = col;
                    System.Diagnostics.Debug.WriteLine($"  Found GroupName at column {col}");
                }
                else if (header == "anreise" || header == "arrival")
                {
                    columnMap["CheckIn"] = col;
                    System.Diagnostics.Debug.WriteLine($"  Found CheckIn at column {col}");
                }
                else if (header == "abreise" || header == "departure")
                {
                    columnMap["CheckOut"] = col;
                    System.Diagnostics.Debug.WriteLine($"  Found CheckOut at column {col}");
                }
                else if (header == "personenzahl" || header.Contains("personen") && !header.Contains("erwachsene"))
                {
                    columnMap["TotalPersons"] = col;
                    System.Diagnostics.Debug.WriteLine($"  Found TotalPersons at column {col}");
                }
                else if (header == "erwachsene" || header == "adults")
                {
                    columnMap["Adults"] = col;
                    System.Diagnostics.Debug.WriteLine($"  Found Adults at column {col}");
                }
                else if (header.Contains("kind") || header.Contains("child"))
                {
                    columnMap["Children"] = col;
                    System.Diagnostics.Debug.WriteLine($"  Found Children at column {col}");
                }
                else if (header.Contains("baby"))
                {
                    columnMap["Babies"] = col;
                    System.Diagnostics.Debug.WriteLine($"  Found Babies at column {col}");
                }
            }

            System.Diagnostics.Debug.WriteLine($"Age category columns found:");
            foreach (var kvp in columnMap)
            {
                System.Diagnostics.Debug.WriteLine($"  {kvp.Key} -> Column {kvp.Value}");
            }

            if (!columnMap.ContainsKey("Adults") && !columnMap.ContainsKey("Children"))
            {
                System.Diagnostics.Debug.WriteLine("No age breakdown columns found");
                statusCallback?.Invoke($"  ⚠ Keine Erwachsene/Kinder-Spalten gefunden");
                return;
            }

            if (!columnMap.ContainsKey("GroupName") && !columnMap.ContainsKey("CheckIn"))
            {
                System.Diagnostics.Debug.WriteLine("No group name or checkin columns found");
                statusCallback?.Invoke($"  ⚠ Keine Gruppenname/Anreise-Spalten gefunden");
                return;
            }

            // Parse age data rows
            int matchedCount = 0;
            int totalRows = 0;
            int attemptedMatches = 0;

            var maxRow = ageSheet.LastRowUsed()?.RowNumber() ?? 1000;
            for (int row = headerRow.Value + 1; row <= maxRow; row++)
            {
                if (ageSheet.Row(row).IsEmpty())
                    break;

                totalRows++;

                try
                {
                    var groupName = GetCellValue(ageSheet, row, columnMap, "GroupName");
                    var checkIn = ParseDate(GetCellValue(ageSheet, row, columnMap, "CheckIn"));
                    var checkOut = ParseDate(GetCellValue(ageSheet, row, columnMap, "CheckOut"));
                    var adults = ParseInt(GetCellValue(ageSheet, row, columnMap, "Adults"));
                    var children = ParseInt(GetCellValue(ageSheet, row, columnMap, "Children"));
                    var babies = ParseInt(GetCellValue(ageSheet, row, columnMap, "Babies"));

                    if (string.IsNullOrWhiteSpace(groupName))
                        continue;

                    attemptedMatches++;

                    if (totalRows <= 3)
                    {
                        System.Diagnostics.Debug.WriteLine($"  Age row {row}: Group='{groupName}', CheckIn={checkIn:dd.MM.yyyy}, Adults={adults}, Children={children}, Babies={babies}");
                    }

                    // Find matching reservation by GroupName + dates
                    var matchingReservation = reservations.FirstOrDefault(r =>
                        r.GroupName.Equals(groupName, StringComparison.OrdinalIgnoreCase) &&
                        r.CheckIn == checkIn &&
                        r.CheckOut == checkOut
                    );

                    if (matchingReservation != null)
                    {
                        matchingReservation.AdultsCount = adults;
                        matchingReservation.ChildrenCount = children + babies; // Combine children and babies
                        matchedCount++;

                        if (totalRows <= 3)
                        {
                            System.Diagnostics.Debug.WriteLine($"    ✓ MATCHED with reservation: {matchingReservation.GuestName} (Group: {matchingReservation.GroupName})");
                        }
                    }
                    else if (totalRows <= 3)
                    {
                        System.Diagnostics.Debug.WriteLine($"    ✗ NO MATCH FOUND");
                        // Show what we're looking for
                        var possibleMatches = reservations.Where(r => r.GroupName.Equals(groupName, StringComparison.OrdinalIgnoreCase)).ToList();
                        if (possibleMatches.Any())
                        {
                            System.Diagnostics.Debug.WriteLine($"      Found reservations with same GroupName but different dates:");
                            foreach (var pm in possibleMatches.Take(2))
                            {
                                System.Diagnostics.Debug.WriteLine($"        - {pm.GuestName}: {pm.CheckIn:dd.MM.yyyy} - {pm.CheckOut:dd.MM.yyyy}");
                            }
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"      No reservations found with GroupName '{groupName}'");
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error parsing age data row {row}: {ex.Message}");
                }
            }

            System.Diagnostics.Debug.WriteLine($"\nAge category parsing complete:");
            System.Diagnostics.Debug.WriteLine($"  Total rows processed: {totalRows}");
            System.Diagnostics.Debug.WriteLine($"  Attempted matches: {attemptedMatches}");
            System.Diagnostics.Debug.WriteLine($"  Successful matches: {matchedCount}/{reservations.Count}");
            statusCallback?.Invoke($"  ✓ {matchedCount} von {reservations.Count} Reservierungen mit Alterskategorien angereichert");
        }
    }
}
