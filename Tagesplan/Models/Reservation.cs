namespace Tagesplan.Models
{
    public class Reservation
    {
        public string ReservationNumber { get; set; } = string.Empty;
        public string GuestName { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty; // MEWS group identifier for matching
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public string RoomCategory { get; set; } = string.Empty;
        public string RoomNumber { get; set; } = string.Empty;
        public string Rate { get; set; } = string.Empty;
        public string RateCode { get; set; } = string.Empty;
        public int AdultsCount { get; set; }
        public int ChildrenCount { get; set; }
        public int TotalPersons => AdultsCount + ChildrenCount;
        public string Status { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string Channel { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        
        public string GuestNotes { get; set; } = string.Empty;
        public string ReservationNotes { get; set; } = string.Empty;
        public string ChannelNotes { get; set; } = string.Empty;
        
        public List<Product> Products { get; set; } = new();
        
        public string GetAllNotes()
        {
            var notes = new List<string>();
            if (!string.IsNullOrWhiteSpace(GuestNotes)) notes.Add(GuestNotes);
            if (!string.IsNullOrWhiteSpace(ReservationNotes)) notes.Add(ReservationNotes);
            if (!string.IsNullOrWhiteSpace(ChannelNotes)) notes.Add(ChannelNotes);
            return string.Join(" | ", notes);
        }
    }
}
