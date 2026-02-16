using Microsoft.Playwright;

namespace Tagesplan.Services
{
    public class MewsAutomation
    {
        private IBrowser? _browser;
        private IBrowserContext? _context;
        private IPage? _page;
        private readonly string _downloadPath;
        private readonly string _userDataDir;

        public event EventHandler<string>? StatusChanged;

        public MewsAutomation(string downloadPath)
        {
            _downloadPath = downloadPath;
            _userDataDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Rechnungen_MEWS",
                "EdgeProfile"
            );
            Directory.CreateDirectory(_userDataDir);
        }
        
        public async Task<bool> InitializeAndLoginAsync(string mewsUrl = "https://app.mews.com")
        {
            try
            {
                UpdateStatus("Initialisiere Browser mit persistentem Profil...");

                var playwright = await Playwright.CreateAsync();

                // Launch persistent context (verwendet dasselbe Profil wie "Rechnungen MEWS")
                _context = await playwright.Chromium.LaunchPersistentContextAsync(_userDataDir, new BrowserTypeLaunchPersistentContextOptions
                {
                    Headless = false,
                    Channel = "msedge",
                    AcceptDownloads = true,
                    Args = new[]
                    {
                        "--disable-blink-features=AutomationControlled",
                        "--disable-dev-shm-usage"
                    }
                });

                _page = _context.Pages.FirstOrDefault() ?? await _context.NewPageAsync();

                UpdateStatus("Öffne MEWS Login-Seite...");
                await _page.GotoAsync(mewsUrl);

                UpdateStatus("Bereit für manuellen Login.");
                UpdateStatus("Bitte melden Sie sich manuell an und navigieren Sie zum Reservierungsbericht.");

                return true;
            }
            catch (Exception ex)
            {
                UpdateStatus($"Fehler: {ex.Message}");
                return false;
            }
        }
        
        public async Task<string?> DownloadReservationsReportAsync()
        {
            if (_page == null)
            {
                UpdateStatus("Fehler: Browser nicht initialisiert");
                return null;
            }
            
            try
            {
                UpdateStatus("Warte auf Download...");
                UpdateStatus("Bitte starten Sie den Download des Reservierungsberichts in MEWS.");
                UpdateStatus("(Detaillierte Ansicht mit Produkten und Notizen)");
                
                // Wait for download
                var download = await _page.WaitForDownloadAsync(new PageWaitForDownloadOptions
                {
                    Timeout = 300000 // 5 minutes
                });
                
                var fileName = download.SuggestedFilename;
                var filePath = Path.Combine(_downloadPath, fileName);
                
                // Ensure directory exists
                Directory.CreateDirectory(_downloadPath);
                
                await download.SaveAsAsync(filePath);
                
                UpdateStatus($"Download erfolgreich: {fileName}");
                return filePath;
            }
            catch (TimeoutException)
            {
                UpdateStatus("Timeout: Kein Download erkannt. Bitte versuchen Sie es erneut.");
                return null;
            }
            catch (Exception ex)
            {
                UpdateStatus($"Fehler beim Download: {ex.Message}");
                return null;
            }
        }
        
        public async Task CloseAsync()
        {
            if (_page != null)
            {
                await _page.CloseAsync();
                _page = null;
            }

            if (_context != null)
            {
                await _context.CloseAsync();
                _context = null;
            }

            if (_browser != null)
            {
                await _browser.CloseAsync();
                _browser = null;
            }

            UpdateStatus("Browser geschlossen (Session gespeichert)");
        }
        
        private void UpdateStatus(string message)
        {
            StatusChanged?.Invoke(this, message);
        }
    }
}
