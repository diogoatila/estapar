using Estapar.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Estapar.Infrastructure.Persistence.Configurations;

public sealed class ParkingSessionConfiguration : IEntityTypeConfiguration<ParkingSession>
{
    public void Configure(EntityTypeBuilder<ParkingSession> builder)
    {
        builder.ToTable("ParkingSessions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.LicensePlate)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.SectorCode)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(x => x.EntryTime)
            .IsRequired();

        builder.Property(x => x.ExitTime)
            .IsRequired(false);

        builder.Property(x => x.PricePerHourApplied)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.AmountCharged)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.HasIndex(x => x.SectorCode);
        builder.HasIndex(x => x.EntryTime);
        builder.HasIndex(x => x.ExitTime);

        // Índice único para garantir 1 sessão ativa por placa
        builder.HasIndex(x => x.LicensePlate)
            .IsUnique()
            .HasFilter("[ExitTime] IS NULL");
    }
}