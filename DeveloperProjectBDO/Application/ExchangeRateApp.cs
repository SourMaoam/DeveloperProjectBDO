using DeveloperProjectBDO.Repositories;
using DeveloperProjectBDO.Services;
using DeveloperProjectBDO.Models;
using Microsoft.EntityFrameworkCore;

namespace DeveloperProjectBDO.Application
{
    public class ExchangeRateApp
    {
        private readonly DbContextOptions<ExchangeRateContext> _dbContextOptions;
        private readonly FixerService _fixerService;
        private readonly ExchangeRateRepository _exchangeRateRepository;

        public ExchangeRateApp(DbContextOptions<ExchangeRateContext> dbContextOptions, FixerService fixerService, ExchangeRateRepository exchangeRateRepository)
        {
            _dbContextOptions = dbContextOptions;
            _fixerService = fixerService;
            _exchangeRateRepository = exchangeRateRepository;
        }

        public async Task Run()
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
                Console.WriteLine("4. Help");
                Console.WriteLine("5. Exit");

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
                        ShowHelp();
                        break;
                    case "5":
                        cancellationTokenSource.Cancel();
                        return;
                }
            }
        }

        private async Task ScheduleDailyTask(Func<Task> task, TimeSpan interval, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    await task();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error occurred during scheduled task: {ex.Message}");
                }
                await Task.Delay(interval, token);
            }
        }

        private async Task FetchExchangeRates()
        {
            var exchangeRates = await _fixerService.GetLatestExchangeRatesAsync();
            if (exchangeRates != null)
            {
                _exchangeRateRepository.AddOrUpdate(exchangeRates);
                Console.WriteLine("Exchange rates updated.");
            }
            else
            {
                Console.WriteLine("Failed to fetch exchange rates.");
            }
        }

        private async Task GetLatestExchangeRates()
        {
            var exchangeRates = await Task.Run(() => _exchangeRateRepository.GetExchangeRate());
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

        private void GetCrossRate()
        {
            var exchangeRates = _exchangeRateRepository.GetExchangeRate();
            if (exchangeRates != null)
            {
                Console.Write("Enter the source currency (e.g., GBP): ");
                var fromCurrency = Console.ReadLine()?.ToUpper();
                if (string.IsNullOrEmpty(fromCurrency) || !exchangeRates.Rates.Any(r => r.Currency == fromCurrency))
                {
                    Console.WriteLine("Invalid source currency.");
                    return;
                }

                Console.Write("Enter the target currency (e.g., USD): ");
                var toCurrency = Console.ReadLine()?.ToUpper();
                if (string.IsNullOrEmpty(toCurrency) || !exchangeRates.Rates.Any(r => r.Currency == toCurrency))
                {
                    Console.WriteLine("Invalid target currency.");
                    return;
                }

                var crossRate = _fixerService.GetCrossRate(fromCurrency, toCurrency, exchangeRates);
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

        private void ViewDatabaseContents()
        {
            using (var context = new ExchangeRateContext(_dbContextOptions))
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

        private void ShowHelp()
        {
            Console.WriteLine("Available options:");
            Console.WriteLine("1. Get Latest Exchange Rates: Fetches and displays the latest exchange rates.");
            Console.WriteLine("2. Get Cross Rate: Calculates and displays the cross rate between two specified currencies.");
            Console.WriteLine("3. View Database Contents: Displays the stored exchange rates from the database.");
            Console.WriteLine("5. Exit: Exits the application.");
        }
    }
}