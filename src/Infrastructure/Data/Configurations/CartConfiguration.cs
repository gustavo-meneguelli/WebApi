using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.ToTable("Carts");
        
        builder.HasKey(c => c.Id);
        
        // 1 usuário = 1 carrinho (UserId único)
        builder.HasIndex(c => c.UserId).IsUnique();
        
        // Relacionamento com User
        builder.HasOne(c => c.User)
            .WithMany()
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Soft Delete Query Filter
        builder.HasQueryFilter(c => !c.IsDeleted);
        
        // Audit Fields
        builder.Property(c => c.CreatedAt).IsRequired();
        builder.Property(c => c.UpdatedAt);
    }
}
