using System.Text.Json;

namespace Tagesplan.Configuration
{
    public class AppConfig
    {
        public string MewsUrl { get; set; } = "https://app.mews.com";
        public string DownloadPath { get; set; } = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), 
            "MEWS_Downloads");
        public int BreakfastOverviewDays { get; set; } = 14;
        public int OccupancyOverviewDays { get; set; } = 7;
        public bool AutoOpenExcelAfterExport { get; set; } = true;
        public int PlaywrightTimeoutSeconds { get; set; } = 300;
        
        private static string ConfigFilePath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Tagesplan",
            "config.json");
        
        public static AppConfig Load()
        {
            try
            {
                if (File.Exists(ConfigFilePath))
                {
                    var json = File.ReadAllText(ConfigFilePath);
                    return JsonSerializer.Deserialize<AppConfig>(json) ?? new AppConfig();
                }
            }
            catch
            {
                // If loading fails, return default config
            }
            
            return new AppConfig();
        }
        
        public void Save()
        {
            try
            {
                var directory = Path.GetDirectoryName(ConfigFilePath);
                if (directory != null && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(this, options);
                File.WriteAllText(ConfigFilePath, json);
            }
            catch
            {
                // Silently fail if we can't save config
            }
        }
    }
}
