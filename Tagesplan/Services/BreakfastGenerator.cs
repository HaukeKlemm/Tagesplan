using Tagesplan.Models;
using Tagesplan.Helpers;

namespace Tagesplan.Services
{
    public class BreakfastGenerator
    {
        public List<BreakfastDay> GenerateBreakfastOverview(List<Reservation> reservations, DateTime startDate, int days = 14)
        {
            var breakfastDays = new List<BreakfastDay>();

            System.Diagnostics.Debug.WriteLine($"\n=== GenerateBreakfastOverview ===");
            System.Diagnostics.Debug.WriteLine($"Start date: {startDate:yyyy-MM-dd}, Days: {days}");
            System.Diagnostics.Debug.WriteLine($"Total reservations: {reservations.Count}");

            for (int i = 0; i < days; i++)
            {
                var date = startDate.AddDays(i);
                var day = new BreakfastDay
                {
                    Date = date,
                    DayOfWeek = date.ToString("dddd", new System.Globalization.CultureInfo("de-DE"))
                };
                
                // Find all reservations that include this date and have breakfast
                var reservationsOnDate = reservations.Where(r =>
                    r.CheckIn <= date &&
                    r.CheckOut > date &&
                    HasBreakfast(r)
                ).ToList();

                System.Diagnostics.Debug.WriteLine($"\n  Date {date:dd.MM.yyyy} ({date:dddd}): Found {reservationsOnDate.Count} reservations with breakfast");

                // Check for potential duplicates (same room on same date)
                var roomGroups = reservationsOnDate.GroupBy(r => r.RoomNumber).Where(g => g.Count() > 1).ToList();
                if (roomGroups.Any())
                {
                    System.Diagnostics.Debug.WriteLine($"    ⚠ WARNING: Found rooms with multiple reservations:");
                    foreach (var group in roomGroups)
                    {
                        System.Diagnostics.Debug.WriteLine($"      Room {group.Key}: {group.Count()} reservations");
                        foreach (var r in group)
                        {
                            System.Diagnostics.Debug.WriteLine($"        - {r.GuestName}: {r.CheckIn:dd.MM} - {r.CheckOut:dd.MM}, {r.AdultsCount} adults, {r.ChildrenCount} children");
                        }
                    }
                }

                foreach (var reservation in reservationsOnDate)
                {
                    System.Diagnostics.Debug.WriteLine($"    Room {reservation.RoomNumber} ({reservation.GuestName}): Adults={reservation.AdultsCount}, Children={reservation.ChildrenCount}, Total={reservation.TotalPersons}, {reservation.CheckIn:dd.MM}-{reservation.CheckOut:dd.MM}");
                    day.Adults += reservation.AdultsCount;
                    day.Children += reservation.ChildrenCount;
                    day.RoomNumbers.Add(reservation.RoomNumber);
                }

                System.Diagnostics.Debug.WriteLine($"    → Total for {date:dd.MM.yyyy}: {day.Adults} adults + {day.Children} children = {day.TotalBreakfasts} breakfasts, {day.RoomNumbers.Count} rooms");

                day.TotalBreakfasts = day.Adults + day.Children;
                breakfastDays.Add(day);
            }
            
            return breakfastDays;
        }
        
        private bool HasBreakfast(Reservation reservation)
        {
            // Check rate definition first
            var rateDefinition = RateMapper.GetRateDefinition(reservation.Rate);
            if (rateDefinition != null)
            {
                // Check if rate includes breakfast products
                bool hasBreakfastInRate = rateDefinition.IncludedProducts.Any(p => 
                    p.ToLower().Contains("frühstück") || 
                    p.ToLower().Contains("breakfast"));

                if (hasBreakfastInRate)
                {
                    System.Diagnostics.Debug.WriteLine($"  Rate '{reservation.Rate}' includes breakfast");
                    return true;
                }
            }

            // Fallback: Check if reservation has breakfast products explicitly
            bool hasBreakfastProduct = reservation.Products.Any(p =>
                p.Name.ToLower().Contains("frühstück") ||
                p.Name.ToLower().Contains("breakfast")
            );

            if (hasBreakfastProduct)
            {
                System.Diagnostics.Debug.WriteLine($"  Found breakfast in products for {reservation.GuestName}");
            }

            return hasBreakfastProduct;
        }
    }
}
