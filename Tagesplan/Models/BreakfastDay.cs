namespace Tagesplan.Models
{
    public class BreakfastDay
    {
        public DateTime Date { get; set; }
        public string DayOfWeek { get; set; } = string.Empty;
        public int TotalBreakfasts { get; set; }
        public int Adults { get; set; }
        public int Children { get; set; }
        public List<string> RoomNumbers { get; set; } = new();
    }
}
