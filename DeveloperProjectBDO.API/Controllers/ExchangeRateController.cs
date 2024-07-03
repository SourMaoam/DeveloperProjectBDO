using DeveloperProjectBDO.Repositories;
using DeveloperProjectBDO.Services;
using Microsoft.AspNetCore.Mvc;

namespace DeveloperProjectBDO.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExchangeRateController : ControllerBase
    {
        private readonly FixerService _fixerService;
        private readonly ExchangeRateRepository _exchangeRateRepository;

        public ExchangeRateController(FixerService fixerService, ExchangeRateRepository exchangeRateRepository)
        {
            _fixerService = fixerService;
            _exchangeRateRepository = exchangeRateRepository;
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateExchangeRates()
        {
            try
            {
                var exchangeRates = await _fixerService.GetLatestExchangeRatesAsync();
                if (exchangeRates != null)
                {
                    _exchangeRateRepository.AddOrUpdate(exchangeRates);
                    return Ok("Exchange rates updated.");
                }
                else
                {
                    return StatusCode(500, "Failed to fetch exchange rates from the API.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}