using Estapar.Application.Garage.Dtos;

namespace Estapar.Application.Garage;
public interface IGarageClient
{
    Task<GarageResponseDto> GetGarageAsync(CancellationToken ct);
}