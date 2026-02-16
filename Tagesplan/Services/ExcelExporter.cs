using ClosedXML.Excel;
using Tagesplan.Models;
using Tagesplan.Helpers;

namespace Tagesplan.Services
{
    public class ExcelExporter
    {
        public void ExportAllLists(
            List<HousekeepingTask> hkTasks,
            List<FrontOfficeTask> foTasks,
            List<BreakfastDay> breakfastDays,
            List<OccupancyDay> occupancyDays,
            List<DailyArrival> arrivals,
            string outputPath)
        {
            using var workbook = new XLWorkbook();

            // Create worksheets in logical order
            ExportOccupancyOverview(workbook.Worksheets.Add("Belegung"), occupancyDays);
            ExportBreakfastOverview(workbook.Worksheets.Add("Frühstück"), breakfastDays);
            ExportHousekeepingTasks(workbook.Worksheets.Add("Hauswirtschaft"), hkTasks);
            ExportFrontOfficeTasks(workbook.Worksheets.Add("Vorbereiten"), foTasks);
            ExportArrivals(workbook.Worksheets.Add("Ankünfte"), arrivals);

            workbook.SaveAs(outputPath);
        }
        
        private void ExportHousekeepingTasks(IXLWorksheet worksheet, List<HousekeepingTask> tasks)
        {
            worksheet.Cell(1, 1).Value = "Datum";
            worksheet.Cell(1, 2).Value = "Zimmer";
            worksheet.Cell(1, 3).Value = "Gast";
            worksheet.Cell(1, 4).Value = "Aufgabe";
            worksheet.Cell(1, 5).Value = "Beschreibung";
            worksheet.Cell(1, 6).Value = "Erledigt";
            
            // Style header
            var headerRange = worksheet.Range(1, 1, 1, 6);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            
            int row = 2;
            foreach (var task in tasks)
            {
                worksheet.Cell(row, 1).Value = task.Date.ToString("dd.MM.yyyy");
                worksheet.Cell(row, 2).Value = task.RoomNumber;
                worksheet.Cell(row, 3).Value = task.GuestName;
                worksheet.Cell(row, 4).Value = task.TaskType;
                worksheet.Cell(row, 5).Value = task.Description;
                worksheet.Cell(row, 6).Value = task.IsCompleted ? "✓" : "";
                
                // Apply color coding
                var rowRange = worksheet.Range(row, 1, row, 6);
                rowRange.Style.Fill.BackgroundColor = XLColor.FromColor(ColorHelper.GetLighterColor(task.TaskColor));
                
                row++;
            }
            
            worksheet.Columns().AdjustToContents();
        }
        
        private void ExportFrontOfficeTasks(IXLWorksheet worksheet, List<FrontOfficeTask> tasks)
        {
            worksheet.Cell(1, 1).Value = "Datum";
            worksheet.Cell(1, 2).Value = "Zimmer";
            worksheet.Cell(1, 3).Value = "Gast";
            worksheet.Cell(1, 4).Value = "Aufgabe";
            worksheet.Cell(1, 5).Value = "Beschreibung";
            worksheet.Cell(1, 6).Value = "Anzahl";
            worksheet.Cell(1, 7).Value = "Notizen";
            
            var headerRange = worksheet.Range(1, 1, 1, 7);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            
            int row = 2;
            foreach (var task in tasks)
            {
                worksheet.Cell(row, 1).Value = task.Date.ToString("dd.MM.yyyy");
                worksheet.Cell(row, 2).Value = task.RoomNumber;
                worksheet.Cell(row, 3).Value = task.GuestName;
                worksheet.Cell(row, 4).Value = task.TaskType;
                worksheet.Cell(row, 5).Value = task.Description;
                worksheet.Cell(row, 6).Value = task.Quantity;
                worksheet.Cell(row, 7).Value = task.Notes;
                
                var rowRange = worksheet.Range(row, 1, row, 7);
                rowRange.Style.Fill.BackgroundColor = XLColor.FromColor(ColorHelper.GetLighterColor(task.TaskColor));
                
                row++;
            }
            
            worksheet.Columns().AdjustToContents();
        }
        
