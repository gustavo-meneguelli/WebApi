using Application.Common.Models;
using Application.Features.Carts.DTOs;

namespace Application.Features.Carts.Services;

public interface ICartService
{
    Task<Result<CartResponseDto>> GetMyCartAsync(int userId);
    Task<Result<CartResponseDto>> AddItemAsync(int userId, AddToCartDto dto);
    Task<Result<CartResponseDto>> UpdateItemQuantityAsync(int userId, int cartItemId, int quantity);
    Task<Result<string>> RemoveItemAsync(int userId, int cartItemId);
    Task<Result<string>> ClearCartAsync(int userId);
}
