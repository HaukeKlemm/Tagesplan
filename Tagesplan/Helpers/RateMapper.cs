using Tagesplan.Models;

namespace Tagesplan.Helpers
{
    public static class RateMapper
    {
        private static List<RateDefinition>? _rates;
        
        public static List<RateDefinition> GetAllRates()
        {
            if (_rates != null)
                return _rates;
            
            _rates = new List<RateDefinition>
            {
                new RateDefinition
                {
                    RateName = "ReisenAktuell",
                    IncludedProducts = new List<string> { "Frühstück Speisen", "Frühstück Getränke", "HP inkl.", "Freigetränk" },
                    IncludedServices = new List<string> { "Frühstück", "Halbpension", "Freigetränk pro Person und Abend" },
                    Description = "Inkl. Frühstück, Halbpension, Freigetränk",
                    DisplayColor = ColorHelper.ReisenAktuellColor
                },
                new RateDefinition
                {
                    RateName = "Kleine Auszeit",
                    IncludedProducts = new List<string> { "Frühstück Speisen", "Frühstück Getränke", "Flasche Sekt", "Obstkorb", "Kissalis Gutschein" },
                    IncludedServices = new List<string> { "Frühstück", "3-Gang-Abendmenü", "Flasche Sekt", "Obstkorb", "KissSalis Gutschein (2h)" },
                    Description = "Inkl. Frühstück, 3-Gang-Menü, Sekt, Obstkorb, KissSalis",
                    DisplayColor = ColorHelper.KleineAuszeitColor
                },
                new RateDefinition
                {
                    RateName = "RomantikSpezial",
                    IncludedProducts = new List<string> { "Frühstück Speisen", "Frühstück Getränke", "Flasche Sekt", "Obstkorb", "Zimmerdeko", "Rückenmassage" },
                    IncludedServices = new List<string> { "Frühstück", "Candlelight Dinner", "Zimmerdekoration", "Flasche Sekt", "Obstkorb", "Rückenmassage pro Person" },
                    Description = "Inkl. Frühstück, Candlelight Dinner, Deko, Sekt, Obstkorb, Massage",
                    DisplayColor = ColorHelper.RomantikSpezialColor
                },
                new RateDefinition
                {
                    RateName = "Kissalis Genießen - 2 Nächte",
                    IncludedProducts = new List<string> { "Frühstück Speisen", "Frühstück Getränke", "HP inkl.", "Entspannungspaket", "Kissalis Gutschein" },
                    IncludedServices = new List<string> { "Frühstück", "Halbpension", "Entspannungspaket", "2-Stunden-Gutschein pro Person" },
                    Description = "Inkl. Frühstück, HP, Entspannungspaket, KissSalis Gutschein",
                    DisplayColor = ColorHelper.KissalisGeniesenColor
                },
                new RateDefinition
                {
                    RateName = "Kissalis Genießen - 3 Nächte",
                    IncludedProducts = new List<string> { "Frühstück Speisen", "Frühstück Getränke", "HP inkl.", "Entspannungspaket", "Kissalis Gutschein" },
                    IncludedServices = new List<string> { "Frühstück", "Halbpension", "Entspannungspaket", "Zwei 2-Stunden-Gutscheine pro Person" },
                    Description = "Inkl. Frühstück, HP, Entspannungspaket, 2x KissSalis Gutschein",
                    DisplayColor = ColorHelper.KissalisGeniesenColor
                }
            };
            
            return _rates;
        }
        
        public static RateDefinition? GetRateDefinition(string rateName)
        {
            var rates = GetAllRates();
            var rateNameLower = rateName.ToLower();
            
            return rates.FirstOrDefault(r => rateNameLower.Contains(r.RateName.ToLower()));
        }
        
        public static bool RateIncludesProduct(string rateName, string productName)
        {
            var rate = GetRateDefinition(rateName);
            if (rate == null) return false;
            
            var productNameLower = productName.ToLower();
            return rate.IncludedProducts.Any(p => productNameLower.Contains(p.ToLower()));
        }
        
        public static bool RateIncludesService(string rateName, string serviceName)
        {
            var rate = GetRateDefinition(rateName);
            if (rate == null) return false;
            
            var serviceNameLower = serviceName.ToLower();
            return rate.IncludedServices.Any(s => serviceNameLower.Contains(s.ToLower()));
        }
    }
}
