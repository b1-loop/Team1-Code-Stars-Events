using Microsoft.EntityFrameworkCore; // Bibliotek för att hantera databaskopplingen (ORM)
using Microsoft.Extensions.Configuration; // Bibliotek för att läsa inställningar (appsettings.json)
using System;
using System.IO;
using System.Linq; // Bibliotek för LINQ-frågor (filtrering, sortering)
using System.Text;
using Team1_Code_Stars_Events.Models; // Inkluderar projektets databasmodeller

namespace SQLTeam
{
    internal class Program
    {
        static void Main()
        {
            // Ställer in terminalen för att kunna visa UTF8-tecken som emojis
            Console.OutputEncoding = Encoding.UTF8;

            // Laddar konfiguration för att nå databasen via appsettings.json
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // Hämtar anslutningssträngen för SQL Server
            var connectionString = config.GetConnectionString("EventifyDb");

            // Kontrollerar att anslutningssträngen faktiskt existerar
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                ShowError("Connection string saknas i appsettings.json.");
                Console.ReadKey();
                return;
            }

            // Konfigurerar databaskontexten (AppDbContext) med SQL Server-inställningar
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(connectionString)
                .Options;

            // 'using' ser till att databaskopplingen stängs automatiskt när programmet avslutas
            using var db = new AppDbContext(options);

            // Huvudloop för menysystemet
            while (true)
            {
                Console.Clear();
                ShowHeader("🎟️  EVENTIFY ADMIN SYSTEM 2026");
                Console.WriteLine("1) 👥 Lista ALLA Kunder");
                Console.WriteLine("2) 📅 Lista ALLA Events");
                Console.WriteLine("3) 🎫 Köp biljett (Skapa relation)");
                Console.WriteLine("4) 👤 Registrera ny kund (Transaktion)");
                Console.WriteLine("5) ⚙️  Uppdatera biljettyp & pris");
                Console.WriteLine("6) 🗑️  Radera biljett");
                Console.WriteLine("7) 📊 Rapporter & Statistik");
                Console.WriteLine("8) 🎫 Visa Kunder & deras Biljetter");
                Console.WriteLine("0) ❌ Avsluta");
                Console.Write("\nVal: ");

                var choice = Console.ReadLine()?.Trim();

                try
                {
                    // Switch-sats för att navigera mellan programmets funktioner
                    switch (choice)
                    {
                        case "1": ListCustomers(db); break;
                        case "2": ListEvents(db); break;
                        case "3": CreateTicket(db); break;
                        case "4": AddCustomer(db); break;
                        case "5": UpdateTicketType(db); break;
                        case "6": DeleteTicket(db); break;
                        case "7": ShowReports(db); break;
                        case "8": ListCustomersWithTickets(db); break;
                        case "0": return;
                        default:
                            ShowError("Felaktigt val, försök igen.");
                            Console.ReadKey();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    // Fångar upp oväntade fel för att förhindra krascher
                    ShowError($"Ett tekniskt fel uppstod: {ex.Message}");
                    Console.ReadKey();
                }
            }
        }

        // --- DATABASE OPERATIONS (Metoder som pratar med SQL) ---

        static void ListCustomers(AppDbContext db)
        {
            Console.Clear();
            ShowHeader("👥 KUNDREGISTER");
            // Hämtar alla kunder från databasen och sorterar dem på efternamn med LINQ
            var customers = db.Customers.OrderBy(c => c.LastName).ToList();
            if (!customers.Any()) Console.WriteLine("Inga kunder hittades.");
            else
            {
                foreach (var c in customers)
                    Console.WriteLine($" 👤 [ID: {c.CustomerId,-3}] {c.FirstName} {c.LastName,-15} | 📧 {c.Email}");
            }
            Console.WriteLine("\nTryck på valfri tangent...");
            Console.ReadKey();
        }

        static void ListEvents(AppDbContext db)
        {
            Console.Clear();
            ShowHeader("📅 EVENEMANGSÖVERSIKT");
            // Hämtar alla events och använder .Include för att även hämta data från tabellen Venues (lokaler)
            var events = db.Events.Include(e => e.Venue).OrderBy(e => e.StartDate).ToList();
            if (!events.Any()) Console.WriteLine("Inga events hittades.");
            else
            {
                foreach (var e in events)
                    Console.WriteLine($" 🎭 [ID: {e.EventId,-3}] {e.Title,-20} | 📍 {e.Venue?.Name}");
            }
            Console.WriteLine("\nTryck på valfri tangent...");
            Console.ReadKey();
        }

