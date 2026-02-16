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
                UpdateStatus("Überprüfe Playwright-Installation...");

                // Check if Playwright browsers are installed
                if (!await IsPlaywrightInstalledAsync())
                {
                    UpdateStatus("Playwright-Browser werden installiert (einmalig, ca. 2-3 Minuten)...");
                    if (!await InstallPlaywrightAsync())
                    {
                        UpdateStatus("✗ Fehler: Playwright-Installation fehlgeschlagen!");
                        UpdateStatus("  Bitte manuell installieren: pwsh bin\\Debug\\net8.0-windows\\playwright.ps1 install");
                        return false;
                    }
                    UpdateStatus("✓ Playwright erfolgreich installiert!");
                }

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

        private async Task<bool> IsPlaywrightInstalledAsync()
        {
            try
            {
                // Check if the Chromium/Edge browser driver exists
                var playwright = await Playwright.CreateAsync();
                return true;
            }
            catch (PlaywrightException ex)
            {
                // If we get an exception about missing drivers, Playwright is not installed
                if (ex.Message.Contains("Driver") || ex.Message.Contains("Executable doesn't exist"))
                {
                    return false;
                }
                // For other exceptions, assume it's installed but there's another issue
                return true;
            }
            catch
            {
                return false;
            }
        }

        private async Task<bool> InstallPlaywrightAsync()
        {
            try
            {
                // Get the path to the playwright.ps1 script
                var assemblyPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                var playwrightScriptPath = Path.Combine(assemblyPath!, "playwright.ps1");

                if (!File.Exists(playwrightScriptPath))
                {
                    UpdateStatus($"✗ Playwright-Skript nicht gefunden: {playwrightScriptPath}");
                    return false;
                }

                UpdateStatus("Starte Playwright-Installation...");

                // Run the playwright install command using PowerShell
                var processStartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "pwsh",
                    Arguments = $"-File \"{playwrightScriptPath}\" install chromium",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = System.Diagnostics.Process.Start(processStartInfo);
                if (process == null)
                {
                    UpdateStatus("✗ Konnte Playwright-Installationsprozess nicht starten");
                    return false;
                }

                var output = await process.StandardOutput.ReadToEndAsync();
                var error = await process.StandardError.ReadToEndAsync();
                await process.WaitForExitAsync();

                if (process.ExitCode == 0)
                {
                    UpdateStatus("✓ Playwright-Installation abgeschlossen");
                    return true;
                }
                else
                {
                    UpdateStatus($"✗ Playwright-Installation fehlgeschlagen (Exit Code: {process.ExitCode})");
                    if (!string.IsNullOrWhiteSpace(error))
                    {
                        UpdateStatus($"  Fehler: {error}");
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"✗ Fehler bei Playwright-Installation: {ex.Message}");
                return false;
            }
        }

        private void UpdateStatus(string message)
        {
            StatusChanged?.Invoke(this, message);
        }
    }
}
