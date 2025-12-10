using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Entities;

namespace Application.Features.Orders.Repositories;

public interface IOrderRepository : IRepository<Order>
{
    Task<Order?> GetByOrderNumberAsync(string orderNumber);
    Task<Order?> GetByIdWithItemsAsync(int id);
    Task<PagedResult<Order>> GetByUserIdAsync(int userId, PaginationParams pagination);
}
