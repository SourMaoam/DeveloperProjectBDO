using DeveloperProjectBDO.Models;
using Newtonsoft.Json;

namespace DeveloperProjectBDO.Services
{
    public class FixerService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public FixerService(HttpClient httpClient, string apiKey)
        {
            _httpClient = httpClient;
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey), "API key must not be null");
        }

        public FixerService() : this(new HttpClient(), Environment.GetEnvironmentVariable("FIXER_API_KEY") ?? throw new InvalidOperationException("API key is not set in environment variables"))
        {
        }

        public async Task<ExchangeRate?> GetLatestExchangeRatesAsync()
        {
            var url = $"http://data.fixer.io/api/latest?access_key={_apiKey}";
            try
            {
                var response = await _httpClient.GetStringAsync(url);
                var fixerResponse = JsonConvert.DeserializeObject<FixerApiResponse>(response);

                if (fixerResponse?.Success == true)
                {
                    return new ExchangeRate
                    {
                        BaseCurrency = fixerResponse.Base,
                        Rates = fixerResponse.Rates.Select(rate => new ExchangeRateEntry
                        {
                            Currency = rate.Key,
                            Rate = rate.Value
                        }).ToList()
                    };
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HttpRequestException: {ex.Message}");
            }
            catch (JsonReaderException ex)
            {
                Console.WriteLine($"JsonReaderException: {ex.Message}");
            }

            return null;
        }

        public decimal? GetCrossRate(string fromCurrency, string toCurrency, ExchangeRate exchangeRate)
        {
            if (string.Equals(fromCurrency, toCurrency, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("The source and target currencies are the same.");
                return null;
            }

            var fromRate = exchangeRate.Rates.FirstOrDefault(r => r.Currency == fromCurrency)?.Rate;
            var toRate = exchangeRate.Rates.FirstOrDefault(r => r.Currency == toCurrency)?.Rate;

            if (!fromRate.HasValue)
            {
                Console.WriteLine($"The source currency '{fromCurrency}' was not found.");
                return null;
            }

            if (!toRate.HasValue)
            {
                Console.WriteLine($"The target currency '{toCurrency}' was not found.");
                return null;
            }

            return toRate.Value / fromRate.Value;
        }

    }

    public class FixerApiResponse
    {
        public bool Success { get; set; }
        public string Base { get; set; } = string.Empty;
        public Dictionary<string, decimal> Rates { get; set; } = new Dictionary<string, decimal>();
    }
}
