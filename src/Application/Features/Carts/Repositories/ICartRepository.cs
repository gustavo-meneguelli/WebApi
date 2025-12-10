using Application.Common.Interfaces;
using Domain.Entities;

namespace Application.Features.Carts.Repositories;

public interface ICartRepository : IRepository<Cart>
{
    Task<Cart?> GetByUserIdAsync(int userId);
    Task<Cart?> GetByUserIdWithItemsAsync(int userId);
}
