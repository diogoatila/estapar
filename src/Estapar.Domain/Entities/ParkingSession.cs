using Estapar.Domain.Enums;

namespace Estapar.Domain.Entities;

public sealed class ParkingSession
{
    public Guid Id { get; private set; }

    public string LicensePlate { get; private set; } = null!;

    public string SectorCode { get; private set; } = null!;
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
        SectorCode = NormalizeSectorCode(sectorCode);

        if (spotId <= 0)
            throw new ArgumentOutOfRangeException(nameof(spotId), "SpotId must be greater than zero.");

        if (pricePerHourApplied < 0)
            throw new ArgumentOutOfRangeException(nameof(pricePerHourApplied), "PricePerHourApplied cannot be negative.");

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
            return; // idempotente

        if (Status != ParkingStatus.Entered)
            throw new InvalidOperationException($"It is not possible to mark it as Parked when the status is {Status}.");

        Status = ParkingStatus.Parked;
    }

    public void RegisterExit(DateTime exitTime, decimal amountCharged)
    {
        if (ExitTime is not null)
            return; // idempotência

        if (Status == ParkingStatus.Exited)
            return;

        if (amountCharged < 0)
            throw new ArgumentOutOfRangeException(nameof(amountCharged), "AmountCharged cannot be negative.");

        var exitUtc = EnsureUtc(exitTime);

        if (exitUtc < EntryTime)
            throw new InvalidOperationException("ExitTime cannot be less than EntryTime.");

        ExitTime = exitUtc;
        AmountCharged = amountCharged;
        Status = ParkingStatus.Exited;
    }

    private static string NormalizePlate(string plate)
    {
        if (string.IsNullOrWhiteSpace(plate))
            throw new ArgumentException("LicensePlate cannot be empty.", nameof(plate));

        plate = plate.Trim().ToUpperInvariant().Replace(" ", "");

        if (plate.Length < 7 || plate.Length > 8)
            throw new ArgumentException("Invalid LicensePlate.", nameof(plate));

        return plate;
    }

    private static string NormalizeSectorCode(string sectorCode)
    {
        if (string.IsNullOrWhiteSpace(sectorCode))
            throw new ArgumentException("SectorCode cannot be empty.", nameof(sectorCode));

        return sectorCode.Trim().ToUpperInvariant();
    }

    private static DateTime EnsureUtc(DateTime dt)
    {
        return dt.Kind switch
        {
            DateTimeKind.Utc => dt,
            DateTimeKind.Local => dt.ToUniversalTime(),
            DateTimeKind.Unspecified => throw new ArgumentException(
                "Date/time must be in UTC. (ex: 2025-01-01T12:00:00.000Z)."),
            _ => throw new ArgumentException("Invalid DateTime.")
        };
    }
}