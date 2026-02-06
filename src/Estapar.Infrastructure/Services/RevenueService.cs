using Estapar.Application.Revenue;
using Estapar.Application.Revenue.Dtos;
using Estapar.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Estapar.Infrastructure.Services;

public sealed class RevenueService : IRevenueService
{
    private readonly AppDbContext _db;

    public RevenueService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<RevenueResponseDto> GetRevenueAsync(DateOnly date, string sectorCode, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(sectorCode))
            throw new ArgumentException("sector is required.");

        sectorCode = sectorCode.Trim().ToUpperInvariant();

        // range do dia em UTC (00:00 até 23:59:59)
        var start = date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var end = date.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);

        var amount = await _db.ParkingSessions
            .AsNoTracking()//performance
            .Where(x =>
                x.SectorCode == sectorCode &&
                x.ExitTime != null &&
                x.ExitTime >= start &&
                x.ExitTime <= end)
            .SumAsync(x => x.AmountCharged, ct);

        return new RevenueResponseDto
        {
            Amount = Math.Round(amount, 2, MidpointRounding.AwayFromZero),
            Currency = "BRL",
            Timestamp = DateTime.UtcNow
        };
    }
}