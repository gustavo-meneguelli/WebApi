using Domain.Enums;

namespace Application.Features.Orders.DTOs;

public class OrderResponseDto
{
    public int Id { get; init; }
    public string OrderNumber { get; init; } = string.Empty;
    public OrderStatus Status { get; init; }
    public decimal TotalAmount { get; init; }
    public DateTime OrderDate { get; init; }
    public List<OrderItemResponseDto> Items { get; init; } = new();
    public int TotalItems => Items.Sum(i => i.Quantity);
}
