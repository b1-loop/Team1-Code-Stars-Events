using Microsoft.EntityFrameworkCore;
using SQLTeam.Services;
using System;
using System.Linq;
using Team1_Code_Stars_Events.Helpers;
using Team1_Code_Stars_Events.Models;

namespace SQLTeam.UI
{
    // Denna klass ansvarar för allt användargränssnitt och menyflöden
    public class MenuManager
    {
        private readonly DbService _service;

        // Konstruktor: Vi kräver en fungerande DbService för att starta menyn
        public MenuManager(DbService service)
        {
            _service = service;
        }

        // Huvudloopen som håller igång programmet tills användaren väljer "0"
        public void Run()
        {
            while (true)
            {
                UIHelper.ShowHeader("🎟️  EVENTIFY ADMIN SYSTEM 2026");
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
                    if (choice == "0") break; // Bryter loopen och stänger programmet
                    HandleChoice(choice);
                }
                catch (Exception ex)
                {
                    // Fångar upp oväntade fel så att inte hela programmet kraschar
                    UIHelper.ShowError($"Ett tekniskt fel uppstod: {ex.Message}");
                    UIHelper.PressAnyKey();
                }
            }
        }

        // Navigerar användaren till rätt metod baserat på menyval
        private void HandleChoice(string choice)
        {
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
            var customers = _service.GetAllCustomers(); // Hämtar data från servicen

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
            UIHelper.ShowHeader("📅 EVENEMANGSÖVERSIKT");
            var events = _service.GetAllEvents();

            if (!events.Any()) Console.WriteLine("Inga events hittades.");
            else
            {
                foreach (var e in events)
                    Console.WriteLine($" 🎭 [ID: {e.EventId,-3}] {e.Title,-20} | 📍 {e.Venue?.Name}");
            }
            UIHelper.PressAnyKey();
        }

        private void CreateTicket()
        {
            UIHelper.ShowHeader("🎫 NYTT BILJETTKÖP");

            // Visar hjälpdata för att användaren ska veta vilka ID:n som finns
            Console.WriteLine("Alla registrerade kunder:");
            foreach (var c in _service.GetAllCustomers())
                Console.WriteLine($" {c.CustomerId}: {c.FirstName} {c.LastName}");

            Console.WriteLine("\nAlla tillgängliga events:");
            foreach (var e in _service.GetAllEvents())
                Console.WriteLine($" {e.EventId}: {e.Title}");

            // Validerar indata med UIHelper
            int cId = UIHelper.GetValidInt("\nAnge Kund-ID: ");
            int eId = UIHelper.GetValidInt("Ange Event-ID: ");

            Console.WriteLine("\nVälj typ: [1] Regular (500kr) [2] Student (300kr) [3] VIP (1200kr) [4] Backstage (2500kr)");
            string type = "";
            decimal price = 0;
            var choice = Console.ReadLine();

            // Mappar menyval till faktiska värden
            switch (choice)
            {
                case "1": type = "Regular"; price = 500m; break;
                case "2": type = "Student"; price = 300m; break;
                case "3": type = "VIP"; price = 1200m; break;
                case "4": type = "Backstage"; price = 2500m; break;
                default: UIHelper.ShowError("Ogiltigt val."); return;
            }

            // Skickar informationen till servicen för lagring
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

            // Visar befintliga biljetter
            foreach (var t in tickets)
                Console.WriteLine($" [ID: {t.TicketId,-3}] {t.Customer?.FirstName} {t.Customer?.LastName,-15} -> {t.Event?.Title}");

            int tId = UIHelper.GetValidInt("\nAnge ID för biljetten du vill ändra: ");
            var ticket = _service.GetTicketById(tId); // Hämtar objektet via servicen

            if (ticket == null) { UIHelper.ShowError("Biljetten hittades inte."); UIHelper.PressAnyKey(); return; }

            Console.WriteLine($"\nVald biljett: {ticket.Type} ({ticket.Price:C})");
            Console.WriteLine("Ny typ: [1] Regular [2] Student [3] VIP [4] Backstage");

            var choice = Console.ReadLine();
            // Uppdaterar objektets värden (State Change)
            if (choice == "1") { ticket.Type = "Regular"; ticket.Price = 500m; }
            else if (choice == "2") { ticket.Type = "Student"; ticket.Price = 300m; }
            else if (choice == "3") { ticket.Type = "VIP"; ticket.Price = 1200m; }
            else if (choice == "4") { ticket.Type = "Backstage"; ticket.Price = 2500m; }
            else { UIHelper.ShowError("Ogiltigt val."); UIHelper.PressAnyKey(); return; }

            // Ber servicen spara de ändrade värdena i databasen
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
                // Bekräftelse för att undvika oavsiktlig radering
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
            Console.WriteLine("2) 💰 Totala intäkter per Event");
            var choice = Console.ReadLine();

            if (choice == "1")
            {
                // Hämtar data som redan är färdigprocessad i DbService
                var report = _service.GetTopCustomersReport();
                foreach (var r in report) Console.WriteLine($" ⭐ {r.Name,-20} : {r.Count} st");
            }
            else if (choice == "2")
            {
                var report = _service.GetRevenueReport();
                foreach (var r in report) Console.WriteLine($" 💵 {r.Title,-20} : {r.Total:C}");
            }
            UIHelper.PressAnyKey();
        }

        private void ListCustomersWithTickets()
        {
            UIHelper.ShowHeader("🎟️  KUNDER OCH DERAS BOKADE EVENTS");
            var customers = _service.GetCustomersWithTickets();

            foreach (var c in customers)
            {
                Console.WriteLine($"👤 {c.FirstName} {c.LastName} (ID: {c.CustomerId})");
                if (!c.Tickets.Any()) Console.WriteLine("   🚫 Inga biljetter bokade.");
                else
                {
                    // Loopar igenom kundens biljetter och deras relaterade event
                    foreach (var t in c.Tickets)
                        Console.WriteLine($"   🎫 {t.Event?.Title,-20} | Typ: {t.Type,-10} | Pris: {t.Price:C}");
                }
                Console.WriteLine("---------------------------------");
            }
            UIHelper.PressAnyKey();
        }
    }
}