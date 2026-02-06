using Estapar.Application.Revenue.Dtos;

namespace Estapar.Application.Revenue;
public interface IRevenueService
{
    Task<RevenueResponseDto> GetRevenueAsync(DateOnly date, string sectorCode, CancellationToken ct);
}