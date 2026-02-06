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

            // --- SECTOR A ---
            var sectorA = new GarageSector(
                sectorCode: "A",
                basePrice: 10m,
                maxCapacity: 100
            );

            db.Sectors.Add(sectorA);

            // --- SPOTS 1..100 ---
            // lat/lng podem ser fixos (não influenciam a regra de negócio)
            for (int i = 1; i <= 100; i++)
            {
                db.Spots.Add(new Spot(
                    id: i,
                    sectorCode: "A",
                    lat: -23.561684m,
                    lng: -46.655981m
                ));
            }

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