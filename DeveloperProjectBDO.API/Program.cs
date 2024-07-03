using DeveloperProjectBDO.Models;
using DeveloperProjectBDO.Repositories;
using DeveloperProjectBDO.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Define the database file path
string homePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
string dbFilePath = Path.Combine(homePath, "AppData", "Local", "DeveloperProjectBDO", "exchangeRates.db");
EnsureDatabaseDirectoryExists(dbFilePath);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<ExchangeRateContext>(options =>
    options.UseSqlite($"Data Source={dbFilePath}"));

builder.Services.AddScoped<ExchangeRateRepository>();
builder.Services.AddScoped<FixerService>();

var app = builder.Build();

// Ensure database schema is created
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ExchangeRateContext>();
    dbContext.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

static void EnsureDatabaseDirectoryExists(string dbFilePath)
{
    string? directoryPath = Path.GetDirectoryName(dbFilePath);
    if (directoryPath != null && !Directory.Exists(directoryPath))
    {
        Directory.CreateDirectory(directoryPath);
    }
}