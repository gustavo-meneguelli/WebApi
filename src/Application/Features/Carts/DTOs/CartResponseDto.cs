namespace Application.Features.Carts.DTOs;

public class CartResponseDto
{
    public int Id { get; init; }
    public List<CartItemResponseDto> Items { get; init; } = new();
    public decimal TotalAmount { get; init; }
    public decimal TotalSavings { get; init; } // Soma de todas as economias
    public int TotalItems => Items.Sum(i => i.Quantity);
}
