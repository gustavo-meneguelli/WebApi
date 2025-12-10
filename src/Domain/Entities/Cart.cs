using Domain.Common;

namespace Domain.Entities;

public class Cart : Entity
{
    public int UserId { get; set; }
    
    // Navigation
    public User User { get; set; } = null!;
    public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
    
    // Computed Property
    public decimal TotalAmount => Items.Sum(item => item.Subtotal);
}
