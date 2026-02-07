namespace Estapar.Domain.Entities;

public sealed class GarageSector
{
    public string SectorCode { get; private set; } = default!;
    public decimal BasePrice { get; private set; }
    public int MaxCapacity { get; private set; }

    private GarageSector() { } // EF

    public GarageSector(string sectorCode, decimal basePrice, int maxCapacity)
    {
        SectorCode = NormalizeSectorCode(sectorCode);
        Validate(basePrice, maxCapacity);

        BasePrice = basePrice;
        MaxCapacity = maxCapacity;
    }

    public void UpdatePricing(decimal basePrice, int maxCapacity)
    {
        Validate(basePrice, maxCapacity);

        BasePrice = basePrice;
        MaxCapacity = maxCapacity;
    }

    //Validações
    private static string NormalizeSectorCode(string sectorCode)
    {
        if (string.IsNullOrWhiteSpace(sectorCode))
            throw new ArgumentException("SectorCode não pode ser vazio.", nameof(sectorCode));

        return sectorCode.Trim().ToUpperInvariant();
    }

    private static void Validate(decimal basePrice, int maxCapacity)
    {
        if (basePrice < 0)
            throw new ArgumentOutOfRangeException(nameof(basePrice), "BasePrice não pode ser negativo.");

        if (maxCapacity <= 0)
            throw new ArgumentOutOfRangeException(nameof(maxCapacity), "MaxCapacity deve ser maior que zero.");
    }
}