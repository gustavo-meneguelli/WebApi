using Application.Interfaces;
using Domain.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProductRepository(AppDbContext context) : IProductRepository
{
    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await context.Products
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await context.Products.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Product?> GetByNameAsync(string name)
    {
        return await context.Products.FirstOrDefaultAsync(p => p.Name == name);
    }

    public async Task<Product> AddAsync(Product product)
    {
        await context.Products.AddAsync(product);
        await context.SaveChangesAsync();
        return product;
    }

    public async Task UpdateAsync(Product product)
    {
        await context.SaveChangesAsync();
    }

    public async Task<bool> ExistByNameAsync(string name)
    {
        return await context.Products.AnyAsync(p => p.Name == name);
    }
}