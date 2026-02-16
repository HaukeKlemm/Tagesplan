using Tagesplan.Models;
using System.Text.RegularExpressions;

namespace Tagesplan.Services
{
    public class ArrivalGenerator
    {
        private static readonly string[] SpecialKeywords = new[]
        {
            "glutenfrei", "laktosefrei", "vegan", "vegetarisch",
            "allergie", "parkplatz", "garage", "hund", "haustier",
            "firmenrechnung", "rechnung", "früher check-in", "später check-out",
            "geburtstag", "hochzeitstag", "jubiläum", "baby", "kinderbett",
            "rollstuhl", "barrierefrei", "nichtraucher"
        };
        
        public List<DailyArrival> GenerateArrivalsForDate(List<Reservation> reservations, DateTime date)
        {
            var arrivals = new List<DailyArrival>();
            
            var arrivingReservations = reservations.Where(r => r.CheckIn.Date == date.Date).ToList();
            
            foreach (var reservation in arrivingReservations)
            {
                var arrival = new DailyArrival
                {
                    GuestName = reservation.GuestName,
                    RoomNumber = reservation.RoomNumber,
                    ArrivalDate = reservation.CheckIn,
                    DepartureDate = reservation.CheckOut,
                    Adults = reservation.AdultsCount,
                    Children = reservation.ChildrenCount,
                    Rate = reservation.Rate,
                    AllNotes = reservation.GetAllNotes()
                };
                
                // Extract special requests from notes
                arrival.SpecialRequests = ExtractSpecialRequests(arrival.AllNotes);
                
                arrivals.Add(arrival);
            }
            
            return arrivals.OrderBy(a => a.RoomNumber).ToList();
        }
        
        private List<string> ExtractSpecialRequests(string notes)
        {
            var requests = new List<string>();
            
            if (string.IsNullOrWhiteSpace(notes))
                return requests;
            
            var notesLower = notes.ToLower();
            
            foreach (var keyword in SpecialKeywords)
            {
                if (notesLower.Contains(keyword))
                {
                    requests.Add(keyword.First().ToString().ToUpper() + keyword.Substring(1));
                }
            }
            
            // Extract time requests
            var timePattern = @"(\d{1,2}):(\d{2})";
            var timeMatches = Regex.Matches(notes, timePattern);
            foreach (Match match in timeMatches)
            {
                requests.Add($"Zeit: {match.Value}");
            }
            
            return requests.Distinct().ToList();
        }
    }
}
