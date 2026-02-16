namespace Tagesplan.Models
{
    public class Product
    {
        public string Name { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public DateTime Date { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}
