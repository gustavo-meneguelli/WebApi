using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Features.Carts.Repositories;
using Application.Features.Orders.DTOs;
using Application.Features.Orders.Repositories;
using Domain.Entities;
using Domain.Enums;

namespace Application.Features.Orders.Services;

public class OrderService(
    IOrderRepository orderRepository,
    ICartRepository cartRepository,
    IUnitOfWork unitOfWork) : IOrderService
{
    public async Task<Result<OrderResponseDto>> CreateFromCartAsync(int userId)
    {
        var cart = await cartRepository.GetByUserIdWithItemsAsync(userId);

        if (cart is null || !cart.Items.Any())
            return Result<OrderResponseDto>.NotFound("Carrinho vazio");

        var order = new Order
        {
            UserId = userId,
            OrderNumber = GenerateOrderNumber(),
            Status = OrderStatus.Pending,
            TotalAmount = cart.TotalAmount,
            OrderDate = DateTime.UtcNow
        };

        foreach (var cartItem in cart.Items)
        {
            order.Items.Add(new OrderItem
            {
                ProductId = cartItem.ProductId,
                Quantity = cartItem.Quantity,
                UnitPrice = cartItem.UnitPrice
            });
        }

        await orderRepository.AddAsync(order);

        cart.Items.Clear();

        await unitOfWork.CommitAsync();

        var response = MapOrderToDto(order);
        return Result<OrderResponseDto>.Success(response);
    }

    public async Task<Result<OrderResponseDto>> GetByIdAsync(int orderId, int userId)
    {
        var order = await orderRepository.GetByIdWithItemsAsync(orderId);

        if (order is null)
            return Result<OrderResponseDto>.NotFound("Pedido não encontrado");

        if (order.UserId != userId)
            return Result<OrderResponseDto>.Unauthorized("Acesso negado");

        var response = MapOrderToDto(order);
        return Result<OrderResponseDto>.Success(response);
    }

    public async Task<Result<PagedResult<OrderResponseDto>>> GetMyOrdersAsync(int userId, PaginationParams pagination)
    {
        var pagedOrders = await orderRepository.GetByUserIdAsync(userId, pagination);

        var ordersDto = pagedOrders.Items.Select(MapOrderToDto).ToList();

        var result = new PagedResult<OrderResponseDto>
        {
            Items = ordersDto,
            TotalCount = pagedOrders.TotalCount,
            CurrentPage = pagedOrders.CurrentPage,
            PageSize = pagedOrders.PageSize,
            TotalPages = pagedOrders.TotalPages
        };

        return Result<PagedResult<OrderResponseDto>>.Success(result);
    }

    public async Task<Result<OrderResponseDto>> CancelOrderAsync(int orderId, int userId)
    {
        var order = await orderRepository.GetByIdWithItemsAsync(orderId);

        if (order is null)
            return Result<OrderResponseDto>.NotFound("Pedido não encontrado");

        if (order.UserId != userId)
            return Result<OrderResponseDto>.Unauthorized("Acesso negado");

        if (order.Status != OrderStatus.Pending)
            return Result<OrderResponseDto>.NotFound("Apenas pedidos pendentes podem ser cancelados");

        order.Status = OrderStatus.Cancelled;
        await unitOfWork.CommitAsync();

        var response = MapOrderToDto(order);
        return Result<OrderResponseDto>.Success(response);
    }

    private static string GenerateOrderNumber()
    {
        var date = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var random = new Random().Next(1000, 9999);
        return $"ORD-{date}-{random}";
    }

    private static OrderResponseDto MapOrderToDto(Order order)
    {
        var items = order.Items.Select(item => new OrderItemResponseDto
        {
            Id = item.Id,
            ProductId = item.ProductId,
            ProductName = item.Product.Name,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice,
            Subtotal = item.Subtotal
        }).ToList();

        return new OrderResponseDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            Status = order.Status,
            TotalAmount = order.TotalAmount,
            OrderDate = order.OrderDate,
            Items = items
        };
    }
}
