namespace Tagesplan.Models
{
    public class OccupancyDay
    {
        public DateTime Date { get; set; }
        public string DayOfWeek { get; set; } = string.Empty;
        public int OccupiedRooms { get; set; }
        public int TotalPersons { get; set; }
        public int Adults { get; set; }
        public int Children { get; set; }
        public Dictionary<string, int> RateCodeCounts { get; set; } = new();
    }
}
