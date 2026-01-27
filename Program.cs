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

            // 1. Konfiguration
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            // 2. Database Setup
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(config.GetConnectionString("EventifyDb"))
                .Options;

            using var db = new AppDbContext(options);

            // 3. Lager-initiering
            var dbService = new DbService(db);
            var menuManager = new MenuManager(dbService);

            // 4. Kör
            menuManager.Run();
        }
    }
}