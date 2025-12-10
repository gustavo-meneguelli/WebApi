using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");
        
        builder.HasKey(o => o.Id);
        
        // OrderNumber único
        builder.HasIndex(o => o.OrderNumber).IsUnique();
        
        builder.Property(o => o.OrderNumber)
            .IsRequired()
            .HasMaxLength(50);
        
        // Precision para valores monetários
        builder.Property(o => o.TotalAmount)
            .HasPrecision(18, 2)
            .IsRequired();
        
        builder.Property(o => o.OrderDate)
            .IsRequired();
        
        builder.Property(o => o.Status)
            .IsRequired();
        
        // Relacionamento com User
        builder.HasOne(o => o.User)
            .WithMany()
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Soft Delete Query Filter
        builder.HasQueryFilter(o => !o.IsDeleted);
        
        // Audit Fields
        builder.Property(o => o.CreatedAt).IsRequired();
        builder.Property(o => o.UpdatedAt);
    }
}
