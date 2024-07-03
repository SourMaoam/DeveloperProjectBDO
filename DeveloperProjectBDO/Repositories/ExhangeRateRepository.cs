using DeveloperProjectBDO.Models;
using Microsoft.EntityFrameworkCore;

namespace DeveloperProjectBDO.Repositories
{
    public class ExchangeRateRepository
    {
        private readonly DbContextOptions<ExchangeRateContext> _options;

        public ExchangeRateRepository(DbContextOptions<ExchangeRateContext> options)
        {
            _options = options;
        }

        public ExchangeRate? GetExchangeRate()
        {
            using (var context = new ExchangeRateContext(_options))
            {
                return context.ExchangeRates
                    .Include(e => e.Rates)
                    .FirstOrDefault();
            }
        }

        public void AddOrUpdate(ExchangeRate exchangeRate)
        {
            using (var context = new ExchangeRateContext(_options))
            {
                var existingRate = context.ExchangeRates
                    .Include(e => e.Rates)
                    .FirstOrDefault();

                if (existingRate != null)
                {
                    existingRate.BaseCurrency = exchangeRate.BaseCurrency;

                    // Remove old rates
                    context.ExchangeRateEntries.RemoveRange(existingRate.Rates);
                    context.SaveChanges();

                    // Add new rates
                    existingRate.Rates = exchangeRate.Rates;
                    context.ExchangeRateEntries.AddRange(existingRate.Rates);
                }
                else
                {
                    context.ExchangeRates.Add(exchangeRate);
                }

                context.SaveChanges();
            }
        }
    }
}