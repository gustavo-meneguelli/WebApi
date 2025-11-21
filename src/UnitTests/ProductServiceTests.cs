using Moq;
using Application.Services;
using Application.Interfaces;
using Domain.Models;
using AutoMapper;

namespace UnitTests;

public class ProductServiceTests
{
    [Fact]
    public async Task GetByIdAsync_ShouldReturnSuccess_WhenProductExists()
    {
        // ARRANGE
        var repositoryMock = new Mock<IProductRepository>();
        var mapperMock = new Mock<IMapper>();

        var fakeProduct = new Product 
        { 
            Id = 1, 
            Name = "Teste", 
            Price = 10 
        };

        repositoryMock
            .Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(fakeProduct);

        var service = new ProductService(repositoryMock.Object, mapperMock.Object);

        // ACT
        var result = await service.GetByIdAsync(1);

        // ASSERT
        Assert.NotNull(result);
        Assert.Equal(Application.Enums.TypeResult.Success, result.TypeResult);
        Assert.Equal("Teste", result.Data?.Name);
    }
    
    [Fact]
    public async Task GetByIdAsync_ShouldReturnNotFound_WhenProductDoesNotExist()
    {
        // ARRANGE
        var repositoryMock = new Mock<IProductRepository>();
        var mapperMock = new Mock<IMapper>();

        repositoryMock
            .Setup(repo => repo.GetByIdAsync(99))
            .ReturnsAsync((Product?)null); 

        var service = new ProductService(repositoryMock.Object, mapperMock.Object);

        // ACT
        var result = await service.GetByIdAsync(99);

        // ASSERT
        Assert.NotNull(result);
        Assert.Equal(Application.Enums.TypeResult.NotFound, result.TypeResult);
        Assert.Equal("No product were found with this ID.", result.Message);
    }
}