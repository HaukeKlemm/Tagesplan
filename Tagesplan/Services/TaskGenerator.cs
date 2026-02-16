using Tagesplan.Models;
using Tagesplan.Helpers;

namespace Tagesplan.Services
{
    public class TaskGenerator
    {
        public List<HousekeepingTask> GenerateHousekeepingTasks(List<Reservation> reservations)
        {
            var tasks = new List<HousekeepingTask>();

            System.Diagnostics.Debug.WriteLine($"\n=== GenerateHousekeepingTasks ===");
            System.Diagnostics.Debug.WriteLine($"Processing {reservations.Count} reservations");

            foreach (var reservation in reservations)
            {
                // Check arrival day for room preparation
                if (reservation.CheckIn >= DateTime.Today)
                {
                    System.Diagnostics.Debug.WriteLine($"Checking reservation: {reservation.GuestName} (Room {reservation.RoomNumber}), Rate: {reservation.Rate}");

                    // Get rate definition
                    var rateDefinition = RateMapper.GetRateDefinition(reservation.Rate);
                    var productsToCheck = new List<string>();

                    // Add products from rate definition
                    if (rateDefinition != null)
                    {
                        productsToCheck.AddRange(rateDefinition.IncludedProducts);
                        System.Diagnostics.Debug.WriteLine($"  Rate '{reservation.Rate}' includes: {string.Join(", ", rateDefinition.IncludedProducts)}");
                    }

                    // Add products from reservation (extras)
                    productsToCheck.AddRange(reservation.Products.Select(p => p.Name));

                    // Check for special items based on products
                    foreach (var productName in productsToCheck.Distinct())
                    {
                        var productLower = productName.ToLower();
                        System.Diagnostics.Debug.WriteLine($"  Checking product: '{productName}'");

                        // Sekt
                        if (productLower.Contains("sekt"))
                        {
                            tasks.Add(new HousekeepingTask
                            {
                                RoomNumber = reservation.RoomNumber,
                                GuestName = reservation.GuestName,
                                Date = reservation.CheckIn,
                                TaskType = "Sekt",
                                Description = $"Flasche Sekt im Zimmer bereitstellen",
                                TaskColor = ColorHelper.PresentColor,
                                Priority = 1
                            });
                            System.Diagnostics.Debug.WriteLine($"    → Added Sekt task");
                        }

                        // Obstkorb
                        if (productLower.Contains("obstkorb"))
                        {
                            tasks.Add(new HousekeepingTask
                            {
                                RoomNumber = reservation.RoomNumber,
                                GuestName = reservation.GuestName,
                                Date = reservation.CheckIn,
                                TaskType = "Obstkorb",
                                Description = $"Obstkorb im Zimmer bereitstellen",
                                TaskColor = ColorHelper.PresentColor,
                                Priority = 1
                            });
                            System.Diagnostics.Debug.WriteLine($"    → Added Obstkorb task");
                        }

                        // Zimmerdeko
                        if (productLower.Contains("deko") || productLower.Contains("dekoration"))
                        {
                            tasks.Add(new HousekeepingTask
                            {
                                RoomNumber = reservation.RoomNumber,
                                GuestName = reservation.GuestName,
                                Date = reservation.CheckIn,
                                TaskType = "Zimmerdeko",
                                Description = $"Zimmer dekorieren (Romantik)",
                                TaskColor = ColorHelper.PresentColor,
                                Priority = 1
                            });
                            System.Diagnostics.Debug.WriteLine($"    → Added Zimmerdeko task");
                        }

                        // Entspannungspaket
                        if (productLower.Contains("entspannung"))
                        {
                            tasks.Add(new HousekeepingTask
                            {
                                RoomNumber = reservation.RoomNumber,
                                GuestName = reservation.GuestName,
                                Date = reservation.CheckIn,
                                TaskType = "Entspannungspaket",
                                Description = $"Entspannungspaket im Zimmer bereitstellen",
                                TaskColor = ColorHelper.WellnessColor,
                                Priority = 2
                            });
                            System.Diagnostics.Debug.WriteLine($"    → Added Entspannungspaket task");
                        }
                    }
                }
            }

            System.Diagnostics.Debug.WriteLine($"Generated {tasks.Count} housekeeping tasks");
            System.Diagnostics.Debug.WriteLine("================================\n");

            return tasks.OrderBy(t => t.Date).ThenBy(t => t.Priority).ThenBy(t => t.RoomNumber).ToList();
        }
        
