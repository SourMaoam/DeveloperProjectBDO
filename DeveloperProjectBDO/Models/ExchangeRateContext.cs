using Microsoft.EntityFrameworkCore;

namespace DeveloperProjectBDO.Models
{
    public class ExchangeRateContext : DbContext
    {
        public DbSet<ExchangeRate> ExchangeRates { get; set; } = null!;
        public DbSet<ExchangeRateEntry> ExchangeRateEntries { get; set; } = null!;

        public ExchangeRateContext(DbContextOptions<ExchangeRateContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=exchangeRates.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ExchangeRate>()
                .HasMany(e => e.Rates)
                .WithOne(e => e.ExchangeRate)
                .HasForeignKey(e => e.ExchangeRateId);

            modelBuilder.Entity<ExchangeRate>()
                .Property(e => e.BaseCurrency)
                .IsRequired()
                .HasMaxLength(3);

            modelBuilder.Entity<ExchangeRateEntry>()
                .Property(e => e.Currency)
                .IsRequired()
                .HasMaxLength(3);
        }
    }
}