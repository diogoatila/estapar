using System.Net.Http.Json;
using Estapar.Application.Garage;
using Estapar.Application.Garage.Dtos;

namespace Estapar.Infrastructure.Clients;

public sealed class GarageClient : IGarageClient
{
    private readonly HttpClient _httpClient;

    public GarageClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<GarageResponseDto> GetGarageAsync(CancellationToken ct)
    {
        var response = await _httpClient.GetAsync("/garage", ct);

        response.EnsureSuccessStatusCode();

        var dto = await response.Content.ReadFromJsonAsync<GarageResponseDto>(cancellationToken: ct);

        if (dto is null)
            throw new InvalidOperationException("Simulator returned an empty response for GET /garage.");

        return dto;
    }
}