using Estapar.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Estapar.Infrastructure.Persistence;

public sealed class AppDbContext : DbContext
{
    public DbSet<GarageSector> Sectors => Set<GarageSector>();
    public DbSet<Spot> Spots => Set<Spot>();
    public DbSet<ParkingSession> ParkingSessions => Set<ParkingSession>();

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}