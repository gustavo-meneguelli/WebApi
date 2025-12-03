using Application.Common.Models;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Generics;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProductRepository(AppDbContext context) : Repository<Product>(context), IProductRepository
{
    private readonly AppDbContext _context = context;

    public async Task<PagedResult<Product>> GetAllAsync(PaginationParams paginationParams)
    {
        var totalCount = await _context.Products.CountAsync();

        var items = await _context.Products
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

    public async Task<Product?> GetByNameAsync(string name)
    {
        return await _context.Products.FirstOrDefaultAsync(p => p.Name == name);
    }

    public async Task<bool> ExistByNameAsync(string name)
    {
        return await _context.Products.AnyAsync(p => p.Name == name);
    }

}