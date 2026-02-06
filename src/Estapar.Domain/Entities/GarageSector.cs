namespace Estapar.Domain.Entities;

public sealed class GarageSector
{
    public string SectorCode { get; private set; } = default!; // ex: "A"
    public decimal BasePrice { get; private set; }             // ex: 10.00
    public int MaxCapacity { get; private set; }               // ex: 100

    private GarageSector() { } // EF

    public GarageSector(string sectorCode, decimal basePrice, int maxCapacity)
    {
        SectorCode = sectorCode.Trim().ToUpperInvariant();
        BasePrice = basePrice;
        MaxCapacity = maxCapacity;
    }
    public void UpdatePricing(decimal basePrice, int maxCapacity)
    {
        BasePrice = basePrice;
        MaxCapacity = maxCapacity;
    }
}

