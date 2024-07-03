using DeveloperProjectBDO.Models;
using DeveloperProjectBDO.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DeveloperProjectBDO.Tests
{
    public class ExchangeRateRepositoryTests
    {
        private DbContextOptions<ExchangeRateContext> CreateNewContextOptions()
        {
            return new DbContextOptionsBuilder<ExchangeRateContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryDbForTesting")
                .Options;
        }

        private void ClearDatabase(ExchangeRateContext context)
        {
            context.ExchangeRates.RemoveRange(context.ExchangeRates);
            context.ExchangeRateEntries.RemoveRange(context.ExchangeRateEntries);
            context.SaveChanges();
        }

        [Fact]
        public void AddOrUpdate_ShouldStoreExchangeRate()
        {
            var options = CreateNewContextOptions();
            using (var context = new ExchangeRateContext(options))
            {
                ClearDatabase(context);
            }

            var repository = new ExchangeRateRepository(options);
            var exchangeRate = new ExchangeRate
            {
                BaseCurrency = "EUR",
                Rates = new List<ExchangeRateEntry>
                {
                    new ExchangeRateEntry { Currency = "USD", Rate = 1.2m },
                    new ExchangeRateEntry { Currency = "GBP", Rate = 0.9m }
                }
            };

            repository.AddOrUpdate(exchangeRate);
            var storedExchangeRate = repository.GetExchangeRate();

            Assert.NotNull(storedExchangeRate);
            Assert.Equal("EUR", storedExchangeRate.BaseCurrency);
            Assert.Equal(2, storedExchangeRate.Rates.Count);
            Assert.Contains(storedExchangeRate.Rates, r => r.Currency == "USD" && r.Rate == 1.2m);
            Assert.Contains(storedExchangeRate.Rates, r => r.Currency == "GBP" && r.Rate == 0.9m);
        }

        [Fact]
        public void AddOrUpdate_ShouldUpdateExchangeRate()
        {
            var options = CreateNewContextOptions();
            using (var context = new ExchangeRateContext(options))
            {
                ClearDatabase(context);
            }

            var repository = new ExchangeRateRepository(options);
            var initialExchangeRate = new ExchangeRate
            {
                BaseCurrency = "EUR",
                Rates = new List<ExchangeRateEntry>
                {
                    new ExchangeRateEntry { Currency = "USD", Rate = 1.2m },
                    new ExchangeRateEntry { Currency = "GBP", Rate = 0.9m }
                }
            };

            var updatedExchangeRate = new ExchangeRate
            {
                BaseCurrency = "EUR",
                Rates = new List<ExchangeRateEntry>
                {
                    new ExchangeRateEntry { Currency = "USD", Rate = 1.3m },
                    new ExchangeRateEntry { Currency = "GBP", Rate = 0.85m }
                }
            };

            repository.AddOrUpdate(initialExchangeRate);
            var storedExchangeRateBeforeUpdate = repository.GetExchangeRate();

            repository.AddOrUpdate(updatedExchangeRate);
            var storedExchangeRateAfterUpdate = repository.GetExchangeRate();

            Assert.NotNull(storedExchangeRateBeforeUpdate);
            Assert.Equal(2, storedExchangeRateBeforeUpdate.Rates.Count);
            Assert.Contains(storedExchangeRateBeforeUpdate.Rates, r => r.Currency == "USD" && r.Rate == 1.2m);
            Assert.Contains(storedExchangeRateBeforeUpdate.Rates, r => r.Currency == "GBP" && r.Rate == 0.9m);

            Assert.NotNull(storedExchangeRateAfterUpdate);
            Assert.Equal(2, storedExchangeRateAfterUpdate.Rates.Count);
            Assert.Contains(storedExchangeRateAfterUpdate.Rates, r => r.Currency == "USD" && r.Rate == 1.3m);
            Assert.Contains(storedExchangeRateAfterUpdate.Rates, r => r.Currency == "GBP" && r.Rate == 0.85m);
        }

        [Fact]
        public void GetExchangeRate_ShouldReturnNullIfNoExchangeRate()
        {
            var options = CreateNewContextOptions();
            using (var context = new ExchangeRateContext(options))
            {
                ClearDatabase(context);
            }

            var repository = new ExchangeRateRepository(options);
            var exchangeRate = repository.GetExchangeRate();

            Assert.Null(exchangeRate);
        }

        [Fact]
        public void GetExchangeRate_ShouldReturnStoredExchangeRate()
        {
            var options = CreateNewContextOptions();
            using (var context = new ExchangeRateContext(options))
            {
                ClearDatabase(context);
            }

            var repository = new ExchangeRateRepository(options);
            var exchangeRate = new ExchangeRate
            {
                BaseCurrency = "EUR",
                Rates = new List<ExchangeRateEntry>
                {
                    new ExchangeRateEntry { Currency = "USD", Rate = 1.2m },
                    new ExchangeRateEntry { Currency = "GBP", Rate = 0.9m }
                }
            };

            repository.AddOrUpdate(exchangeRate);
            var storedExchangeRate = repository.GetExchangeRate();

            Assert.NotNull(storedExchangeRate);
            Assert.Equal("EUR", storedExchangeRate.BaseCurrency);
            Assert.Equal(2, storedExchangeRate.Rates.Count);
            Assert.Contains(storedExchangeRate.Rates, r => r.Currency == "USD" && r.Rate == 1.2m);
            Assert.Contains(storedExchangeRate.Rates, r => r.Currency == "GBP" && r.Rate == 0.9m);
        }
    }
}
