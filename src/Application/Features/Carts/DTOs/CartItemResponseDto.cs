namespace Application.Features.Carts.DTOs;

public class CartItemResponseDto
{
    public int Id { get; init; }
    public int ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal YourPrice { get; init; } // Preço congelado (vai pagar)
    public decimal CurrentPrice { get; init; } // Preço atual do produto
    public decimal Savings { get; init; } // CurrentPrice - YourPrice (+ = economizando, - = aumentou)
    public decimal Subtotal { get; init; }
}
