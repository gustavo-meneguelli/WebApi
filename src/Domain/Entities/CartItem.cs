using Domain.Common;

namespace Domain.Entities;

public class CartItem : Entity
{
    public int CartId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; } // Preço congelado no momento da adição
    
    // Computed Property
    public decimal Subtotal => Quantity * UnitPrice;
    
    // Navigation
    public Cart Cart { get; set; } = null!;
    public Product Product { get; set; } = null!;
}
