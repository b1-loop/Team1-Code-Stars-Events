using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Team1_Code_Stars_Events.Models;
using SQLTeam.Services;
using SQLTeam.UI;
using Team1_Code_Stars_Events.Helpers; // Se till att denna using finns för UIHelper

namespace SQLTeam
{
    internal class Program
    {
        static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;

            try
            {
                // 1. Konfiguration
                var config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();

                // 2. Database Setup
                var connectionString = config.GetConnectionString("EventifyDb");
                var options = new DbContextOptionsBuilder<AppDbContext>()
                    .UseSqlServer(connectionString)
                    .Options;

                using var db = new AppDbContext(options);

                // --- NY FELSÖKNINGSKONTROLL ---
                // Vi testar anslutningen direkt. Om detta misslyckas hoppar vi till catch.
                if (!db.Database.CanConnect())
                {
                    throw new Exception("Kunde inte ansluta till databasen. Kontrollera Mixed Mode och Lösenord.");
                }

                // 3. Lager-initiering
                var dbService = new DbService(db);
                var menuManager = new MenuManager(dbService);

                // 4. Kör
                menuManager.Run();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n--- KRITISKT ANSLUTNINGSFEL ---");

                // Detta visar det specifika felet från SQL Server (t.ex. "Login failed")
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Detaljer: {ex.InnerException.Message}");
                }
                else
                {
                    Console.WriteLine($"Fel: {ex.Message}");
                }

                Console.ResetColor();
                Console.WriteLine("\nTryck på valfri tangent för att avsluta...");
                Console.ReadKey();
            }
        }
    }
}