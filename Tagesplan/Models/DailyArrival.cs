namespace Tagesplan.Models
{
    public class DailyArrival
    {
        public string GuestName { get; set; } = string.Empty;
        public string RoomNumber { get; set; } = string.Empty;
        public DateTime ArrivalDate { get; set; }
        public DateTime DepartureDate { get; set; }
        public int Nights => (DepartureDate.Date - ArrivalDate.Date).Days;
        public int Adults { get; set; }
        public int Children { get; set; }
        public string Rate { get; set; } = string.Empty;
        public List<string> SpecialRequests { get; set; } = new();
        public string AllNotes { get; set; } = string.Empty;
    }
}
