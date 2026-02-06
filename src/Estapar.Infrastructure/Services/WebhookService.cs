using System.Data;
using Estapar.Application.Webhook;
using Estapar.Application.Webhook.Dtos;
using Estapar.Domain.Entities;
using Estapar.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Estapar.Infrastructure.Services;

public sealed class WebhookService : IWebhookService
{
    private readonly AppDbContext _db;
    private readonly ILogger<WebhookService> _logger;

    public WebhookService(AppDbContext db, ILogger<WebhookService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task ProcessAsync(WebhookEventRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.LicensePlate))
            throw new ArgumentException("license_plate is required.");

        if (string.IsNullOrWhiteSpace(request.EventType))
            throw new ArgumentException("event_type is required.");

        var eventType = WebhookEventTypeParser.Parse(request.EventType);

        switch (eventType)
        {
            case WebhookEventType.ENTRY:
                if (request.EntryTime is null)
                    throw new ArgumentException("entry_time is required for ENTRY.");
                await HandleEntryAsync(request.LicensePlate, request.EntryTime.Value, ct);
                break;

            case WebhookEventType.PARKED:
                await HandleParkedAsync(request.LicensePlate, request.Lat, request.Lng, ct);
                break;

            case WebhookEventType.EXIT:
                if (request.ExitTime is null)
                    throw new ArgumentException("exit_time is required for EXIT.");
                await HandleExitAsync(request.LicensePlate, request.ExitTime.Value, ct);
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }


    // ENTRY
    private async Task HandleEntryAsync(string licensePlate, DateTime entryTime, CancellationToken ct)
    {
        licensePlate = licensePlate.Trim().ToUpperInvariant();
        entryTime = EnsureUtc(entryTime);

        await using var tx = await _db.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);

        // Idempotência: se já existe sessão ativa, retorna OK
        var activeSession = await _db.ParkingSessions
            .FirstOrDefaultAsync(x => x.LicensePlate == licensePlate && x.ExitTime == null, ct);

        if (activeSession is not null)
        {
            await tx.CommitAsync(ct);
            return;
        }

        // escolher um setor com vaga disponível
        // pegar o primeiro setor que tenha spot livre.
        var sectors = await _db.Sectors
            .AsNoTracking()
            .OrderBy(x => x.SectorCode)
            .ToListAsync(ct);

        if (sectors.Count == 0)
            throw new InvalidOperationException("Garage sectors not loaded. Did bootstrap run?");

        foreach (var sector in sectors)
        {
            var totalCapacity = sector.MaxCapacity;

            // spots cadastrados desse setor
            var totalSpots = await _db.Spots.CountAsync(x => x.SectorCode == sector.SectorCode, ct);

            // Se o simulador mandar max_capacity maior que spots cadastrados,
            // o certo é respeitar MaxCapacity como limitador.
            var capacity = Math.Min(totalCapacity, totalSpots);

            if (capacity <= 0)
                continue;

            var occupied = await _db.Spots.CountAsync(x => x.SectorCode == sector.SectorCode && x.IsOccupied, ct);

            if (occupied >= capacity)
                continue; // setor cheio

            // lotação ANTES de ocupar
            var occupancyRate = (decimal)occupied / capacity;

            // preço dinâmico na ENTRADA
            var pricePerHour = ApplyDynamicPricing(sector.BasePrice, occupancyRate);

            // pega uma vaga livre
            var spot = await _db.Spots
                .FirstOrDefaultAsync(x => x.SectorCode == sector.SectorCode && !x.IsOccupied, ct);

            if (spot is null)
                continue;

            // ocupar vaga
            spot.Occupy();

            // criar sessão
            var session = new ParkingSession(
                licensePlate: licensePlate,
                sectorCode: sector.SectorCode,
                spotId: spot.Id,
                entryTime: entryTime,
                pricePerHourApplied: pricePerHour
            );

            _db.ParkingSessions.Add(session);

            await _db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);

            return;
        }

        // Se chegou aqui, nenhum setor tinha vaga
        throw new ParkingFullException("Garage is full. ENTRY blocked.");
    }


    // PARKED
    private async Task HandleParkedAsync(string licensePlate, decimal? lat, decimal? lng, CancellationToken ct)
    {
        licensePlate = licensePlate.Trim().ToUpperInvariant();

        var activeSession = await _db.ParkingSessions
            .FirstOrDefaultAsync(x => x.LicensePlate == licensePlate && x.ExitTime == null, ct);

        if (activeSession is null)
            return; // idempotente / tolerante

        activeSession.MarkParked();

        // lat/lng são opcionais. Você pode ignorar.
        // Se quiser atualizar a vaga com a última posição:
        if (lat is not null && lng is not null)
        {
            var spot = await _db.Spots.FirstOrDefaultAsync(x => x.Id == activeSession.SpotId, ct);
            if (spot is not null)
            {
                spot.UpdateLocationAndSector(spot.SectorCode, lat.Value, lng.Value);
            }
        }

        await _db.SaveChangesAsync(ct);
    }

     // EXIT
    private async Task HandleExitAsync(string licensePlate, DateTime exitTime, CancellationToken ct)
    {
        licensePlate = licensePlate.Trim().ToUpperInvariant();
        exitTime = EnsureUtc(exitTime);

        await using var tx = await _db.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);

        var activeSession = await _db.ParkingSessions
            .FirstOrDefaultAsync(x => x.LicensePlate == licensePlate && x.ExitTime == null, ct);

        if (activeSession is null)
        {
            await tx.CommitAsync(ct);
            return; // idempotente
        }

        // libera spot
        var spot = await _db.Spots.FirstOrDefaultAsync(x => x.Id == activeSession.SpotId, ct);
        if (spot is not null)
        {
            spot.Release();
        }

        // calcula cobrança
        var amount = CalculateCharge(activeSession.EntryTime, exitTime, activeSession.PricePerHourApplied);

        activeSession.RegisterExit(exitTime, amount);

        await _db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
    }


    // Helpers
    private static decimal ApplyDynamicPricing(decimal basePrice, decimal occupancyRate)
    {
        // occupancyRate: 0.00 -> 1.00

        if (occupancyRate < 0.25m)
            return RoundMoney(basePrice * 0.90m);

        if (occupancyRate <= 0.50m)
            return RoundMoney(basePrice);

        if (occupancyRate <= 0.75m)
            return RoundMoney(basePrice * 1.10m);

        // <= 1.00
        return RoundMoney(basePrice * 1.25m);
    }

    private static decimal CalculateCharge(DateTime entryUtc, DateTime exitUtc, decimal pricePerHourApplied)
    {
        if (exitUtc <= entryUtc)
            return 0m;

        var totalMinutes = (exitUtc - entryUtc).TotalMinutes;

        if (totalMinutes <= 30)
            return 0m;

        // após 30 minutos: cobra por hora, arredondando para cima
        var billableMinutes = totalMinutes - 30;
        var hours = (int)Math.Ceiling(billableMinutes / 60.0);

        return RoundMoney(hours * pricePerHourApplied);
    }

    private static decimal RoundMoney(decimal value)
        => Math.Round(value, 2, MidpointRounding.AwayFromZero);

    private static DateTime EnsureUtc(DateTime dt)
        => dt.Kind == DateTimeKind.Utc ? dt : DateTime.SpecifyKind(dt, DateTimeKind.Utc);
}