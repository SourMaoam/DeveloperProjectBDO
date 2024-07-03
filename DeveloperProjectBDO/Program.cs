using DeveloperProjectBDO.Repositories;
using DeveloperProjectBDO.Services;
using DeveloperProjectBDO.Models;
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

        static async Task Main()
        {
            // Schedule daily task to fetch exchange rates
            var cancellationTokenSource = new CancellationTokenSource();
            _ = Task.Run(() => ScheduleDailyTask(FetchExchangeRates, TimeSpan.FromDays(1), cancellationTokenSource.Token));

            while (true)
            {
                Console.WriteLine("Choose an option:");
                Console.WriteLine("1. Get Latest Exchange Rates");
                Console.WriteLine("2. Get Cross Rate (e.g., GBP to USD)");
                Console.WriteLine("3. View Database Contents");
                Console.WriteLine("4. Exit");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await GetLatestExchangeRates();
                        break;
                    case "2":
                        GetCrossRate();
                        break;
                    case "3":
                        ViewDatabaseContents();
                        break;
                    case "4":
                        cancellationTokenSource.Cancel();
                        return;
                }
            }
        }

        // Fetch and store the latest exchange rates
        static async Task FetchExchangeRates()
        {
            var exchangeRates = await FixerService.GetLatestExchangeRatesAsync();
            if (exchangeRates != null)
            {
                ExchangeRateRepository.AddOrUpdate(exchangeRates);
                Console.WriteLine("Exchange rates updated.");
            }
            else
            {
                Console.WriteLine("Failed to fetch exchange rates.");
            }
        }

        // Display the stored exchange rates
        static async Task GetLatestExchangeRates()
        {
            var exchangeRates = await Task.Run(() => ExchangeRateRepository.GetExchangeRate());
            if (exchangeRates != null)
            {
                Console.WriteLine($"Base Currency: {exchangeRates.BaseCurrency}");
                foreach (var rate in exchangeRates.Rates)
                {
                    Console.WriteLine($"{rate.Currency}: {rate.Rate}");
                }
            }
            else
            {
                Console.WriteLine("No exchange rates available.");
            }
        }


        // Get cross rate between two currencies
        static void GetCrossRate()
        {
            var exchangeRates = ExchangeRateRepository.GetExchangeRate();
            if (exchangeRates != null)
            {
                Console.Write("Enter the source currency (e.g., GBP): ");
                var fromCurrency = Console.ReadLine()?.ToUpper();
                if (string.IsNullOrEmpty(fromCurrency))
                {
                    Console.WriteLine("Invalid source currency.");
                    return;
                }

                Console.Write("Enter the target currency (e.g., USD): ");
                var toCurrency = Console.ReadLine()?.ToUpper();
                if (string.IsNullOrEmpty(toCurrency))
                {
                    Console.WriteLine("Invalid target currency.");
                    return;
                }

                var crossRate = FixerService.GetCrossRate(fromCurrency, toCurrency, exchangeRates);
                if (crossRate.HasValue)
                {
                    Console.WriteLine($"{fromCurrency} to {toCurrency}: {crossRate.Value}");
                }
                else
                {
                    Console.WriteLine("Could not calculate cross rate.");
                }
            }
            else
            {
                Console.WriteLine("No exchange rates available.");
            }
        }

        // View the contents of the database
        static void ViewDatabaseContents()
        {
            using (var context = new ExchangeRateContext(DbContextOptions))
            {
                var exchangeRates = context.ExchangeRates.Include(e => e.Rates).ToList();

                if (exchangeRates.Any())
                {
                    foreach (var exchangeRate in exchangeRates)
                    {
                        Console.WriteLine($"Base Currency: {exchangeRate.BaseCurrency}");
                        foreach (var rate in exchangeRate.Rates)
                        {
                            Console.WriteLine($"{rate.Currency}: {rate.Rate}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No exchange rates available.");
                }
            }
        }

        // Schedule a task to run at a specified interval
        static async Task ScheduleDailyTask(Func<Task> task, TimeSpan interval, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await task();
                await Task.Delay(interval, token);
            }
        }
    }
}
