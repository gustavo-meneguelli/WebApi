using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");
        
        builder.HasKey(oi => oi.Id);
        
        // Precision para valores monetários
        builder.Property(oi => oi.UnitPrice)
            .HasPrecision(18, 2)
            .IsRequired();
        
        builder.Property(oi => oi.Quantity)
            .IsRequired();
        
        // Relacionamento com Order
        builder.HasOne(oi => oi.Order)
            .WithMany(o => o.Items)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Relacionamento com Product
        builder.HasOne(oi => oi.Product)
            .WithMany()
            .HasForeignKey(oi => oi.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Soft Delete Query Filter
        builder.HasQueryFilter(oi => !oi.IsDeleted);
        
        // Audit Fields
        builder.Property(oi => oi.CreatedAt).IsRequired();
        builder.Property(oi => oi.UpdatedAt);
    }
}
