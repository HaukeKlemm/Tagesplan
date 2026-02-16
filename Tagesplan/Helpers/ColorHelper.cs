namespace Tagesplan.Helpers
{
    public static class ColorHelper
    {
        // Task colors
        public static readonly Color PresentColor = Color.FromArgb(255, 215, 0); // Gold
        public static readonly Color HalfBoardColor = Color.FromArgb(220, 20, 60); // Crimson Red
        public static readonly Color WellnessColor = Color.FromArgb(70, 130, 180); // Steel Blue
        public static readonly Color KissSalisColor = Color.FromArgb(64, 224, 208); // Turquoise
        public static readonly Color SpecialRequestColor = Color.FromArgb(147, 112, 219); // Medium Purple
        
        // Rate colors
        public static readonly Color ReisenAktuellColor = Color.FromArgb(255, 165, 0); // Orange
        public static readonly Color KleineAuszeitColor = Color.FromArgb(255, 182, 193); // Light Pink
        public static readonly Color RomantikSpezialColor = Color.FromArgb(220, 20, 60); // Crimson Red
        public static readonly Color KissalisGeniesenColor = Color.FromArgb(144, 238, 144); // Light Green
        public static readonly Color StandardColor = Color.FromArgb(169, 169, 169); // Dark Gray
        public static readonly Color OTAColor = Color.FromArgb(135, 206, 250); // Light Sky Blue
        
        public static Color GetTaskColor(string taskType)
        {
            return taskType.ToLower() switch
            {
                var t when t.Contains("sekt") || t.Contains("obstkorb") || t.Contains("deko") => PresentColor,
                var t when t.Contains("halbpension") || t.Contains("hp") || t.Contains("dinner") => HalfBoardColor,
                var t when t.Contains("massage") || t.Contains("wellness") || t.Contains("entspannung") => WellnessColor,
                var t when t.Contains("kissalis") => KissSalisColor,
                var t when t.Contains("sonderwunsch") || t.Contains("special") => SpecialRequestColor,
                _ => Color.White
            };
        }
        
        public static Color GetRateColor(string rateName)
        {
            var rateNameLower = rateName.ToLower();
            
            if (rateNameLower.Contains("reisenaktuell") || rateNameLower.Contains("reisen aktuell"))
                return ReisenAktuellColor;
            
            if (rateNameLower.Contains("kleine auszeit"))
                return KleineAuszeitColor;
            
            if (rateNameLower.Contains("romantik") && rateNameLower.Contains("spezial"))
                return RomantikSpezialColor;
            
            if (rateNameLower.Contains("kissalis") && rateNameLower.Contains("genießen"))
                return KissalisGeniesenColor;
            
            if (rateNameLower.Contains("booking") || rateNameLower.Contains("expedia") || 
                rateNameLower.Contains("hrs") || rateNameLower.Contains("ota"))
                return OTAColor;
            
            return StandardColor;
        }
        
        public static Color GetLighterColor(Color color, float factor = 0.3f)
        {
            return Color.FromArgb(
                color.A,
                (int)(color.R + (255 - color.R) * factor),
                (int)(color.G + (255 - color.G) * factor),
                (int)(color.B + (255 - color.B) * factor)
            );
        }
    }
}
