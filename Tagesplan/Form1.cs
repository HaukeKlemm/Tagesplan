using Tagesplan.Models;
using Tagesplan.Services;
using Tagesplan.Helpers;
using Tagesplan.Configuration;

namespace Tagesplan
{
    public partial class Form1 : Form
    {
        private MewsAutomation? _mewsAutomation;
        private List<Reservation>? _reservations;
        private readonly AppConfig _config;
        private string _downloadPath;

        public Form1()
        {
            InitializeComponent();
            _config = AppConfig.Load();
            _downloadPath = _config.DownloadPath;
            Directory.CreateDirectory(_downloadPath);
            AddStatusMessage("Bereit. Bitte wählen Sie eine Aktion.");
        }

        private async void BtnLoginMews_Click(object? sender, EventArgs e)
        {
            try
            {
                btnLoginMews.Enabled = false;
                AddStatusMessage("Starte MEWS Browser-Automation...");

                _mewsAutomation = new MewsAutomation(_downloadPath);
                _mewsAutomation.StatusChanged += (s, msg) => Invoke(() => AddStatusMessage(msg));

                var success = await _mewsAutomation.InitializeAndLoginAsync(_config.MewsUrl);

                if (success)
                {
                    btnDownloadReport.Enabled = true;
                    AddStatusMessage("✓ Browser geöffnet. Bitte melden Sie sich in MEWS an.");
                }
                else
                {
                    AddStatusMessage("✗ Fehler beim Öffnen des Browsers.");
                    btnLoginMews.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Starten der Browser-Automation:\n{ex.Message}", 
                    "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                AddStatusMessage($"✗ Fehler: {ex.Message}");
                btnLoginMews.Enabled = true;
            }
        }

        private async void BtnDownloadReport_Click(object? sender, EventArgs e)
        {
            if (_mewsAutomation == null)
            {
                MessageBox.Show("Bitte zuerst in MEWS einloggen.", "Hinweis", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                btnDownloadReport.Enabled = false;
                progressBar.Style = ProgressBarStyle.Marquee;

                AddStatusMessage("Warte auf Download...");
                AddStatusMessage("Bitte navigieren Sie in MEWS zu:");
                AddStatusMessage("  → Berichte → Reservierungsbericht");
                AddStatusMessage("  → Detaillierte Ansicht aktivieren");
                AddStatusMessage("  → Produkte und Notizen einblenden");
                AddStatusMessage("  → Export als Excel starten");

                var filePath = await _mewsAutomation.DownloadReservationsReportAsync();

                progressBar.Style = ProgressBarStyle.Continuous;

                if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                {
                    txtFilePath.Text = filePath;
                    AddStatusMessage($"✓ Download erfolgreich: {Path.GetFileName(filePath)}");
                    btnGenerateLists.Enabled = true;
                }
                else
                {
                    AddStatusMessage("✗ Download fehlgeschlagen oder abgebrochen.");
                    btnDownloadReport.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Download:\n{ex.Message}", 
                    "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                AddStatusMessage($"✗ Fehler: {ex.Message}");
                btnDownloadReport.Enabled = true;
                progressBar.Style = ProgressBarStyle.Continuous;
            }
        }

        private void BtnSelectFile_Click(object? sender, EventArgs e)
        {
            using var openFileDialog = new OpenFileDialog
            {
                Filter = "Excel Files (*.xlsx;*.xls)|*.xlsx;*.xls|All Files (*.*)|*.*",
                Title = "MEWS Reservierungsbericht auswählen",
                InitialDirectory = _downloadPath
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtFilePath.Text = openFileDialog.FileName;
                AddStatusMessage($"Datei ausgewählt: {Path.GetFileName(openFileDialog.FileName)}");
                btnGenerateLists.Enabled = true;
            }
        }

        private async void BtnGenerateLists_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFilePath.Text) || !File.Exists(txtFilePath.Text))
            {
                MessageBox.Show("Bitte wählen Sie eine gültige Excel-Datei aus.", 
                    "Hinweis", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                btnGenerateLists.Enabled = false;
                progressBar.Value = 0;
                progressBar.Maximum = 100;

                AddStatusMessage("═══════════════════════════════════════");
                AddStatusMessage("Starte Listen-Generierung...");

                // Parse Excel
                AddStatusMessage("Lese Excel-Datei...");
                progressBar.Value = 10;

                var parser = new MewsExcelParser();
                _reservations = await Task.Run(() => parser.ParseReservations(
                    txtFilePath.Text, 
                    msg => Invoke(() => AddStatusMessage(msg))
                ));

                AddStatusMessage($"✓ {_reservations.Count} Reservierungen geladen");

                // DIAGNOSTIC: Show details about reservations
                if (_reservations.Count > 0)
                {
                    AddStatusMessage($"\nDIAGNOSE:");
                    var futureReservations = _reservations.Where(r => r.CheckIn >= DateTime.Today).ToList();
                    AddStatusMessage($"  - Reservierungen ab heute: {futureReservations.Count}");

                    var rateGroups = _reservations.GroupBy(r => r.Rate).OrderByDescending(g => g.Count());
                    AddStatusMessage($"  - Raten im Export:");
                    foreach (var group in rateGroups.Take(5))
                    {
                        AddStatusMessage($"    • {group.Key}: {group.Count()}x");
                    }

                    var withProducts = _reservations.Count(r => r.Products.Any());
                    AddStatusMessage($"  - Reservierungen mit Produkten: {withProducts}");

                    AddStatusMessage($"  - Erste Reservierung:");
                    var first = _reservations.First();
                    AddStatusMessage($"    • Gast: {first.GuestName}");
                    AddStatusMessage($"    • Zimmer: {first.RoomNumber}");
                    AddStatusMessage($"    • Anreise: {first.CheckIn:dd.MM.yyyy}");
                    AddStatusMessage($"    • Rate: {first.Rate}");
                    AddStatusMessage($"    • Personen: {first.AdultsCount} Erw., {first.ChildrenCount} Kinder");
                    AddStatusMessage($"    • Produkte: {first.Products.Count}");
                    if (first.Products.Any())
                    {
                        foreach (var p in first.Products.Take(3))
                        {
                            AddStatusMessage($"      - {p.Name}");
                        }
                    }
                }
                else
                {
                    AddStatusMessage("✗ FEHLER: Keine Reservierungen gefunden!");
                    AddStatusMessage("Bitte überprüfen Sie:");
                    AddStatusMessage("  1. Ist die Excel-Datei eine MEWS Reservierungsbericht?");
                    AddStatusMessage("  2. Enthält die Datei Daten in den Zeilen?");
                    AddStatusMessage("  3. Sind die Spalten korrekt beschriftet?");
                    btnGenerateLists.Enabled = true;
                    progressBar.Value = 0;
                    return;
                }

                progressBar.Value = 30;

                // Generate lists
                AddStatusMessage("\nGeneriere Housekeeping-Aufgaben...");
                var taskGenerator = new TaskGenerator();
                var hkTasks = taskGenerator.GenerateHousekeepingTasks(_reservations);
                AddStatusMessage($"  → {hkTasks.Count} Aufgaben erstellt");

                if (hkTasks.Count == 0)
                {
                    AddStatusMessage("  ⚠ WARNUNG: Keine Housekeeping-Aufgaben generiert!");
                    AddStatusMessage("    Mögliche Gründe:");
                    AddStatusMessage("    - Keine zukünftigen Anreisen (nur Aufgaben für Check-in >= heute)");
                    AddStatusMessage("    - Raten enthalten keine HK-Produkte (Sekt, Obstkorb, Deko, Entspannung)");
                }

                progressBar.Value = 45;

                AddStatusMessage("Generiere Front Office-Aufgaben...");
                var foTasks = taskGenerator.GenerateFrontOfficeTasks(_reservations);
                AddStatusMessage($"  → {foTasks.Count} Aufgaben erstellt");

                if (foTasks.Count == 0)
                {
                    AddStatusMessage("  ⚠ WARNUNG: Keine Front Office-Aufgaben generiert!");
                    AddStatusMessage("    Mögliche Gründe:");
                    AddStatusMessage("    - Raten enthalten keine FO-Produkte (HP, Massage, KissSalis, Fregetränk)");
                    AddStatusMessage("    - Rate-Namen stimmen nicht überein (siehe Debug-Ausgabe)");
                }

                progressBar.Value = 60;

                AddStatusMessage($"Generiere Frühstücks-Übersicht ({_config.BreakfastOverviewDays} Tage)...");
                var breakfastGenerator = new BreakfastGenerator();
                var breakfastDays = breakfastGenerator.GenerateBreakfastOverview(_reservations, DateTime.Today, _config.BreakfastOverviewDays);
                AddStatusMessage($"  → {breakfastDays.Count} Tage geplant");

                var totalBreakfasts = breakfastDays.Sum(d => d.TotalBreakfasts);
                if (totalBreakfasts == 0)
                {
                    AddStatusMessage($"  ⚠ WARNUNG: Keine Frühstücke gefunden!");
                    AddStatusMessage($"    Mögliche Gründe:");
                    AddStatusMessage($"    - Raten enthalten kein Frühstück");
                    AddStatusMessage($"    - Keine Reservierungen im Zeitraum {DateTime.Today:dd.MM.yyyy} - {DateTime.Today.AddDays(_config.BreakfastOverviewDays):dd.MM.yyyy}");
                }
                else
                {
                    AddStatusMessage($"    (Gesamt: {totalBreakfasts} Frühstücke)");
                }

                progressBar.Value = 70;

                AddStatusMessage($"Generiere Belegungs-Übersicht ({_config.OccupancyOverviewDays} Tage)...");
                var occupancyGenerator = new OccupancyGenerator();
                var occupancyDays = occupancyGenerator.GenerateOccupancyOverview(_reservations, DateTime.Today, _config.OccupancyOverviewDays);
                AddStatusMessage($"  → {occupancyDays.Count} Tage geplant");
                progressBar.Value = 80;

                AddStatusMessage("Generiere Anreisen-Liste für heute...");
                var arrivalGenerator = new ArrivalGenerator();
                var arrivals = arrivalGenerator.GenerateArrivalsForDate(_reservations, DateTime.Today);
                AddStatusMessage($"  → {arrivals.Count} Anreisen heute");
                progressBar.Value = 85;

                // Export to Excel
                AddStatusMessage("Exportiere Listen nach Excel...");
                var outputFileName = $"MEWS_Listen_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                var outputPath = Path.Combine(_downloadPath, outputFileName);

                var exporter = new ExcelExporter();
                await Task.Run(() => exporter.ExportAllLists(hkTasks, foTasks, breakfastDays, occupancyDays, arrivals, outputPath));

                progressBar.Value = 100;
                AddStatusMessage($"✓ Listen erfolgreich exportiert nach:");
                AddStatusMessage($"  {outputPath}");
                AddStatusMessage("═══════════════════════════════════════");

                // Ask to open file
                if (_config.AutoOpenExcelAfterExport)
                {
                    var result = MessageBox.Show(
                        $"Listen erfolgreich generiert!\n\nDatei: {outputFileName}\n\nMöchten Sie die Datei jetzt öffnen?",
                        "Erfolg",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information);

                    if (result == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = outputPath,
                            UseShellExecute = true
                        });
                    }
                }
                else
                {
                    MessageBox.Show($"Listen erfolgreich generiert!\n\nDatei: {outputFileName}",
                        "Erfolg", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                btnGenerateLists.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Generieren der Listen:\n{ex.Message}\n\n{ex.StackTrace}", 
                    "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                AddStatusMessage($"✗ Fehler: {ex.Message}");
                btnGenerateLists.Enabled = true;
                progressBar.Value = 0;
            }
        }

        private async void Form1_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (_mewsAutomation != null)
            {
                await _mewsAutomation.CloseAsync();
            }
        }

        private void AddStatusMessage(string message)
        {
            var timestamp = DateTime.Now.ToString("HH:mm:ss");
            txtStatus.AppendText($"[{timestamp}] {message}{Environment.NewLine}");
            txtStatus.SelectionStart = txtStatus.Text.Length;
            txtStatus.ScrollToCaret();
            lblStatus.Text = message;
            Application.DoEvents();
        }
    }
}
