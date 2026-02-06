using Estapar.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Estapar.Infrastructure.Persistence.Configurations;

public sealed class SpotConfiguration : IEntityTypeConfiguration<Spot>
{
    public void Configure(EntityTypeBuilder<Spot> builder)
    {
        builder.ToTable("Spots");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever(); // id vem do simulador

        builder.Property(x => x.SectorCode)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(x => x.Lat)
            .HasColumnType("decimal(9,6)")
            .IsRequired();

        builder.Property(x => x.Lng)
            .HasColumnType("decimal(9,6)")
            .IsRequired();

        builder.Property(x => x.IsOccupied)
            .IsRequired();

        builder.HasIndex(x => new { x.SectorCode, x.IsOccupied });
    }
}