        public List<FrontOfficeTask> GenerateFrontOfficeTasks(List<Reservation> reservations)
        {
            var tasks = new List<FrontOfficeTask>();

            System.Diagnostics.Debug.WriteLine($"\n=== GenerateFrontOfficeTasks ===");
            System.Diagnostics.Debug.WriteLine($"Processing {reservations.Count} reservations");

            foreach (var reservation in reservations)
            {
                System.Diagnostics.Debug.WriteLine($"Checking reservation: {reservation.GuestName} (Room {reservation.RoomNumber}), Rate: {reservation.Rate}");

                // Get rate definition
                var rateDefinition = RateMapper.GetRateDefinition(reservation.Rate);
                var productsToCheck = new List<string>();

                // Add products from rate definition
                if (rateDefinition != null)
                {
                    productsToCheck.AddRange(rateDefinition.IncludedProducts);
                    System.Diagnostics.Debug.WriteLine($"  Rate includes: {string.Join(", ", rateDefinition.IncludedProducts)}");
                }

                // Add products from reservation (extras)
                productsToCheck.AddRange(reservation.Products.Select(p => p.Name));

                // Check for Halbpension
                if (productsToCheck.Any(p => p.ToLower().Contains("hp inkl") || p.ToLower().Contains("halbpension")))
                {
                    for (var date = reservation.CheckIn; date < reservation.CheckOut; date = date.AddDays(1))
                    {
                        tasks.Add(new FrontOfficeTask
                        {
                            RoomNumber = reservation.RoomNumber,
                            GuestName = reservation.GuestName,
                            Date = date,
                            TaskType = "Halbpension",
                            Description = $"HP für {reservation.TotalPersons} Personen",
                            TaskColor = ColorHelper.HalfBoardColor,
                            Quantity = reservation.TotalPersons,
                            Notes = $"{reservation.AdultsCount} Erw., {reservation.ChildrenCount} Kinder"
                        });
                    }
                    System.Diagnostics.Debug.WriteLine($"  → Added Halbpension tasks");
                }

                // Check for Candlelight Dinner (RomantikSpezial)
                var rateLower = reservation.Rate.ToLower();
                if (rateLower.Contains("romantik"))
                {
                    tasks.Add(new FrontOfficeTask
                    {
                        RoomNumber = reservation.RoomNumber,
                        GuestName = reservation.GuestName,
                        Date = reservation.CheckIn,
                        TaskType = "Candlelight Dinner",
                        Description = $"Candlelight Dinner für {reservation.TotalPersons} Personen reservieren",
                        TaskColor = ColorHelper.HalfBoardColor,
                        Quantity = reservation.TotalPersons,
                        Notes = "Tisch reservieren und Restaurant informieren"
                    });
                    System.Diagnostics.Debug.WriteLine($"  → Added Candlelight Dinner task");
                }

                // Check for KissSalis Gutscheine
                if (productsToCheck.Any(p => p.ToLower().Contains("kissalis")))
                {
                    // Determine number of vouchers based on rate
                    int vouchersPerPerson = 1;
                    if (rateLower.Contains("3 nächte") || rateLower.Contains("3 naechte"))
                    {
                        vouchersPerPerson = 2;
                    }

                    var totalVouchers = vouchersPerPerson * reservation.TotalPersons;

                    tasks.Add(new FrontOfficeTask
                    {
                        RoomNumber = reservation.RoomNumber,
                        GuestName = reservation.GuestName,
                        Date = reservation.CheckIn,
                        TaskType = "KissSalis Gutscheine",
                        Description = $"{totalVouchers} KissSalis Gutscheine bereitstellen",
                        TaskColor = ColorHelper.KissSalisColor,
                        Quantity = totalVouchers,
                        Notes = "2-Stunden-Gutscheine"
                    });
                    System.Diagnostics.Debug.WriteLine($"  → Added KissSalis Gutscheine task ({totalVouchers} vouchers)");
                }

                // Check for Massage
                if (productsToCheck.Any(p => p.ToLower().Contains("massage")))
                {
                    var massageCount = reservation.TotalPersons;
                    tasks.Add(new FrontOfficeTask
                    {
                        RoomNumber = reservation.RoomNumber,
                        GuestName = reservation.GuestName,
                        Date = reservation.CheckIn,
                        TaskType = "Massage",
                        Description = $"{massageCount} Massage(n) - Gast kontaktieren für Terminvereinbarung",
                        TaskColor = ColorHelper.WellnessColor,
                        Quantity = massageCount,
                        Notes = "Termin mit Gast absprechen"
                    });
                    System.Diagnostics.Debug.WriteLine($"  → Added Massage task");
                }

                // Check for Freigetränke
                if (productsToCheck.Any(p => p.ToLower().Contains("freigetränk") || p.ToLower().Contains("freigetraenk")))
                {
                    for (var date = reservation.CheckIn; date < reservation.CheckOut; date = date.AddDays(1))
                    {
                        tasks.Add(new FrontOfficeTask
                        {
                            RoomNumber = reservation.RoomNumber,
                            GuestName = reservation.GuestName,
                            Date = date,
                            TaskType = "Freigetränk",
                            Description = $"{reservation.TotalPersons} Freigetränk(e) pro Abend",
                            TaskColor = ColorHelper.HalfBoardColor,
                            Quantity = reservation.TotalPersons,
                            Notes = "Pro Person und Abend"
                        });
                    }
                    System.Diagnostics.Debug.WriteLine($"  → Added Freigetränk tasks");
                }

                // Check for 3-Gang-Abendmenü (Kleine Auszeit)
                if (rateLower.Contains("kleine auszeit"))
                {
                    // Add for each night
                    for (var date = reservation.CheckIn; date < reservation.CheckOut; date = date.AddDays(1))
                    {
                        tasks.Add(new FrontOfficeTask
                        {
                            RoomNumber = reservation.RoomNumber,
                            GuestName = reservation.GuestName,
                            Date = date,
                            TaskType = "3-Gang-Abendmenü",
                            Description = $"3-Gang-Menü für {reservation.TotalPersons} Personen",
                            TaskColor = ColorHelper.HalfBoardColor,
                            Quantity = reservation.TotalPersons,
                            Notes = $"{reservation.AdultsCount} Erw., {reservation.ChildrenCount} Kinder"
                        });
                    }
                    System.Diagnostics.Debug.WriteLine($"  → Added 3-Gang-Abendmenü tasks");
                }
            }

            System.Diagnostics.Debug.WriteLine($"Generated {tasks.Count} front office tasks");
            System.Diagnostics.Debug.WriteLine("================================\n");

            return tasks.OrderBy(t => t.Date).ThenBy(t => t.RoomNumber).ToList();
        }
    }
}
