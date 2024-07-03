using System;
using System.Threading;
using System.Threading.Tasks;
using DeveloperProjectBDO.Repositories;
using DeveloperProjectBDO.Services;



namespace DeveloperProjectBDO
{
    class Program
    {
        private static readonly FixerService _fixerService = new FixerService();
        private static readonly ExchangeRateRepository _exchangeRateRepository = new ExchangeRateRepository();

        static async Task Main(string[] args)
        {
            // Schedule daily task to fetch exchange rates
            var cancellationTokenSource = new CancellationTokenSource();
            _ = Task.Run(() => ScheduleDailyTask(FetchExchangeRates, TimeSpan.FromDays(1), cancellationTokenSource.Token));

            while (true)
            {
                Console.WriteLine("Choose an option:");
                Console.WriteLine("1. Get Latest Exchange Rates");
                Console.WriteLine("2. Exit");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        GetLatestExchangeRates();
                        break;
                    case "2":
                        cancellationTokenSource.Cancel();
                        return;
                }
            }
        }

        // Fetch and store the latest exchange rates
        static async Task FetchExchangeRates()
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

        // Display the stored exchange rates
        static void GetLatestExchangeRates()
        {
            var exchangeRates = _exchangeRateRepository.GetExchangeRate();
            if (exchangeRates != null)
            {
                Console.WriteLine($"Base Currency: {exchangeRates.BaseCurrency}");
                foreach (var rate in exchangeRates.Rates)
                {
                    Console.WriteLine($"{rate.Key}: {rate.Value}");
                }
            }
            else
            {
                Console.WriteLine("No exchange rates available.");
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
