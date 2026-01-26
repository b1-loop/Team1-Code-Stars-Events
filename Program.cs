
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Team1_Code_Stars_Events.Models;

namespace SQLTeam
{
    internal class Program
    {
        static void Main()
        {
            using var db = new AppDbContext();

            if (!db.Database.CanConnect())
            {
                Console.WriteLine("❌ Kan inte ansluta till databasen");
                Console.ReadKey();
                return;
            }

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

                var choice = Console.ReadLine();

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

            var events = db.Events.ToList();

            foreach (var e in events)
            {
                Console.WriteLine($"{e.EventId} | {e.Title} | {e.StartDate:d}");
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
                .ToList();

            foreach (var e in events)
            {
                Console.WriteLine($"Event: {e.Title}");
                Console.WriteLine($"Datum: {e.StartDate:d}");
                Console.WriteLine($"Lokal: {e.Venue.Name} (Kapacitet: {e.Venue.Capacity})");
                Console.WriteLine($"Arrangör: {e.Organizer.Name}");
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
                .ToList();

            foreach (var c in customers)
            {
                Console.WriteLine($"{c.FirstName} {c.LastName}");

                if (!c.Tickets.Any())
                {
                    Console.WriteLine("  (Inga biljetter)");
                }
                else
                {
                    foreach (var t in c.Tickets)
                    {
                        Console.WriteLine($"  🎟 {t.Event.Title} ({t.Event.StartDate:d})");
                    }
                }

                Console.WriteLine();
            }

            Console.ReadKey();
        }
    }
}
