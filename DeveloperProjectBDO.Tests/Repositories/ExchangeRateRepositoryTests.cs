using System.Collections.Generic;
using DeveloperProjectBDO.Models;
using DeveloperProjectBDO.Repositories;
using Xunit;

namespace DeveloperProjectBDO.Tests
{
    public class ExchangeRateRepositoryTests
    {
        [Fact]
        public void AddOrUpdate_ShouldStoreExchangeRate()
        {
            // Arrange
            var repository = new ExchangeRateRepository();
            var exchangeRate = new ExchangeRate
            {
                BaseCurrency = "EUR",
                Rates = new Dictionary<string, decimal>
                {
                    { "USD", 1.2m },
                    { "GBP", 0.9m }
                }
            };

            // Act
            repository.AddOrUpdate(exchangeRate);
            var storedExchangeRate = repository.GetExchangeRate();

            // Assert
            Assert.NotNull(storedExchangeRate);
            Assert.Equal("EUR", storedExchangeRate.BaseCurrency);
            Assert.Equal(1.2m, storedExchangeRate.Rates["USD"]);
            Assert.Equal(0.9m, storedExchangeRate.Rates["GBP"]);
        }

        [Fact]
        public void AddOrUpdate_ShouldUpdateExchangeRate()
        {
            // Arrange
            var repository = new ExchangeRateRepository();
            var initialExchangeRate = new ExchangeRate
            {
                BaseCurrency = "EUR",
                Rates = new Dictionary<string, decimal>
                {
                    { "USD", 1.2m },
                    { "GBP", 0.9m }
                }
            };

            var updatedExchangeRate = new ExchangeRate
            {
                BaseCurrency = "EUR",
                Rates = new Dictionary<string, decimal>
                {
                    { "USD", 1.3m },
                    { "GBP", 0.85m }
                }
            };

            // Act
            repository.AddOrUpdate(initialExchangeRate);
            var storedExchangeRateBeforeUpdate = repository.GetExchangeRate();

            repository.AddOrUpdate(updatedExchangeRate);
            var storedExchangeRateAfterUpdate = repository.GetExchangeRate();

            // Assert
            Assert.NotNull(storedExchangeRateBeforeUpdate);
            Assert.Equal(1.2m, storedExchangeRateBeforeUpdate.Rates["USD"]);
            Assert.Equal(0.9m, storedExchangeRateBeforeUpdate.Rates["GBP"]);

            Assert.NotNull(storedExchangeRateAfterUpdate);
            Assert.Equal(1.3m, storedExchangeRateAfterUpdate.Rates["USD"]);
            Assert.Equal(0.85m, storedExchangeRateAfterUpdate.Rates["GBP"]);
        }

        [Fact]
        public void GetExchangeRate_ShouldReturnNullIfNoExchangeRate()
        {
            // Arrange
            var repository = new ExchangeRateRepository();

            // Act
            var exchangeRate = repository.GetExchangeRate();

            // Assert
            Assert.Null(exchangeRate);
        }

        [Fact]
        public void GetExchangeRate_ShouldReturnStoredExchangeRate()
        {
            // Arrange
            var repository = new ExchangeRateRepository();
            var exchangeRate = new ExchangeRate
            {
                BaseCurrency = "EUR",
                Rates = new Dictionary<string, decimal>
                {
                    { "USD", 1.2m },
                    { "GBP", 0.9m }
                }
            };

            // Act
            repository.AddOrUpdate(exchangeRate);
            var storedExchangeRate = repository.GetExchangeRate();

            // Assert
            Assert.NotNull(storedExchangeRate);
            Assert.Equal("EUR", storedExchangeRate.BaseCurrency);
            Assert.Equal(1.2m, storedExchangeRate.Rates["USD"]);
            Assert.Equal(0.9m, storedExchangeRate.Rates["GBP"]);
        }
    }
}
