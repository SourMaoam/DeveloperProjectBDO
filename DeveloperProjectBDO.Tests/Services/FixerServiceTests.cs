using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DeveloperProjectBDO.Models;
using DeveloperProjectBDO.Services;
using Moq;
using Moq.Protected;
using Xunit;

namespace DeveloperProjectBDO.Tests
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
            Assert.Equal(1.2m, exchangeRates.Rates["USD"]);
            Assert.Equal(0.9m, exchangeRates.Rates["GBP"]);

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
    }
}
