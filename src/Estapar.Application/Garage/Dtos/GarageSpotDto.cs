using System.Text.Json.Serialization;

namespace Estapar.Application.Garage.Dtos;

public sealed class GarageSpotDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("sector")]
    public string Sector { get; set; } = default!;

    [JsonPropertyName("lat")]
    public decimal Lat { get; set; }

    [JsonPropertyName("lng")]
    public decimal Lng { get; set; }
}