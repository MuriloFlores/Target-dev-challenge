using CommissionManager.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommissionManager.Infrastructure.Persistence.Configurations;

internal sealed class SaleRowConfiguration : IEntityTypeConfiguration<SaleRow>
{
    public void Configure(EntityTypeBuilder<SaleRow> builder)
    {
        builder.ToTable("sales");
        builder.HasKey(x => x.Id);
        builder.Property(s => s.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(s => s.SellerName)
            .HasColumnName("seller_name")
            .HasMaxLength(100)
            .IsRequired();
        
        builder.Property(s => s.Value)
            .HasColumnName("value")
            .HasPrecision(18,2)
            .IsRequired();
    }
}