using DeveloperProjectBDO.Application;
using DeveloperProjectBDO.Models;
using DeveloperProjectBDO.Repositories;
using DeveloperProjectBDO.Services;
using Microsoft.EntityFrameworkCore;

namespace DeveloperProjectBDO
{
    class Program
    {
        public static readonly string HomePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        public static readonly string DbFilePath = Path.Combine(HomePath, "AppData", "Local", "DeveloperProjectBDO", "exchangeRates.db");

        private static readonly DbContextOptions<ExchangeRateContext> DbContextOptions = new DbContextOptionsBuilder<ExchangeRateContext>()
            .UseSqlite($"Data Source={DbFilePath}")
            .Options;

        private static readonly FixerService FixerService = new FixerService();
        private static readonly ExchangeRateRepository ExchangeRateRepository = new ExchangeRateRepository(DbContextOptions);

        private static async Task Main()
        {
            EnsureDatabaseDirectoryExists();
            EnsureDatabaseCreated();

            var app = new ExchangeRateApp(DbContextOptions, FixerService, ExchangeRateRepository);
            await app.Run();
        }

        private static void EnsureDatabaseCreated()
        {
            using (var context = new ExchangeRateContext(DbContextOptions))
            {
                context.Database.EnsureCreated();
            }
        }

        private static void EnsureDatabaseDirectoryExists()
        {
            string? directoryPath = Path.GetDirectoryName(DbFilePath);
            if (directoryPath != null && !Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }
    }
}