
namespace DeveloperProjectBDO.Models
{
    public class ExchangeRate
    {
        public int Id { get; set; }
        public string BaseCurrency { get; set; } = string.Empty;
        public List<ExchangeRateEntry> Rates { get; set; } = new List<ExchangeRateEntry>();
    }

    public class ExchangeRateEntry
    {
        public int Id { get; set; }
        public string Currency { get; set; } = string.Empty;
        public decimal Rate { get; set; }
        public int ExchangeRateId { get; set; }
        public ExchangeRate ExchangeRate { get; set; } = null!;
    }
}