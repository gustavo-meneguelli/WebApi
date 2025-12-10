using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Enums;
using Application.Features.Carts.DTOs;
using Application.Features.Carts.Repositories;
using Application.Features.Carts.Services;
using Application.Features.Products.Repositories;
using Domain.Entities;
using Moq;

namespace UnitTests;

public class CartServiceTests
{
    private readonly Mock<ICartRepository> _cartRepositoryMock;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CartService _cartService;

    public CartServiceTests()
    {
        _cartRepositoryMock = new Mock<ICartRepository>();
        _productRepositoryMock = new Mock<IProductRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _cartService = new CartService(
            _cartRepositoryMock.Object,
            _productRepositoryMock.Object,
            _unitOfWorkMock.Object
        );
    }

    [Fact]
    public async Task GetMyCartAsync_ShouldReturnEmptyCart_WhenCartDoesNotExist()
    {
        // Arrange
        _cartRepositoryMock
            .Setup(r => r.GetByUserIdWithItemsAsync(It.IsAny<int>()))
            .ReturnsAsync((Cart?)null);

        // Act
        var result = await _cartService.GetMyCartAsync(1);

        // Assert
        Assert.NotNull(result.Data);
        Assert.Empty(result.Data!.Items);
        Assert.Equal(0, result.Data.TotalAmount);
    }



    [Fact]
    public async Task AddItemAsync_ShouldReturnNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        var userId = 1;
        var cart = new Cart { UserId = userId, Items = new List<CartItem>() };
        var dto = new AddToCartDto { ProductId = 999, Quantity = 1 };

        _cartRepositoryMock
            .Setup(r => r.GetByUserIdWithItemsAsync(userId))
            .ReturnsAsync(cart);

        _productRepositoryMock
            .Setup(r => r.GetByIdAsync(dto.ProductId))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _cartService.AddItemAsync(userId, dto);

        // Assert
        Assert.Equal(TypeResult.NotFound, result.TypeResult);
    }

    [Fact]
    public async Task AddItemAsync_ShouldUpdateQuantity_WhenProductAlreadyInCart()
    {
        // Arrange
        var userId = 1;
        var product = new Product { Id = 1, Name = "Notebook", Price = 3000m };
        var existingItem = new CartItem 
        { 
            Id = 1, 
            ProductId = 1, 
            Quantity = 1, 
            UnitPrice = 3000m,
            Product = product
        };
        var cart = new Cart 
        { 
            UserId = userId, 
            Items = new List<CartItem> { existingItem } 
        };
        var dto = new AddToCartDto { ProductId = 1, Quantity = 2 };

        _cartRepositoryMock
            .Setup(r => r.GetByUserIdWithItemsAsync(userId))
            .ReturnsAsync(cart);

        _productRepositoryMock
            .Setup(r => r.GetByIdAsync(dto.ProductId))
            .ReturnsAsync(product);

        // Act
        var result = await _cartService.AddItemAsync(userId, dto);

        // Assert
        Assert.NotNull(result.Data);
        Assert.Equal(3, existingItem.Quantity); // 1 + 2
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task RemoveItemAsync_ShouldRemoveItem_WhenItemExists()
    {
        // Arrange
        var userId = 1;
        var product = new Product { Id = 1, Name = "Notebook", Price = 3000m };
        var cartItem = new CartItem 
        { 
            Id = 1, 
            ProductId = 1, 
            Quantity = 1, 
            UnitPrice = 3000m,
            Product = product
        };
        var cart = new Cart 
        { 
            UserId = userId, 
            Items = new List<CartItem> { cartItem } 
        };

        _cartRepositoryMock
            .Setup(r => r.GetByUserIdWithItemsAsync(userId))
            .ReturnsAsync(cart);

        // Act
        var result = await _cartService.RemoveItemAsync(userId, 1);

        // Assert
        Assert.Equal(TypeResult.Success, result.TypeResult);
        Assert.Empty(cart.Items);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task ClearCartAsync_ShouldClearAllItems_WhenCartExists()
    {
        // Arrange
        var userId = 1;
        var product = new Product { Id = 1, Name = "Notebook", Price = 3000m };
        var cart = new Cart 
        { 
            UserId = userId, 
            Items = new List<CartItem> 
            { 
                new CartItem { Id = 1, ProductId = 1, Product = product },
                new CartItem { Id = 2, ProductId = 1, Product = product }
            } 
        };

        _cartRepositoryMock
            .Setup(r => r.GetByUserIdWithItemsAsync(userId))
            .ReturnsAsync(cart);

        // Act
        var result = await _cartService.ClearCartAsync(userId);

        // Assert
        Assert.Equal(TypeResult.Success, result.TypeResult);
        Assert.Empty(cart.Items);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateItemQuantityAsync_ShouldUpdateQuantity_WhenItemExists()
    {
        // Arrange
        var userId = 1;
        var product = new Product { Id = 1, Name = "Notebook", Price = 3000m };
        var cartItem = new CartItem 
        { 
            Id = 1, 
            ProductId = 1, 
            Quantity = 1, 
            UnitPrice = 3000m,
            Product = product
        };
        var cart = new Cart 
        { 
            UserId = userId, 
            Items = new List<CartItem> { cartItem } 
        };

        _cartRepositoryMock
            .Setup(r => r.GetByUserIdWithItemsAsync(userId))
            .ReturnsAsync(cart);

        // Act
        var result = await _cartService.UpdateItemQuantityAsync(userId, 1, 5);

        // Assert
        Assert.Equal(TypeResult.Success, result.TypeResult);
        Assert.Equal(5, cartItem.Quantity);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
    }
}
