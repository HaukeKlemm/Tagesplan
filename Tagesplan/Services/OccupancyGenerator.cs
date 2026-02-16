using Tagesplan.Models;

namespace Tagesplan.Services
{
    public class OccupancyGenerator
    {
        public List<OccupancyDay> GenerateOccupancyOverview(List<Reservation> reservations, DateTime startDate, int days = 7)
        {
            var occupancyDays = new List<OccupancyDay>();
            
            for (int i = 0; i < days; i++)
            {
                var date = startDate.AddDays(i);
                var day = new OccupancyDay
                {
                    Date = date,
                    DayOfWeek = date.ToString("dddd", new System.Globalization.CultureInfo("de-DE"))
                };
                
                // Find all reservations that occupy a room on this date
                var reservationsOnDate = reservations.Where(r =>
                    r.CheckIn <= date &&
                    r.CheckOut > date
                ).ToList();
                
                day.OccupiedRooms = reservationsOnDate.Count;
                
                foreach (var reservation in reservationsOnDate)
                {
                    day.Adults += reservation.AdultsCount;
                    day.Children += reservation.ChildrenCount;
                    
                    // Count rate codes
                    var rateCode = string.IsNullOrWhiteSpace(reservation.RateCode) 
                        ? reservation.Rate 
                        : reservation.RateCode;
                    
                    if (!string.IsNullOrWhiteSpace(rateCode))
                    {
                        if (day.RateCodeCounts.ContainsKey(rateCode))
                            day.RateCodeCounts[rateCode]++;
                        else
                            day.RateCodeCounts[rateCode] = 1;
                    }
                }
                
                day.TotalPersons = day.Adults + day.Children;
                occupancyDays.Add(day);
            }
            
            return occupancyDays;
        }
    }
}
