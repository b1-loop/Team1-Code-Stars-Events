using Microsoft.EntityFrameworkCore;
using Team1_Code_Stars_Events.Models;
using System.Collections.Generic;
using System.Linq;
using System;

namespace SQLTeam.Services
{
    // Denna klass hanterar all direkt kommunikation med databasen (Data Access Layer)
    public class DbService
    {
        private readonly AppDbContext _db;

        // Konstruktor som tar emot databaskontexten (Dependency Injection)
        public DbService(AppDbContext db)
        {
            _db = db;
        }

        // --- KUNDER ---

        // Hämtar alla kunder sorterade efter efternamn
        public List<Customer> GetAllCustomers() =>
            _db.Customers.OrderBy(c => c.LastName).ToList();

        // Skapar och sparar en ny kund i databasen
        public void AddCustomer(string fName, string lName, string email)
        {
            _db.Customers.Add(new Customer { FirstName = fName, LastName = lName, Email = email });
            _db.SaveChanges(); // Skickar ändringarna till SQL Server
        }

        // --- EVENTS ---

        // Hämtar alla events och inkluderar information om lokalen (Venue) via en Join
        public List<Event> GetAllEvents() =>
            _db.Events.Include(e => e.Venue).OrderBy(e => e.StartDate).ToList();

        // --- BILJETTER ---

        // Skapar en ny biljett och kopplar ihop Kund-ID med Event-ID
        public void CreateTicket(int cId, int eId, string type, decimal price)
        {
            _db.Tickets.Add(new Ticket
            {
                CustomerId = cId,
                EventId = eId,
                Price = price,
                Type = type,
                PurchaseDate = DateTime.Now
            });
            _db.SaveChanges();
        }

        // Hämtar alla biljetter och inkluderar relaterad data för både kund och event
        public List<Ticket> GetAllTickets() =>
            _db.Tickets.Include(t => t.Customer).Include(t => t.Event).ToList();

        // Söker upp en specifik biljett baserat på dess Primärnyckel (ID)
        public Ticket GetTicketById(int id) => _db.Tickets.Find(id);

        // Tar bort en biljett från databasen
        public void DeleteTicket(Ticket ticket)
        {
            _db.Tickets.Remove(ticket);
            _db.SaveChanges();
        }

        // Sparar manuella ändringar (används t.ex. vid uppdatering av objekt)
        public void SaveChanges() => _db.SaveChanges();

        // --- RAPPORTER & STATISTIK ---

        // Hämtar de 5 kunder som köpt flest biljetter
        public dynamic GetTopCustomersReport()
        {
            return _db.Customers
                .Select(c => new { Name = c.FirstName + " " + c.LastName, Count = c.Tickets.Count })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToList();
        }

        // Summerar de totala biljettintäkterna för varje event
        public dynamic GetRevenueReport()
        {
            return _db.Events
                .Select(e => new { e.Title, Total = e.Tickets.Sum(t => t.Price) })
                .ToList();
        }

        // Hämtar alla kunder inklusive deras biljetter och de events biljetterna hör till
        public List<Customer> GetCustomersWithTickets()
        {
            return _db.Customers
                .Include(c => c.Tickets)
                .ThenInclude(t => t.Event)
                .OrderBy(c => c.LastName)
                .ToList();
        }
    }
}