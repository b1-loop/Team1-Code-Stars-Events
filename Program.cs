using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Team1_Code_Stars_Events.Models;
using SQLTeam.Services;
using SQLTeam.UI;

namespace SQLTeam
{
    internal class Program
    {
        static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;

            // 1. Ladda konfigurationen först
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            string connectionString = "";
            string currentRole = "";

            // 2. Visa menyn för val av användare
            while (true)
            {
                Console.Clear();
                Console.WriteLine("========================================");
                Console.WriteLine("   VÄLJ INLOGGNINGSPROFIL");
                Console.WriteLine("========================================");
                Console.WriteLine("1) Standardanvändare (Begränsad - AppRole)");
                Console.WriteLine("2) Administratör (Full åtkomst - AdminRole)");
                Console.WriteLine("0) Avsluta");
                Console.Write("\nVal: ");

                var roleChoice = Console.ReadLine();

                if (roleChoice == "1")
                {
                    // Hämtar den begränsade strängen
                    connectionString = config.GetConnectionString("EventifyDb");
                    currentRole = "Standardanvändare";
                    break;
                }
                else if (roleChoice == "2")
                {
                    // Hämtar admin-strängen
                    connectionString = config.GetConnectionString("EventifyAdmin");
                    currentRole = "Administratör";
                    break;
                }
                else if (roleChoice == "0") return;
            }

            try
            {
                // 3. Database Setup med den valda strängen
                var options = new DbContextOptionsBuilder<AppDbContext>()
                    .UseSqlServer(connectionString)
                    .Options;

                using var db = new AppDbContext(options);

                // Kontrollera anslutningen direkt
                if (!db.Database.CanConnect())
                {
                    throw new Exception($"Kunde inte ansluta som {currentRole}.");
                }

                // 4. Starta programmet
                Console.WriteLine($"\nInloggad som: {currentRole}. Startar systemet...");
                Thread.Sleep(1000); // Pausa kort för att bekräfta valet

                var dbService = new DbService(db);
                var menuManager = new MenuManager(dbService, db);

                menuManager.Run();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n--- ANSLUTNINGSFEL ---");
                Console.WriteLine($"Fel vid inloggning som {currentRole}: {ex.Message}");
                Console.ResetColor();
                Console.WriteLine("\nTryck på valfri tangent för att försöka igen...");
                Console.ReadKey();
                Main(); // Starta om för att välja roll igen
            }
        }
    }
}