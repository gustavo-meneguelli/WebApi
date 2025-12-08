using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Product> Products { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Category> Categories { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>() //Exibe apenas produtos que não foram deletados
            .HasQueryFilter(p => !p.IsDeleted);

        modelBuilder.Entity<User>()
            .HasQueryFilter(u => !u.IsDeleted); //Exibe apenas usuários que não foram deletados
        
        modelBuilder.Entity<Category>()
            .HasQueryFilter(c => !c.IsDeleted); //Exibe apenas categorias que não foram deletadas
        
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict); //Não deixa deletar categoria se houver produtos relacionado 
    }
    
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<Domain.Common.Entity>();

        //Para cada entidade trackeada será feita uma modificação no seu estado e propriedades
        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
                entry.Entity.IsDeleted = false;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }

            if (entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified; 
            
                entry.Entity.IsDeleted = true;
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}