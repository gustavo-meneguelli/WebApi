using Application.Features.Carts.Repositories;
using Domain.Entities;
using Infrastructure.Generics;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CartRepository(Data.AppDbContext context) : Repository<Cart>(context), ICartRepository
{
    private readonly Data.AppDbContext _context = context;
    public async Task<Cart?> GetByUserIdAsync(int userId)
    {
        return await _context.Carts
            .FirstOrDefaultAsync(c => c.UserId == userId);
    }

    public async Task<Cart?> GetByUserIdWithItemsAsync(int userId)
    {
        return await _context.Carts
            .Include(c => c.Items)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId);
    }
}
