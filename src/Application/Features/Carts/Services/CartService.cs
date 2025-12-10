using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Features.Carts.DTOs;
using Application.Features.Carts.Repositories;
using Application.Features.Carts.Services;
using Application.Features.Products.Repositories;
using Domain.Entities;

namespace Application.Features.Carts.Services;

public class CartService(
    ICartRepository cartRepository,
    IProductRepository productRepository,
    IUnitOfWork unitOfWork) : ICartService
{
    public async Task<Result<CartResponseDto>> GetMyCartAsync(int userId)
    {
        var cart = await cartRepository.GetByUserIdWithItemsAsync(userId);

        if (cart is null)
        {
            return Result<CartResponseDto>.Success(new CartResponseDto());
        }

        var response = MapCartToDto(cart);
        return Result<CartResponseDto>.Success(response);
    }

    public async Task<Result<CartResponseDto>> AddItemAsync(int userId, AddToCartDto dto)
    {
        var cart = await cartRepository.GetByUserIdWithItemsAsync(userId);

        if (cart is null)
        {
            cart = new Cart { UserId = userId };
            await cartRepository.AddAsync(cart);
        }

        var product = await productRepository.GetByIdAsync(dto.ProductId);
        if (product is null)
            return Result<CartResponseDto>.NotFound("Produto não encontrado");

        var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == dto.ProductId);

        if (existingItem is not null)
        {
            existingItem.Quantity += dto.Quantity;
        }
        else
        {
            var cartItem = new CartItem
            {
                CartId = cart.Id,
                ProductId = product.Id,
                Quantity = dto.Quantity,
                UnitPrice = product.Price
            };

            cart.Items.Add(cartItem);
        }

        await unitOfWork.CommitAsync();

        var response = MapCartToDto(cart);
        return Result<CartResponseDto>.Success(response);
    }

    public async Task<Result<CartResponseDto>> UpdateItemQuantityAsync(int userId, int cartItemId, int quantity)
    {
        var cart = await cartRepository.GetByUserIdWithItemsAsync(userId);

        if (cart is null)
            return Result<CartResponseDto>.NotFound("Carrinho não encontrado");

        var item = cart.Items.FirstOrDefault(i => i.Id == cartItemId);

        if (item is null)
            return Result<CartResponseDto>.NotFound("Item não encontrado no carrinho");

        item.Quantity = quantity;
        await unitOfWork.CommitAsync();

        var response = MapCartToDto(cart);
        return Result<CartResponseDto>.Success(response);
    }

    public async Task<Result<string>> RemoveItemAsync(int userId, int cartItemId)
    {
        var cart = await cartRepository.GetByUserIdWithItemsAsync(userId);

        if (cart is null)
            return Result<string>.NotFound("Carrinho não encontrado");

        var item = cart.Items.FirstOrDefault(i => i.Id == cartItemId);

        if (item is null)
            return Result<string>.NotFound("Item não encontrado no carrinho");

        cart.Items.Remove(item);
        await unitOfWork.CommitAsync();

        return Result<string>.Success("Item removido com sucesso");
    }

    public async Task<Result<string>> ClearCartAsync(int userId)
    {
        var cart = await cartRepository.GetByUserIdWithItemsAsync(userId);

        if (cart is null)
            return Result<string>.NotFound("Carrinho não encontrado");

        cart.Items.Clear();
        await unitOfWork.CommitAsync();

        return Result<string>.Success("Carrinho limpo com sucesso");
    }

    private CartResponseDto MapCartToDto(Cart cart)
    {
        var items = cart.Items.Select(item => new CartItemResponseDto
        {
            Id = item.Id,
            ProductId = item.ProductId,
            ProductName = item.Product.Name,
            Quantity = item.Quantity,
            YourPrice = item.UnitPrice,
            CurrentPrice = item.Product.Price,
            Savings = item.Product.Price - item.UnitPrice,
            Subtotal = item.Subtotal
        }).ToList();

        return new CartResponseDto
        {
            Id = cart.Id,
            Items = items,
            TotalAmount = cart.TotalAmount,
            TotalSavings = items.Sum(i => i.Savings * i.Quantity)
        };
    }
}
