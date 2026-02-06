using Estapar.Domain.Entities;
using Estapar.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Estapar.Infrastructure.Bootstrap;

public sealed class GarageSeedHostedService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<GarageSeedHostedService> _logger;

    public GarageSeedHostedService(
        IServiceScopeFactory scopeFactory,
        ILogger<GarageSeedHostedService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // garante migrations aplicadas
            await db.Database.MigrateAsync(stoppingToken);

            // se já tem dados, não faz nada
            var hasSectors = await db.Sectors.AnyAsync(stoppingToken);
            var hasSpots = await db.Spots.AnyAsync(stoppingToken);

            if (hasSectors && hasSpots)
            {
                _logger.LogInformation("Garage seed skipped (already initialized).");
                return;
            }

            _logger.LogInformation("Seeding garage initial data...");

            //  SECTORS 
            db.Sectors.Add(new GarageSector("A", 10m, 5));
            db.Sectors.Add(new GarageSector("B", 12m, 5));
            db.Sectors.Add(new GarageSector("C", 15m, 5));

            //  SPOTS A (1..5) 
            for (int i = 1; i <= 5; i++)
                db.Spots.Add(new Spot(i, "A", -23.561684m, -46.655981m));

            //  SPOTS B (6..10) 
            for (int i = 6; i <= 10; i++)
                db.Spots.Add(new Spot(i, "B", -23.561684m, -46.655981m));

            // SPOTS C (11..15) 
            for (int i = 11; i <= 15; i++)
                db.Spots.Add(new Spot(i, "C", -23.561684m, -46.655981m));
            await db.SaveChangesAsync(stoppingToken);

            _logger.LogInformation("Garage seed completed successfully.");
        }
        catch (Exception ex)
        {
            // Não derruba a API
            _logger.LogError(ex, "Garage seed failed. API will keep running.");
        }
    }
}