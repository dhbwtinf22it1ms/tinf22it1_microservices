using Dhbw.ThesisManager.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", optional: true)
    .Build();

var services = new ServiceCollection();

// Konfiguriere den DB Context
services.AddDbContext<ThesisManagerDbContext>(options =>
    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

var serviceProvider = services.BuildServiceProvider();

try
{
    Console.WriteLine("Starte Datenbank-Migration...");
    
    using var scope = serviceProvider.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ThesisManagerDbContext>();
    
    await dbContext.Database.MigrateAsync();
    
    Console.WriteLine("Datenbank-Migration erfolgreich abgeschlossen!");
}
catch (Exception ex)
{
    Console.WriteLine($"Fehler bei der Migration: {ex.Message}");
    Environment.Exit(1);
}
