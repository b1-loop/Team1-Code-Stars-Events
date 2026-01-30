using Microsoft.EntityFrameworkCore;
using Team1_Code_Stars_Events.Models;
using System.Collections.Generic;
using System.Linq;
using System;

namespace SQLTeam.Services
{
    public class DbService
    {
        private readonly AppDbContext _db;

        public DbService(AppDbContext db)
        {
            _db = db;
        }

        // --- KUNDER ---

        // Hämtar alla kunder sorterade på efternamn
        public List<Customer> GetAllCustomers() =>
            _db.Customers.OrderBy(c => c.LastName).ToList();

        // Lägger till en ny kund i databasen
        public void AddCustomer(string fName, string lName, string email)
        {
            _db.Customers.Add(new Customer { FirstName = fName, LastName = lName, Email = email });
            _db.SaveChanges();
        }

        // --- EVENTS ---

        // Hämtar data från vyn (säkerställer att vi inte bryter mot DENY-regler i SQL)
        public List<VwUpcomingEvent> GetUpcomingEvents() =>
            _db.VwUpcomingEvents.OrderBy(e => e.StartDate).ToList();

        // --- BILJETTER ---

        // Skapar en ny biljett-koppling mellan kund och event
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

        // Hämtar alla biljetter och inkluderar information om vem som köpt och till vilket event
        public List<Ticket> GetAllTickets() =>
            _db.Tickets.Include(t => t.Customer).Include(t => t.Event).ToList();

        // Hittar en specifik biljett via dess ID
        public Ticket GetTicketById(int id) => _db.Tickets.Find(id);

        // Tar bort en biljett permanent
        public void DeleteTicket(Ticket ticket)
        {
            _db.Tickets.Remove(ticket);
            _db.SaveChanges();
        }

        // --- RAPPORTER & STATISTIK ---

        // Använder statistik-vyn för att få fram försäljningssiffror
        public List<VwEventStatistic> GetEventStatisticsReport() =>
            _db.VwEventStatistics.OrderByDescending(s => s.TotalRevenue).ToList();

        // En enkel rapport för att se vilka kunder som handlat mest
        public dynamic GetTopCustomersReport()
        {
            return _db.Customers
                .Select(c => new { Name = c.FirstName + " " + c.LastName, Count = c.Tickets.Count })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToList();
        }

        // --- RELATIONER (HÄR ÄR DEN SAKNADE METODEN) ---

        // Hämtar kunder, deras biljetter och namnet på eventet för varje biljett
        public List<Customer> GetCustomersWithTickets()
        {
            // Vi använder .Include för att hämta biljetter 
            // och .ThenInclude för att gå ett steg djupare och hämta själva eventet
            return _db.Customers
                .Include(c => c.Tickets)
                    .ThenInclude(t => t.Event)
                .OrderBy(c => c.LastName)
                .ToList();
        }

        // Sparar manuella ändringar som görs på objekt
        public void SaveChanges() => _db.SaveChanges();

        // Returnerar en lista av biljetter med en specifik typ
        public List<Ticket> GetTicketsByType(string type) =>
        _db.Tickets
            .Include(t => t.Customer)
            .Include(t => t.Event)
            .Where(t => t.Type == type)
            .ToList();
    }
}