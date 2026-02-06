using Estapar.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Estapar.Infrastructure.Persistence.Configurations;

public sealed class GarageSectorConfiguration : IEntityTypeConfiguration<GarageSector>
{
    public void Configure(EntityTypeBuilder<GarageSector> builder)
    {
        builder.ToTable("GarageSectors");

        builder.HasKey(x => x.SectorCode);

        builder.Property(x => x.SectorCode)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(x => x.BasePrice)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.MaxCapacity)
            .IsRequired();
    }
}