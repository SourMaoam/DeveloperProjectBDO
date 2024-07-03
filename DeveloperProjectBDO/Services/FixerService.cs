using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
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
            _apiKey = apiKey;
        }

        public FixerService() : this(new HttpClient(), Environment.GetEnvironmentVariable("FIXER_API_KEY"))
        {
        }

        public async Task<ExchangeRate> GetLatestExchangeRatesAsync()
        {
            var url = $"http://data.fixer.io/api/latest?access_key={_apiKey}";
            try
            {
                var response = await _httpClient.GetStringAsync(url);
                var fixerResponse = JsonConvert.DeserializeObject<FixerApiResponse>(response);

                if (fixerResponse.Success)
                {
                    return new ExchangeRate
                    {
                        BaseCurrency = fixerResponse.Base,
                        Rates = fixerResponse.Rates
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
    }

    public class FixerApiResponse
    {
        public bool Success { get; set; }
        public string Base { get; set; }
        public Dictionary<string, decimal> Rates { get; set; }
    }
}