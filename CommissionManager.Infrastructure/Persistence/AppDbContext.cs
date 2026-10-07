using CommissionManager.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace CommissionManager.Infrastructure.Persistence;

internal sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<SaleRow> Sales => Set<SaleRow>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}