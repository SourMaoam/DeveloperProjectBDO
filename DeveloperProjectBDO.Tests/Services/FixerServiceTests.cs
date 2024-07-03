using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DeveloperProjectBDO.Models;
using DeveloperProjectBDO.Services;
using Moq;
using Moq.Protected;
using Xunit;

namespace DeveloperProjectBDO.Tests.Services
{
    public class FixerServiceTests
    {
        [Fact]
        public async Task GetLatestExchangeRatesAsync_ShouldReturnExchangeRates()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent("{\"success\":true, \"base\":\"EUR\", \"rates\":{\"USD\":1.2,\"GBP\":0.9}}"),
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            var fixerService = new FixerService(httpClient, "dummy_api_key");

            // Act
            var exchangeRates = await fixerService.GetLatestExchangeRatesAsync();

            // Assert
            Assert.NotNull(exchangeRates);
            Assert.Equal("EUR", exchangeRates.BaseCurrency);
            Assert.Contains(exchangeRates.Rates, r => r.Currency == "USD" && r.Rate == 1.2m);
            Assert.Contains(exchangeRates.Rates, r => r.Currency == "GBP" && r.Rate == 0.9m);

            // Verify the HTTP request was made once
            mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task GetLatestExchangeRatesAsync_ShouldHandleError()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            var fixerService = new FixerService(httpClient, "dummy_api_key");

            // Act
            var exchangeRates = await fixerService.GetLatestExchangeRatesAsync();

            // Assert
            Assert.Null(exchangeRates);

            // Verify the HTTP request was made once
            mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task GetLatestExchangeRatesAsync_ShouldReturnNullForInvalidJson()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent("Invalid JSON"),
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            var fixerService = new FixerService(httpClient, "dummy_api_key");

            // Act
            var exchangeRates = await fixerService.GetLatestExchangeRatesAsync();

            // Assert
            Assert.Null(exchangeRates);

            // Verify the HTTP request was made once
            mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public void GetCrossRate_ShouldReturnCorrectCrossRate()
        {
            // Arrange
            var exchangeRate = new ExchangeRate
            {
                BaseCurrency = "EUR",
                Rates = new List<ExchangeRateEntry>
                {
                    new ExchangeRateEntry { Currency = "USD", Rate = 1.2m },
                    new ExchangeRateEntry { Currency = "GBP", Rate = 0.9m }
                }
            };

            // Since the constructor requiring an API key is not being used here, we need to mock it correctly.
            var httpClient = new HttpClient();
            var fixerService = new FixerService(httpClient, "dummy_api_key");

            // Act
            var crossRate = fixerService.GetCrossRate("GBP", "USD", exchangeRate);

            // Assert
            Assert.NotNull(crossRate);
            Assert.Equal(1.3333m, Math.Round(crossRate.Value, 4));
        }
    }
}
