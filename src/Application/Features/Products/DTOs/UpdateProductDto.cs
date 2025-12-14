namespace Application.Features.Products.DTOs;

public class UpdateProductDto
{
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public int CategoryId { get; init; }
}
