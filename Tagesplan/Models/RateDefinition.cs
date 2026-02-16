namespace Tagesplan.Models
{
    public class RateDefinition
    {
        public string RateName { get; set; } = string.Empty;
        public List<string> IncludedProducts { get; set; } = new();
        public List<string> IncludedServices { get; set; } = new();
        public string Description { get; set; } = string.Empty;
        public Color DisplayColor { get; set; } = Color.Gray;
    }
}
