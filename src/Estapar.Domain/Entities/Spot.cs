namespace Estapar.Domain.Entities;

public sealed class Spot
{
    public int Id { get; private set; }                      
    public string SectorCode { get; private set; } = default!;
    public decimal Lat { get; private set; }
    public decimal Lng { get; private set; }
    public bool IsOccupied { get; private set; }

    private Spot() { } // EF
    public Spot(int id, string sectorCode, decimal lat, decimal lng)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(nameof(id), "Id deve ser maior que zero.");

        Id = id;
        SectorCode = NormalizeSectorCode(sectorCode);

        Lat = lat;
        Lng = lng;

        IsOccupied = false;
    }

    public void Occupy() => IsOccupied = true;
    public void Release() => IsOccupied = false;
    public void UpdateLocationAndSector(string sectorCode, decimal lat, decimal lng)
    {
        SectorCode = sectorCode.Trim().ToUpperInvariant();
        Lat = lat;
        Lng = lng;
    }
    private static string NormalizeSectorCode(string sectorCode)
    {
        if (string.IsNullOrWhiteSpace(sectorCode))
            throw new ArgumentException("SectorCode não pode ser vazio.", nameof(sectorCode));

        return sectorCode.Trim().ToUpperInvariant();
    }
}

