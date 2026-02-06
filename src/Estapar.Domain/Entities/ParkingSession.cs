using Estapar.Domain.Enums;

namespace Estapar.Domain.Entities;

public sealed class ParkingSession
{
    public Guid Id { get; private set; }

    public string LicensePlate { get; private set; } = default!;

    public string SectorCode { get; private set; } = default!;
    public int SpotId { get; private set; }

    public DateTime EntryTime { get; private set; }
    public DateTime? ExitTime { get; private set; }

    public ParkingStatus Status { get; private set; }

    public decimal PricePerHourApplied { get; private set; }
    public decimal AmountCharged { get; private set; }

    private ParkingSession() { } // EF

    public ParkingSession(
        string licensePlate,
        string sectorCode,
        int spotId,
        DateTime entryTime,
        decimal pricePerHourApplied)
    {
        Id = Guid.NewGuid();
        LicensePlate = NormalizePlate(licensePlate);
        SectorCode = sectorCode.Trim().ToUpperInvariant();
        SpotId = spotId;

        EntryTime = EnsureUtc(entryTime);
        ExitTime = null;

        Status = ParkingStatus.Entered;

        PricePerHourApplied = pricePerHourApplied;
        AmountCharged = 0m;
    }

    public void MarkParked()
    {
        if (Status == ParkingStatus.Exited)
            return;

        Status = ParkingStatus.Parked;
    }

    public void RegisterExit(DateTime exitTime, decimal amountCharged)
    {
        if (ExitTime is not null) // idempotência
            return;

        ExitTime = EnsureUtc(exitTime);
        AmountCharged = amountCharged;
        Status = ParkingStatus.Exited;
    }

    private static string NormalizePlate(string plate)
        => plate.Trim().ToUpperInvariant();

    private static DateTime EnsureUtc(DateTime dt)
        => dt.Kind == DateTimeKind.Utc ? dt : DateTime.SpecifyKind(dt, DateTimeKind.Utc);
}
