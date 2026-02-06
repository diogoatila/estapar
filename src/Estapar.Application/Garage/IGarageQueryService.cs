using Estapar.Application.Garage.Dtos;

namespace Estapar.Application.Garage;
public interface IGarageQueryService
{
    Task<GarageResponseDto> GetGarageAsync(CancellationToken ct);
}