        static void CreateTicket(AppDbContext db)
        {
            Console.Clear();
            ShowHeader("🎫 NYTT BILJETTKÖP");

            // --- FULLSTÄNDIGA LISTOR (Visar all data för att hjälpa användaren) ---
            Console.WriteLine("Alla registrerade kunder:");
            var allCustomers = db.Customers.OrderBy(c => c.FirstName).ToList();
            foreach (var c in allCustomers)
                Console.WriteLine($" {c.CustomerId}: {c.FirstName} {c.LastName}");

            Console.WriteLine("\nAlla tillgängliga events:");
            var allEvents = db.Events.OrderBy(e => e.Title).ToList();
            foreach (var e in allEvents)
                Console.WriteLine($" {e.EventId}: {e.Title}");

            Console.WriteLine("---------------------------------\n");

            // Hämtar ID från användaren för att skapa en koppling mellan kund och event
            int cId = GetValidInt("Ange Kund-ID: ");
            int eId = GetValidInt("Ange Event-ID: ");

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
                default: ShowError("Ogiltigt val."); return;
            }

            // Skapar ett nytt biljettobjekt och lägger till det i kontexten
            db.Tickets.Add(new Ticket { CustomerId = cId, EventId = eId, Price = price, Type = type, PurchaseDate = DateTime.Now });
            // Kör SaveChanges() för att skicka det faktiska INSERT-kommandot till SQL
            db.SaveChanges();
            ShowSuccess("Biljett registrerad!");
            Console.ReadKey();
        }

        static void AddCustomer(AppDbContext db)
        {
            Console.Clear();
            ShowHeader("👤 REGISTRERA NY KUND");
            string fName = GetValidString("Förnamn: ");
            string lName = GetValidString("Efternamn: ");
            string email = GetValidString("Email: ");

            // Lägger till en ny kundrad i tabellen Customers
            db.Customers.Add(new Customer { FirstName = fName, LastName = lName, Email = email });
            db.SaveChanges();
            ShowSuccess("Kunden har sparats!");
            Console.ReadKey();
        }

        static void UpdateTicketType(AppDbContext db)
        {
            Console.Clear();
            ShowHeader("⚙️  UPPDATERA BILJETT");

            // Hämtar hela listan på biljetter inkl. kund och event för att visa ID
            var ticketList = db.Tickets.Include(t => t.Customer).Include(t => t.Event).ToList();
            if (!ticketList.Any()) { Console.WriteLine("Inga biljetter hittades."); Console.ReadKey(); return; }

            foreach (var t in ticketList)
                Console.WriteLine($" [ID: {t.TicketId,-3}] {t.Customer?.FirstName} {t.Customer?.LastName,-15} -> {t.Event?.Title}");

            int tId = GetValidInt("\nAnge ID för biljetten du vill ändra: ");
            // Söker upp den specifika biljetten i databasen
            var ticket = db.Tickets.Find(tId);

            if (ticket == null) { ShowError("Biljetten hittades inte."); Console.ReadKey(); return; }

            Console.WriteLine($"\nVald biljett: {ticket.Type} ({ticket.Price:C})");
            Console.WriteLine("Ny typ: [1] Regular [2] Student [3] VIP [4] Backstage");

            var choice = Console.ReadLine();
            // Ändrar objektets egenskaper (EF Core håller reda på att objektet har blivit 'Dirty'/ändrat)
            if (choice == "1") { ticket.Type = "Regular"; ticket.Price = 500m; }
            else if (choice == "2") { ticket.Type = "Student"; ticket.Price = 300m; }
            else if (choice == "3") { ticket.Type = "VIP"; ticket.Price = 1200m; }
            else if (choice == "4") { ticket.Type = "Backstage"; ticket.Price = 2500m; }
            else { ShowError("Ogiltigt val."); Console.ReadKey(); return; }

            // Uppdaterar raden i SQL
            db.SaveChanges();
            ShowSuccess("Biljetten har uppdaterats!");
            Console.ReadKey();
        }

