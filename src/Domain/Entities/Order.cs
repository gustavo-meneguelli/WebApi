using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

public class Order : Entity
{
    public int UserId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public decimal TotalAmount { get; set; } // Congelado no momento da compra
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    
    // Navigation
    public User User { get; set; } = null!;
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}