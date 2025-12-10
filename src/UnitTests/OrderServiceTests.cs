using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Enums;
using Application.Features.Carts.Repositories;
using Application.Features.Orders.Repositories;
using Application.Features.Orders.Services;
using Domain.Entities;
using Domain.Enums;
using Moq;

namespace UnitTests;

public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<ICartRepository> _cartRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly OrderService _orderService;

    public OrderServiceTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _cartRepositoryMock = new Mock<ICartRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _orderService = new OrderService(
            _orderRepositoryMock.Object,
            _cartRepositoryMock.Object,
            _unitOfWorkMock.Object
        );
    }



    [Fact]
    public async Task CreateFromCartAsync_ShouldReturnNotFound_WhenCartIsEmpty()
    {
        // Arrange
        var userId = 1;
        var cart = new Cart { UserId = userId, Items = new List<CartItem>() };

        _cartRepositoryMock
            .Setup(r => r.GetByUserIdWithItemsAsync(userId))
            .ReturnsAsync(cart);

        // Act
        var result = await _orderService.CreateFromCartAsync(userId);

        // Assert
        Assert.Equal(TypeResult.NotFound, result.TypeResult);
        _orderRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Never);
    }

    [Fact]
    public async Task CreateFromCartAsync_ShouldReturnNotFound_WhenCartDoesNotExist()
    {
        // Arrange
        _cartRepositoryMock
            .Setup(r => r.GetByUserIdWithItemsAsync(It.IsAny<int>()))
            .ReturnsAsync((Cart?)null);

        // Act
        var result = await _orderService.CreateFromCartAsync(1);

        // Assert
        Assert.Equal(TypeResult.NotFound, result.TypeResult);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnOrder_WhenOrderExists()
    {
        // Arrange
        var userId = 1;
        var product = new Product { Id = 1, Name = "Notebook", Price = 3000m };
        var orderItem = new OrderItem 
        { 
            Id = 1, 
            ProductId = 1, 
            Quantity = 1, 
            UnitPrice = 3000m,
            Product = product
        };
        var order = new Order 
        { 
            Id = 1, 
            UserId = userId, 
            OrderNumber = "ORD-001",
            TotalAmount = 3000m,
            Items = new List<OrderItem> { orderItem }
        };

        _orderRepositoryMock
            .Setup(r => r.GetByIdWithItemsAsync(1))
            .ReturnsAsync(order);

        // Act
        var result = await _orderService.GetByIdAsync(1, userId);

        // Assert
        Assert.Equal(TypeResult.Success, result.TypeResult);
        Assert.NotNull(result.Data);
        Assert.Equal("ORD-001", result.Data!.OrderNumber);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnUnauthorized_WhenUserIsNotOwner()
    {
        // Arrange
        var order = new Order 
        { 
            Id = 1, 
            UserId = 2, // Different user
            OrderNumber = "ORD-001",
            Items = new List<OrderItem>()
        };

        _orderRepositoryMock
            .Setup(r => r.GetByIdWithItemsAsync(1))
            .ReturnsAsync(order);

        // Act
        var result = await _orderService.GetByIdAsync(1, userId: 1);

        // Assert
        Assert.Equal(TypeResult.Unauthorized, result.TypeResult);
    }

    [Fact]
    public async Task CancelOrderAsync_ShouldCancelOrder_WhenOrderIsPending()
    {
        // Arrange
        var userId = 1;
        var order = new Order 
        { 
            Id = 1, 
            UserId = userId, 
            OrderNumber = "ORD-001",
            Status = OrderStatus.Pending,
            Items = new List<OrderItem>()
        };

        _orderRepositoryMock
            .Setup(r => r.GetByIdWithItemsAsync(1))
            .ReturnsAsync(order);

        // Act
        var result = await _orderService.CancelOrderAsync(1, userId);

        // Assert
        Assert.Equal(TypeResult.Success, result.TypeResult);
        Assert.Equal(OrderStatus.Cancelled, order.Status);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task CancelOrderAsync_ShouldReturnNotFound_WhenOrderIsNotPending()
    {
        // Arrange
        var userId = 1;
        var order = new Order 
        { 
            Id = 1, 
            UserId = userId, 
            OrderNumber = "ORD-001",
            Status = OrderStatus.Paid, // Already paid
            Items = new List<OrderItem>()
        };

        _orderRepositoryMock
            .Setup(r => r.GetByIdWithItemsAsync(1))
            .ReturnsAsync(order);

        // Act
        var result = await _orderService.CancelOrderAsync(1, userId);

        // Assert
        Assert.Equal(TypeResult.NotFound, result.TypeResult);
        Assert.Equal(OrderStatus.Paid, order.Status); // Status unchanged
    }

    [Fact]
    public async Task GetMyOrdersAsync_ShouldReturnPagedOrders()
    {
        // Arrange
        var userId = 1;
        var product = new Product { Id = 1, Name = "Notebook", Price = 3000m };
        var orders = new List<Order>
        {
            new Order 
            { 
                Id = 1, 
                UserId = userId, 
                OrderNumber = "ORD-001",
                Items = new List<OrderItem> 
                { 
                    new OrderItem { ProductId = 1, Product = product, Quantity = 1, UnitPrice = 3000m } 
                }
            }
        };

        var pagedResult = new PagedResult<Order>
        {
            Items = orders,
            TotalCount = 1,
            CurrentPage = 1,
            PageSize = 10,
            TotalPages = 1
        };

        _orderRepositoryMock
            .Setup(r => r.GetByUserIdAsync(userId, It.IsAny<PaginationParams>()))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _orderService.GetMyOrdersAsync(userId, new PaginationParams { PageNumber = 1, PageSize = 10 });

        // Assert
        Assert.Equal(TypeResult.Success, result.TypeResult);
        Assert.NotNull(result.Data);
        Assert.Single(result.Data!.Items);
    }
}
