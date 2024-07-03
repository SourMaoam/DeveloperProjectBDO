using DeveloperProjectBDO.Models;

namespace DeveloperProjectBDO.Repositories
{
    public class ExchangeRateRepository
    {
        private ExchangeRate _exchangeRate;

        public ExchangeRate GetExchangeRate() => _exchangeRate;

        public void AddOrUpdate(ExchangeRate exchangeRate) => _exchangeRate = exchangeRate;
    }
}