namespace Tagesplan.Models
{
    public class FrontOfficeTask
    {
        public string RoomNumber { get; set; } = string.Empty;
        public string GuestName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string TaskType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Color TaskColor { get; set; } = Color.White;
        public int Quantity { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}
