using Microsoft.EntityFrameworkCore;
using SQLTeam.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Team1_Code_Stars_Events.Helpers;
using Team1_Code_Stars_Events.Models;

namespace SQLTeam.UI
{
    public class MenuManager
    {
        private readonly DbService _service;
        private readonly AppDbContext _db; // För säkerhetstestning

        // Sparar ner servicen så vi kan använda den i hela klassen
        public MenuManager(DbService service)
        {
            _service = service;
            _db = db; // Exponerar DbContext för säkerhetstest
        }

        public void Run()
        {
            while (true)
            {
                // Rensar och visar menyn
                UIHelper.ShowHeader("🎟️  EVENTIFY ADMIN SYSTEM 2026");
                Console.WriteLine("1) 👥 Lista ALLA Kunder");
                Console.WriteLine("2) 📅 Lista ALLA Events (Säker Vy)");
                Console.WriteLine("3) 🎫 Köp biljett (Skapa relation)");
                Console.WriteLine("4) 👤 Registrera ny kund");
                Console.WriteLine("5) ⚙️  Uppdatera biljettyp & pris");
                Console.WriteLine("6) 🗑️  Radera biljett");
                Console.WriteLine("7) 📊 Rapporter & Statistik (Via Vy)");
                Console.WriteLine("8) 🎫 Visa Kunder & deras Biljetter");
                Console.WriteLine("9) 🔐 Testa Databassäkerhet");
                Console.WriteLine("0) ❌ Avsluta");
                Console.Write("\nVal: ");

                var choice = Console.ReadLine()?.Trim();

                try
                {
                    if (choice == "0") break; // Stänger ner programmet
                    HandleChoice(choice);
                }
                catch (Exception ex)
                {
                    // Fångar alla fel så appen inte dör, visar istället vad som gick snett
                    UIHelper.ShowError($"Ett tekniskt fel uppstod: {ex.Message}");
                    UIHelper.PressAnyKey();
                }
            }
        }

        private void HandleChoice(string choice)
        {
            // Skickar användaren vidare baserat på siffra
            switch (choice)
            {
                case "1": ListCustomers(); break;
                case "2": ListEvents(); break;
                case "3": CreateTicket(); break;
                case "4": AddCustomer(); break;
                case "5": UpdateTicketType(); break;
                case "6": DeleteTicket(); break;
                case "7": ShowReports(); break;
                case "8": ListCustomersWithTickets(); break;
                case "9": TestSecurity(); break;
                default:
                    UIHelper.ShowError("Felaktigt val, försök igen.");
                    UIHelper.PressAnyKey();
                    break;
            }
        }

        // --- MENYFUNKTIONER ---

        private void ListCustomers()
        {
            UIHelper.ShowHeader("👥 KUNDREGISTER");
            var customers = _service.GetAllCustomers();

            if (!customers.Any()) Console.WriteLine("Inga kunder hittades.");
            else
            {
                foreach (var c in customers)
                    Console.WriteLine($" 👤 [ID: {c.CustomerId,-3}] {c.FirstName} {c.LastName,-15} | 📧 {c.Email}");
            }
            UIHelper.PressAnyKey();
        }

        private void ListEvents()
        {
            UIHelper.ShowHeader("📅 EVENEMANGSÖVERSIKT (Säker Vy)");

            // Vi hämtar från vyn för att undvika 'Access Denied' på råtabellerna
            var events = _service.GetUpcomingEvents();

            if (!events.Any()) Console.WriteLine("Inga events hittades.");
            else
            {
                foreach (var e in events)
                {
                    // Notera: Vi använder e.Event (från vyn) istället för e.Title (från tabellen)
                    Console.WriteLine($" 🎭 {e.Event,-20} | 📍 {e.Venue,-15} | 🏢 {e.Organizer}");
                    Console.WriteLine($"    📅 {e.StartDate:yyyy-MM-dd HH:mm} | 🏙️  {e.City}");
                    Console.WriteLine("    ---------------------------------------------------------");
                }
            }
            UIHelper.PressAnyKey();
        }

