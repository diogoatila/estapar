using Estapar.Application.Garage;
using Estapar.Domain.Entities;
using Estapar.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Estapar.Infrastructure.HostedServices;

public sealed class GarageBootstrapHostedService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<GarageBootstrapHostedService> _logger;

    public GarageBootstrapHostedService(
        IServiceScopeFactory scopeFactory,
        ILogger<GarageBootstrapHostedService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Bootstrapping garage data...");

        // Pequeno retry caso o simulador demore a subir
        const int maxAttempts = 5;

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                await using var scope = _scopeFactory.CreateAsyncScope();

                var client = scope.ServiceProvider.GetRequiredService<IGarageClient>();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var dto = await client.GetGarageAsync(stoppingToken);

                await UpsertGarageAsync(db, dto, stoppingToken);

                _logger.LogInformation("Garage bootstrap finished successfully.");
                return;
            }
            catch (Exception ex) when (attempt < maxAttempts)
            {
                _logger.LogWarning(ex, "Bootstrap attempt {Attempt}/{MaxAttempts} failed. Retrying...", attempt, maxAttempts);
                await Task.Delay(TimeSpan.FromSeconds(2 * attempt), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Garage bootstrap failed permanently.");
                throw;
            }
        }
    }

    private static async Task UpsertGarageAsync(
        AppDbContext db,
        Estapar.Application.Garage.Dtos.GarageResponseDto dto,
        CancellationToken ct)
    {
        // Setores (upsert)
        foreach (var sectorDto in dto.Garage)
        {
            var code = sectorDto.Sector.Trim().ToUpperInvariant();

            var existingSector = await db.Sectors
                .FirstOrDefaultAsync(x => x.SectorCode == code, ct);

            if (existingSector is null)
            {
                db.Sectors.Add(new GarageSector(code, sectorDto.BasePrice, sectorDto.Max_Capacity));
            }
            else
            {
                // Atualização (como não temos setters públicos, o ideal é criar métodos no domínio)
                // Para manter simples no teste, você pode trocar para setters internos,
                // OU criar métodos no domínio. Vou te mostrar o jeito sênior:

                UpdateSector(existingSector, sectorDto.BasePrice, sectorDto.Max_Capacity);
            }
        }

        // Spots (upsert)
        foreach (var spotDto in dto.Spots)
        {
            var code = spotDto.Sector.Trim().ToUpperInvariant();

            var existingSpot = await db.Spots
                .FirstOrDefaultAsync(x => x.Id == spotDto.Id, ct);

            if (existingSpot is null)
            {
                db.Spots.Add(new Spot(spotDto.Id, code, spotDto.Lat, spotDto.Lng));
            }
            else
            {
                UpdateSpot(existingSpot, code, spotDto.Lat, spotDto.Lng);
            }
        }

        await db.SaveChangesAsync(ct);
    }

    private static void UpdateSector(GarageSector sector, decimal basePrice, int maxCapacity)
    {

        sector.UpdatePricing(basePrice, maxCapacity);
    }

    private static void UpdateSpot(Spot spot, string sectorCode, decimal lat, decimal lng)
    {
        spot.UpdateLocationAndSector(sectorCode, lat, lng);
    }
}