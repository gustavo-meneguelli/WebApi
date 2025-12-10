using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.ToTable("CartItems");
        
        builder.HasKey(ci => ci.Id);
        
        // Precision para valores monetários
        builder.Property(ci => ci.UnitPrice)
            .HasPrecision(18, 2)
            .IsRequired();
        
        builder.Property(ci => ci.Quantity)
            .IsRequired();
        
        // Relacionamento com Cart
        builder.HasOne(ci => ci.Cart)
            .WithMany(c => c.Items)
            .HasForeignKey(ci => ci.CartId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Relacionamento com Product
        builder.HasOne(ci => ci.Product)
            .WithMany()
            .HasForeignKey(ci => ci.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Soft Delete Query Filter
        builder.HasQueryFilter(ci => !ci.IsDeleted);
        
        // Audit Fields
        builder.Property(ci => ci.CreatedAt).IsRequired();
        builder.Property(ci => ci.UpdatedAt);
    }
}
