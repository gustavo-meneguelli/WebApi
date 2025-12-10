using Application.Common.Models;
using Application.Features.Orders.DTOs;

namespace Application.Features.Orders.Services;

public interface IOrderService
{
    Task<Result<OrderResponseDto>> CreateFromCartAsync(int userId);
    Task<Result<OrderResponseDto>> GetByIdAsync(int orderId, int userId);
    Task<Result<PagedResult<OrderResponseDto>>> GetMyOrdersAsync(int userId, PaginationParams pagination);
    Task<Result<OrderResponseDto>> CancelOrderAsync(int orderId, int userId);
}
