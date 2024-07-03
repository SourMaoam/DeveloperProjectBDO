using DeveloperProjectBDO.Application;
using DeveloperProjectBDO.Models;
using DeveloperProjectBDO.Repositories;
using DeveloperProjectBDO.Services;
using Microsoft.EntityFrameworkCore;

namespace DeveloperProjectBDO
{
    class Program
    {
        private static readonly DbContextOptions<ExchangeRateContext> DbContextOptions = new DbContextOptionsBuilder<ExchangeRateContext>()
            .UseSqlite("Data Source=exchangeRates.db")
            .Options;

        private static readonly FixerService FixerService = new FixerService();
        private static readonly ExchangeRateRepository ExchangeRateRepository = new ExchangeRateRepository(DbContextOptions);

        private static async Task Main()
        {
            var app = new ExchangeRateApp(DbContextOptions, FixerService, ExchangeRateRepository);
            await app.Run();
        }
    }
}