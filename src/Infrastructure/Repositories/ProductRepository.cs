using Application.Common.Models;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProductRepository(AppDbContext context) : IProductRepository
{
    public async Task<PagedResult<Product>> GetAllAsync(PaginationParams paginationParams)
    {
        var totalCount = await context.Products.CountAsync();

        var items = await context.Products
            .OrderBy(p => p.Id)
            .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .AsNoTracking() 
            .ToListAsync();

        var totalPages = (int)Math.Ceiling(totalCount / (double)paginationParams.PageSize);

        return new PagedResult<Product>
        {
            Items = items,
            CurrentPage = paginationParams.PageNumber,
            PageSize = paginationParams.PageSize,
            TotalCount = totalCount,
            TotalPages = totalPages 
        };
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

    public async Task DeleteAsync(Product product)
    {
        context.Products.Remove(product);
        await context.SaveChangesAsync();
    }
}