using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using Team1_Code_Stars_Events.Models;

namespace SQLTeam
{
    internal class Program
    {
        static void Main()
        {
            // 1) Läs config (appsettings.json ligger i projektet och kopieras till output)
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var connectionString = config.GetConnectionString("EventifyDb");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                Console.WriteLine("❌ Connection string saknas. Kontrollera appsettings.json -> ConnectionStrings:EventifyDb");
                Console.ReadKey();
                return;
            }

            // 2) Bygg DbContextOptions och skapa DbContext
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(connectionString)
                .Options;

            using var db = new AppDbContext(options);

            // 3) Testa anslutning (och skriv riktiga fel om det failar)
            try
            {
                if (!db.Database.CanConnect())
                {
                    Console.WriteLine("❌ Kan inte ansluta till databasen (Database.CanConnect() = false).");
                    Console.ReadKey();
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Kunde inte ansluta till databasen:");
                Console.WriteLine(ex.Message);
                Console.ReadKey();
                return;
            }

            // 4) Meny-loop
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=================================");
                Console.WriteLine("        EVENTIFY CONSOLE");
                Console.WriteLine("=================================");
                Console.WriteLine("1) Lista alla events");
                Console.WriteLine("2) Visa events + venue + organizer");
                Console.WriteLine("3) Visa kunder & deras biljetter");
                Console.WriteLine("0) Avsluta");
                Console.Write("\nVal: ");

                var choice = Console.ReadLine()?.Trim();

                switch (choice)
                {
                    case "1":
                        ListEvents(db);
                        break;
                    case "2":
                        EventsWithDetails(db);
                        break;
                    case "3":
                        CustomersWithTickets(db);
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Felaktigt val");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static void ListEvents(AppDbContext db)
        {
            Console.Clear();
            Console.WriteLine("=== ALLA EVENTS ===\n");

            var events = db.Events
                .OrderBy(e => e.StartDate)
                .ToList();

            if (!events.Any())
            {
                Console.WriteLine("(Inga events hittades)");
                Console.ReadKey();
                return;
            }

            foreach (var e in events)
            {
                Console.WriteLine($"{e.EventId} | {e.Title} | {e.StartDate:yyyy-MM-dd HH:mm}");
            }

            Console.ReadKey();
        }

        static void EventsWithDetails(AppDbContext db)
        {
            Console.Clear();
            Console.WriteLine("=== EVENTS MED DETALJER ===\n");

            var events = db.Events
                .Include(e => e.Venue)
                .Include(e => e.Organizer)
                .OrderBy(e => e.StartDate)
                .ToList();

            if (!events.Any())
            {
                Console.WriteLine("(Inga events hittades)");
                Console.ReadKey();
                return;
            }

            foreach (var e in events)
            {
                Console.WriteLine($"Event: {e.Title}");
                Console.WriteLine($"Datum: {e.StartDate:yyyy-MM-dd HH:mm}");
                Console.WriteLine($"Lokal: {e.Venue?.Name ?? "(saknas)"} (Kapacitet: {e.Venue?.Capacity.ToString() ?? "-"})");
                Console.WriteLine($"Arrangör: {e.Organizer?.Name ?? "(saknas)"}");
                Console.WriteLine("----------------------------------");
            }

            Console.ReadKey();
        }

        static void CustomersWithTickets(AppDbContext db)
        {
            Console.Clear();
            Console.WriteLine("=== KUNDER & DERAS BILJETTER ===\n");

            var customers = db.Customers
                .Include(c => c.Tickets)
                    .ThenInclude(t => t.Event)
                .OrderBy(c => c.LastName)
                .ThenBy(c => c.FirstName)
                .ToList();

            if (!customers.Any())
            {
                Console.WriteLine("(Inga kunder hittades)");
                Console.ReadKey();
                return;
            }

            foreach (var c in customers)
            {
                Console.WriteLine($"{c.FirstName} {c.LastName}");

                if (c.Tickets == null || !c.Tickets.Any())
                {
                    Console.WriteLine("  (Inga biljetter)");
                }
                else
                {
                    foreach (var t in c.Tickets.OrderBy(t => t.PurchaseDate))
                    {
                        var title = t.Event?.Title ?? "(okänt event)";
                        var date = t.Event != null ? t.Event.StartDate.ToString("yyyy-MM-dd HH:mm") : "-";
                        Console.WriteLine($"  🎟 {title} ({date})");
                    }
                }

                Console.WriteLine();
            }

            Console.ReadKey();
        }
    }
}
