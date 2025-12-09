using Application.Features.Products.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Generics;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProductRepository(AppDbContext context) : Repository<Product>(context), IProductRepository
{
    private readonly AppDbContext _context = context;

    public async Task<Product?> GetByNameAsync(string name)
    {
        return await _context.Products.FirstOrDefaultAsync(p => p.Name == name);
    }

    public async Task<bool> ExistByNameAsync(string name)
    {
        return await _context.Products.AnyAsync(p => p.Name == name);
    }

    public override async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

}