        private void CreateTicket()
        {
            UIHelper.ShowHeader("🎫 NYTT BILJETTKÖP");

            // Visar hjälplistor så användaren vet vilka ID:n som kan skrivas in
            Console.WriteLine("Alla registrerade kunder:");
            foreach (var c in _service.GetAllCustomers())
                Console.WriteLine($" {c.CustomerId}: {c.FirstName} {c.LastName}");

            Console.WriteLine("\nAlla tillgängliga events:");
            var upcomingEvents = _service.GetUpcomingEvents();
            foreach (var e in upcomingEvents)
                Console.WriteLine($" {e.EventId}: {e.Event}");

            // Läser in ID från användaren (valideras som siffror via UIHelper)
            int cId = UIHelper.GetValidInt("\nAnge Kund-ID: ");
            int eId = UIHelper.GetValidInt("Ange Event-ID: ");

            // Hanterar priser och typer lokalt i menyn
            Console.WriteLine("\nVälj typ: [1] Regular (500kr) [2] Student (300kr) [3] VIP (1200kr) [4] Backstage (2500kr)");
            string type = "";
            decimal price = 0;
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1": type = "Regular"; price = 500m; break;
                case "2": type = "Student"; price = 300m; break;
                case "3": type = "VIP"; price = 1200m; break;
                case "4": type = "Backstage"; price = 2500m; break;
                default: UIHelper.ShowError("Ogiltigt val."); return;
            }

            // Skickar till servicen som sköter databasanropet
            _service.CreateTicket(cId, eId, type, price);
            UIHelper.ShowSuccess("Biljett registrerad!");
            UIHelper.PressAnyKey();
        }

        private void AddCustomer()
        {
            UIHelper.ShowHeader("👤 REGISTRERA NY KUND");
            string fName = UIHelper.GetValidString("Förnamn: ");
            string lName = UIHelper.GetValidString("Efternamn: ");
            string email = UIHelper.GetValidString("Email: ");

            _service.AddCustomer(fName, lName, email);
            UIHelper.ShowSuccess("Kunden har sparats!");
            UIHelper.PressAnyKey();
        }

        private void UpdateTicketType()
        {
            UIHelper.ShowHeader("⚙️  UPPDATERA BILJETT");
            var tickets = _service.GetAllTickets();

            if (!tickets.Any()) { Console.WriteLine("Inga biljetter hittades."); UIHelper.PressAnyKey(); return; }

            // Listar biljetter så vi kan välja rätt ID
            foreach (var t in tickets)
                Console.WriteLine($" [ID: {t.TicketId,-3}] {t.Customer?.FirstName} {t.Customer?.LastName,-15} -> {t.Event?.Title}");

            int tId = UIHelper.GetValidInt("\nAnge ID för biljetten du vill ändra: ");
            var ticket = _service.GetTicketById(tId);

            if (ticket == null) { UIHelper.ShowError("Biljetten hittades inte."); UIHelper.PressAnyKey(); return; }

            // Visar vad biljetten har för värden just nu
            Console.WriteLine($"\nVald biljett: {ticket.Type} ({ticket.Price:C})");
            Console.WriteLine("Ny typ: [1] Regular [2] Student [3] VIP [4] Backstage");

            var choice = Console.ReadLine();
            // Ändrar värdena på det hämtade objektet
            if (choice == "1") { ticket.Type = "Regular"; ticket.Price = 500m; }
            else if (choice == "2") { ticket.Type = "Student"; ticket.Price = 300m; }
            else if (choice == "3") { ticket.Type = "VIP"; ticket.Price = 1200m; }
            else if (choice == "4") { ticket.Type = "Backstage"; ticket.Price = 2500m; }
            else { UIHelper.ShowError("Ogiltigt val."); UIHelper.PressAnyKey(); return; }

            // Sparar ändringarna i databasen
            _service.SaveChanges();
            UIHelper.ShowSuccess("Biljetten har uppdaterats!");
            UIHelper.PressAnyKey();
        }

        private void DeleteTicket()
        {
            UIHelper.ShowHeader("🗑️  RADERA BILJETT");
            var tickets = _service.GetAllTickets();

            if (!tickets.Any()) { Console.WriteLine("Inga biljetter att radera."); UIHelper.PressAnyKey(); return; }

            foreach (var t in tickets)
                Console.WriteLine($" [ID: {t.TicketId,-3}] {t.Customer?.FirstName} {t.Customer?.LastName,-15} | Event: {t.Event?.Title}");

            int tId = UIHelper.GetValidInt("\nAnge ID för biljetten som ska raderas: ");
            var ticket = _service.GetTicketById(tId);

            if (ticket != null)
            {
                // Dubbelkoll så användaren inte raderar av misstag
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"\n⚠️  VARNING: Radera biljetten?");
                Console.Write("Bekräfta med (J/N): ");
                Console.ResetColor();

                if (Console.ReadLine()?.ToUpper() == "J")
                {
                    _service.DeleteTicket(ticket);
                    UIHelper.ShowSuccess("Biljetten har raderats.");
                }
            }
            else UIHelper.ShowError("ID hittades inte.");

