using System.Text.Json.Serialization;

namespace Estapar.Application.Garage.Dtos;

public sealed class GarageSectorDto
{
    [JsonPropertyName("sector")]
    public string Sector { get; set; } = default!;

    [JsonPropertyName("basePrice")]
    public decimal BasePrice { get; set; }

    [JsonPropertyName("max_capacity")]
    public int Max_Capacity { get; set; }
}