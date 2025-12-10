using Application.Common.Models;
using Application.Features.Orders.Repositories;
using Domain.Entities;
using Infrastructure.Generics;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class OrderRepository(Data.AppDbContext context) : Repository<Order>(context), IOrderRepository
{
    private readonly Data.AppDbContext _context = context;

    public async Task<Order?> GetByOrderNumberAsync(string orderNumber)
    {
        return await _context.Orders
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);
    }

    public async Task<Order?> GetByIdWithItemsAsync(int id)
    {
        return await _context.Orders
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<PagedResult<Order>> GetByUserIdAsync(int userId, PaginationParams pagination)
    {
        var query = _context.Orders
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.OrderDate);

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .ToListAsync();

        var totalPages = (int)Math.Ceiling(totalCount / (double)pagination.PageSize);

        return new PagedResult<Order>
        {
            Items = items,
            TotalCount = totalCount,
            CurrentPage = pagination.PageNumber,
            PageSize = pagination.PageSize,
            TotalPages = totalPages
        };
    }
}