            UIHelper.PressAnyKey();
        }

        private void ShowReports()
        {
            UIHelper.ShowHeader("📊 RAPPORTCENTRAL");
            Console.WriteLine("1) 🏆 Top 5 kunder (Flest köp)");
            Console.WriteLine("2) 💰 Försäljningsstatistik per Event (Via Vy)");
            var choice = Console.ReadLine();

            if (choice == "1")
            {
                // Hämtar anonym data för en enkel lista
                var report = _service.GetTopCustomersReport();
                foreach (var r in report) Console.WriteLine($" ⭐ {r.Name,-20} : {r.Count} st");
            }
            else if (choice == "2")
            {
                // Hämtar färdigberäknad statistik från SQL-vyn
                var stats = _service.GetEventStatisticsReport();
                Console.WriteLine($"\n{"EVENT",-20} | {"SÅLDA",-8} | {"INTÄKT",-12} | {"KAPACITET",-10}");
                Console.WriteLine(new string('-', 60));

                foreach (var s in stats)
                {
                    Console.WriteLine($" {s.EventTitle,-19} | {s.TicketsSold,-8} | {s.TotalRevenue,10:C} | {s.MaxCapacity,8}");
                }
            }
            UIHelper.PressAnyKey();
        }

        private void ListCustomersWithTickets()
        {
            UIHelper.ShowHeader("🎟️  KUNDER OCH DERAS BOKADE EVENTS");
            // Hämtar kunder och inkluderar deras biljett-samlingar (Eager Loading)
            var customers = _service.GetCustomersWithTickets();

            foreach (var c in customers)
            {
                Console.WriteLine($"👤 {c.FirstName} {c.LastName} (ID: {c.CustomerId})");
                if (!c.Tickets.Any()) Console.WriteLine("   🚫 Inga biljetter bokade.");
                else
                {
                    // Loopar igenom varje biljett kunden äger
                    foreach (var t in c.Tickets)
                        Console.WriteLine($"   🎫 {t.Event?.Title,-20} | Typ: {t.Type,-10} | Pris: {t.Price:C}");
                }
                Console.WriteLine("---------------------------------");
            }
            UIHelper.PressAnyKey();
        }

        private void TestSecurity()
        {
            UIHelper.ShowHeader("🛡️ SÄKERHETSTEST: INTEGRITETSKONTROLL");

            try
            {
                Console.WriteLine("Systemet försöker läsa direkt från tabellen 'Organizers'...");

                // Denna rad triggar en Exception om inloggningen tillhör AppRole (pga DENY)
                // Om inloggningen tillhör DatabaseAdminRole tillåts anropet (pga GRANT)
                var organizers = _db.Organizers.ToList();

                // Om vi når hit betyder det att inloggningen har Admin-rättigheter
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\n🔓 ÅTKOMST BEVILJAD: Inloggad som Administratör");
                Console.ResetColor();
                Console.WriteLine("Hämtar data direkt från den skyddade tabellen:");

                foreach (var o in organizers)
                {
                    Console.WriteLine($" - ID: {o.OrganizerId} | Namn: {o.Name}");
                }
            }
            catch (Exception ex)
            {
                // Om vi hamnar här har SQL Server blockerat anropet
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n🔒 ÅTKOMST NEKAD: Din roll saknar rättigheter för råtabeller.");
                Console.ResetColor();

                Console.WriteLine("\nFörklaring för läraren:");
                Console.WriteLine("- Applikationen använder just nu 'EventifyAppLogin'.");
                Console.WriteLine("- Denna användare tillhör 'AppRole' som har en 'DENY' på tabellen 'Organizers'.");
                Console.WriteLine("- För att se datan måste man använda vyn eller byta till Admin-inloggningen.");

                // Skriv ut det faktiska felet från SQL Server för att bevisa att det är databasen som nekar
                if (ex.InnerException != null)
                    Console.WriteLine($"\nFelmeddelande från SQL Server: {ex.InnerException.Message}");
            }

            UIHelper.PressAnyKey();
        }
    }
}