        private void ExportBreakfastOverview(IXLWorksheet worksheet, List<BreakfastDay> days)
        {
            worksheet.Cell(1, 1).Value = "Datum";
            worksheet.Cell(1, 2).Value = "Wochentag";
            worksheet.Cell(1, 3).Value = "Gesamt";
            worksheet.Cell(1, 4).Value = "Erwachsene";
            worksheet.Cell(1, 5).Value = "Kinder";
            worksheet.Cell(1, 6).Value = "Zimmer";
            
            var headerRange = worksheet.Range(1, 1, 1, 6);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            
            int row = 2;
            foreach (var day in days)
            {
                worksheet.Cell(row, 1).Value = day.Date.ToString("dd.MM.yyyy");
                worksheet.Cell(row, 2).Value = day.DayOfWeek;
                worksheet.Cell(row, 3).Value = day.TotalBreakfasts;
                worksheet.Cell(row, 4).Value = day.Adults;
                worksheet.Cell(row, 5).Value = day.Children;
                worksheet.Cell(row, 6).Value = string.Join(", ", day.RoomNumbers);
                
                // Highlight weekends
                if (day.Date.DayOfWeek == DayOfWeek.Saturday || day.Date.DayOfWeek == DayOfWeek.Sunday)
                {
                    var rowRange = worksheet.Range(row, 1, row, 6);
                    rowRange.Style.Fill.BackgroundColor = XLColor.LightBlue;
                }
                
                row++;
            }
            
            worksheet.Columns().AdjustToContents();
        }
        
        private void ExportOccupancyOverview(IXLWorksheet worksheet, List<OccupancyDay> days)
        {
            worksheet.Cell(1, 1).Value = "Datum";
            worksheet.Cell(1, 2).Value = "Wochentag";
            worksheet.Cell(1, 3).Value = "Zimmer";
            worksheet.Cell(1, 4).Value = "Personen";
            worksheet.Cell(1, 5).Value = "Erwachsene";
            worksheet.Cell(1, 6).Value = "Kinder";
            worksheet.Cell(1, 7).Value = "Ratencodes";
            
            var headerRange = worksheet.Range(1, 1, 1, 7);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            
            int row = 2;
            foreach (var day in days)
            {
                worksheet.Cell(row, 1).Value = day.Date.ToString("dd.MM.yyyy");
                worksheet.Cell(row, 2).Value = day.DayOfWeek;
                worksheet.Cell(row, 3).Value = day.OccupiedRooms;
                worksheet.Cell(row, 4).Value = day.TotalPersons;
                worksheet.Cell(row, 5).Value = day.Adults;
                worksheet.Cell(row, 6).Value = day.Children;
                
                var rateCodes = string.Join(", ", day.RateCodeCounts.Select(kvp => $"{kvp.Key}: {kvp.Value}"));
                worksheet.Cell(row, 7).Value = rateCodes;
                
                if (day.Date.DayOfWeek == DayOfWeek.Saturday || day.Date.DayOfWeek == DayOfWeek.Sunday)
                {
                    var rowRange = worksheet.Range(row, 1, row, 7);
                    rowRange.Style.Fill.BackgroundColor = XLColor.LightBlue;
                }
                
                row++;
            }
            
            worksheet.Columns().AdjustToContents();
        }
        
        private void ExportArrivals(IXLWorksheet worksheet, List<DailyArrival> arrivals)
        {
            worksheet.Cell(1, 1).Value = "Zimmer";
            worksheet.Cell(1, 2).Value = "Gast";
            worksheet.Cell(1, 3).Value = "Anreise";
            worksheet.Cell(1, 4).Value = "Abreise";
            worksheet.Cell(1, 5).Value = "Nächte";
            worksheet.Cell(1, 6).Value = "Personen";
            worksheet.Cell(1, 7).Value = "Rate";
            worksheet.Cell(1, 8).Value = "Sonderwünsche";
            worksheet.Cell(1, 9).Value = "Alle Notizen";
            
            var headerRange = worksheet.Range(1, 1, 1, 9);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            
            int row = 2;
            foreach (var arrival in arrivals)
            {
                worksheet.Cell(row, 1).Value = arrival.RoomNumber;
                worksheet.Cell(row, 2).Value = arrival.GuestName;
                worksheet.Cell(row, 3).Value = arrival.ArrivalDate.ToString("dd.MM.yyyy HH:mm");
                worksheet.Cell(row, 4).Value = arrival.DepartureDate.ToString("dd.MM.yyyy HH:mm");
                worksheet.Cell(row, 5).Value = arrival.Nights;
                worksheet.Cell(row, 6).Value = $"{arrival.Adults + arrival.Children} ({arrival.Adults} Erw., {arrival.Children} K)";
                worksheet.Cell(row, 7).Value = arrival.Rate;
                worksheet.Cell(row, 8).Value = string.Join(", ", arrival.SpecialRequests);
                worksheet.Cell(row, 9).Value = arrival.AllNotes;
                
                // Highlight if special requests exist
                if (arrival.SpecialRequests.Any())
                {
                    var rowRange = worksheet.Range(row, 1, row, 9);
                    rowRange.Style.Fill.BackgroundColor = XLColor.FromColor(ColorHelper.GetLighterColor(ColorHelper.SpecialRequestColor));
                }
                
                row++;
            }
            
            worksheet.Columns().AdjustToContents();
        }
    }
}
