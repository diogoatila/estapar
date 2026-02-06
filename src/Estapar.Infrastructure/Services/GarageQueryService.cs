using Estapar.Application.Garage;
using Estapar.Application.Garage.Dtos;
using Estapar.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Estapar.Infrastructure.Services;

public sealed class GarageQueryService : IGarageQueryService
{
    private readonly AppDbContext _db;

    public GarageQueryService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<GarageResponseDto> GetGarageAsync(CancellationToken ct)
    {
        var sectors = await _db.Sectors
            .AsNoTracking()
            .OrderBy(x => x.SectorCode)
            .Select(x => new GarageSectorDto
            {
                Sector = x.SectorCode,
                BasePrice = x.BasePrice,
                Max_Capacity = x.MaxCapacity
            })
            .ToListAsync(ct);

        var spots = await _db.Spots
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Select(x => new GarageSpotDto
            {
                Id = x.Id,
                Sector = x.SectorCode,
                Lat = x.Lat,
                Lng = x.Lng
            })
            .ToListAsync(ct);

        return new GarageResponseDto
        {
            Garage = sectors,
            Spots = spots
        };
    }
}