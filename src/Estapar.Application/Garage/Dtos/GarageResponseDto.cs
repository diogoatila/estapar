namespace Estapar.Application.Garage.Dtos;

public sealed class GarageResponseDto
{
    public List<GarageSectorDto> Garage { get; set; } = new();
    public List<GarageSpotDto> Spots { get; set; } = new();
}