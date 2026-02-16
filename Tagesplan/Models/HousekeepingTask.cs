namespace Tagesplan.Models
{
    public class HousekeepingTask
    {
        public string RoomNumber { get; set; } = string.Empty;
        public string GuestName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string TaskType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Color TaskColor { get; set; } = Color.White;
        public int Priority { get; set; }
        public bool IsCompleted { get; set; }
    }
}