        static void DeleteTicket(AppDbContext db)
        {
            Console.Clear();
            ShowHeader("🗑️  RADERA BILJETT");

            // Visar alla biljetter i systemet
            var ticketList = db.Tickets.Include(t => t.Customer).Include(t => t.Event).ToList();

            if (!ticketList.Any()) { Console.WriteLine("Inga biljetter att radera."); Console.ReadKey(); return; }

            foreach (var t in ticketList)
                Console.WriteLine($" [ID: {t.TicketId,-3}] {t.Customer?.FirstName} {t.Customer?.LastName,-15} | Event: {t.Event?.Title}");

            int tId = GetValidInt("\nAnge ID för biljetten som ska raderas: ");
            // Hämtar biljetten och inkluderar namn för att kunna visa en bekräftelse
            var ticket = db.Tickets.Include(t => t.Customer).Include(t => t.Event).FirstOrDefault(t => t.TicketId == tId);

            if (ticket != null)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"\n⚠️  VARNING: Radera biljetten för {ticket.Customer?.FirstName} till {ticket.Event?.Title}?");
                Console.Write("Bekräfta med (J/N): ");
                Console.ResetColor();

                if (Console.ReadLine()?.ToUpper() == "J")
                {
                    // Tar bort biljetten från kontexten
                    db.Tickets.Remove(ticket);
                    // Verkställer raderingen i databasen
                    db.SaveChanges();
                    ShowSuccess("Biljetten har raderats.");
                }
            }
            else ShowError("ID hittades inte.");

            Console.ReadKey();
        }

        static void ShowReports(AppDbContext db)
        {
            Console.Clear();
            ShowHeader("📊 RAPPORTSENTRAL");
            Console.WriteLine("1) 🏆 Top 5 kunder (Flest köp)");
            Console.WriteLine("2) 💰 Totala intäkter per Event");
            var choice = Console.ReadLine();
            if (choice == "1")
            {
                // Avancerad LINQ-fråga: Grupperar och räknar biljetter per kund
                var report = db.Customers.Select(c => new { Name = c.FirstName + " " + c.LastName, Count = c.Tickets.Count })
                    .OrderByDescending(x => x.Count).Take(5).ToList();
                foreach (var r in report) Console.WriteLine($" ⭐ {r.Name,-20} : {r.Count} st");
            }
            else if (choice == "2")
            {
                // Avancerad LINQ-fråga: Summerar priser för alla biljetter per event
                var report = db.Events.Select(e => new { e.Title, Total = e.Tickets.Sum(t => t.Price) }).ToList();
                foreach (var r in report) Console.WriteLine($" 💵 {r.Title,-20} : {r.Total:C}");
            }
            Console.ReadKey();
        }

        static void ListCustomersWithTickets(AppDbContext db)
        {
            Console.Clear();
            ShowHeader("🎟️  KUNDER OCH DERAS BOKADE EVENTS");
            // Hämtar kunder och kedjar ihop Include/ThenInclude för att nå djupt ner i datamodellen (Kund -> Biljetter -> Event)
            var customers = db.Customers.Include(c => c.Tickets).ThenInclude(t => t.Event).OrderBy(c => c.LastName).ToList();
            foreach (var c in customers)
            {
                Console.WriteLine($"👤 {c.FirstName} {c.LastName} (ID: {c.CustomerId})");
                if (!c.Tickets.Any()) Console.WriteLine("   🚫 Inga biljetter bokade.");
                else
                {
                    foreach (var t in c.Tickets)
                        Console.WriteLine($"   🎫 {t.Event?.Title,-20} | Typ: {t.Type,-10} | Pris: {t.Price:C}");
                }
                Console.WriteLine("---------------------------------");
            }
            Console.ReadKey();
        }

        // --- HELPERS (Verktygsmetoder för indata och design) ---

        // Metod för att säkerställa att en sträng inte är tom
        static string GetValidString(string prompt) { string input; do { Console.Write(prompt); input = Console.ReadLine()?.Trim(); } while (string.IsNullOrWhiteSpace(input)); return input; }

        // Metod för att säkerställa att användaren skriver in en siffra (int)
        static int GetValidInt(string prompt) { int result; while (true) { Console.Write(prompt); if (int.TryParse(Console.ReadLine(), out result)) return result; ShowError("Ange ett heltal."); } }

        // Metoder för att formatera rubriker, lyckade meddelanden och felmeddelanden med färger
        static void ShowHeader(string text) { Console.ForegroundColor = ConsoleColor.Cyan; Console.WriteLine($"\n{text}"); Console.ResetColor(); Console.WriteLine("---------------------------------"); }
        static void ShowSuccess(string msg) { Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine($"\n✅ {msg}"); Console.ResetColor(); }
        static void ShowError(string msg) { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine($"\n❌ {msg}"); Console.ResetColor(); }
    }
}