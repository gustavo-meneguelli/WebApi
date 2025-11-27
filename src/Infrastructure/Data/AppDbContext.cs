using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Product> Products { get; set; }
    public DbSet<User> Users { get; set; }
}