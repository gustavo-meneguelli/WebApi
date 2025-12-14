using Application.Common.Interfaces;
using Application.Features.Products.DTOs;
using Application.Features.Products.Repositories;
using Application.Features.ProductReviews.Repositories;
using Application.Features.Products.Services;
using AutoMapper;
using Domain.Constants;
using Domain.Entities;
using Moq;

namespace UnitTests;

public class ProductServiceTests
{
    private readonly Mock<IProductReviewRepository> _productReviewRepositoryMock = new();

    public ProductServiceTests()
    {
        // Setup padrão: retorna (0, 0) para média de avaliações
        _productReviewRepositoryMock
            .Setup(r => r.GetProductRatingSummaryAsync(It.IsAny<int>()))
            .ReturnsAsync((0.0, 0));
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnSuccess_WhenProductExists()
    {
        // ARRANGE
        var productRepositoryMock = new Mock<IProductRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new Application.Common.Mappings.MappingProfile());
        });
        var mapper = configuration.CreateMapper();

        var model = new Product
        {
            Id = 1,
            Name = "Teste",
            Description = "Descrição do produto",
            ImageUrl = "https://example.com/image.jpg",
            Price = 10,
            CategoryId = 1
        };

        productRepositoryMock
            .Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(model);

        var service = new ProductService(
            productRepositoryMock.Object,
            _productReviewRepositoryMock.Object,
            mapper,
            unitOfWorkMock.Object);

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
        var productRepositoryMock = new Mock<IProductRepository>();
        var mapperMock = new Mock<IMapper>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        productRepositoryMock
            .Setup(repo => repo.GetByIdAsync(99))
            .ReturnsAsync((Product?)null);

        var service = new ProductService(
            productRepositoryMock.Object,
            _productReviewRepositoryMock.Object,
            mapperMock.Object,
            unitOfWorkMock.Object);

        // ACT
        var result = await service.GetByIdAsync(99);

        // ASSERT
        Assert.NotNull(result);
        Assert.Equal(Application.Enums.TypeResult.NotFound, result.TypeResult);

        var expectedMessage = string.Format(ErrorMessages.NotFound, "Produto");
        Assert.Equal(expectedMessage, result.Message);
    }


    [Fact]
    public async Task UpdateAsync_ShouldReturnSuccess_WhenDataIsValid()
    {
        // ARRANGE
        var productRepositoryMock = new Mock<IProductRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        // Setup do Mapper Real
        var config = new MapperConfiguration(cfg => cfg.AddProfile(new Application.Common.Mappings.MappingProfile()));
        var mapper = config.CreateMapper();

        // Produto original no banco
        var originalProduct = new Product 
        { 
            Id = 1, 
            Name = "Original", 
            Description = "Descrição original",
            ImageUrl = "https://example.com/original.jpg",
            Price = 10, 
            CategoryId = 1 
        };

        productRepositoryMock
            .Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(originalProduct);

        // Setup para validar que o nome novo não existe (se mudou de nome)
        productRepositoryMock
            .Setup(repo => repo.ExistByNameAsync("Novo Nome"))
            .ReturnsAsync(false);

        var service = new ProductService(
            productRepositoryMock.Object,
            _productReviewRepositoryMock.Object,
            mapper,
            unitOfWorkMock.Object);

        var updateDto = new UpdateProductDto { Name = "Novo Nome", Price = 20, CategoryId = 1 };

        // ACT
        var result = await service.UpdateAsync(1, updateDto);

        // ASSERT
        Assert.NotNull(result.Data);
        Assert.Equal(Application.Enums.TypeResult.Success, result.TypeResult);
        Assert.Equal("Novo Nome", result.Data.Name); // Verifica se o Mapper atualizou o retorno

        // Verifica se chamou Update e Commit
        productRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Product>()), Times.Once);
        unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task AddAsync_ShouldReturnSuccess_WhenDataIsValid()
    {
        // ARRANGE
        var productRepositoryMock = new Mock<IProductRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        var mapperMock = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new Application.Common.Mappings.MappingProfile());
        });
        var mapper = mapperMock.CreateMapper();

        var dto = new CreateProductDto()
        {
            Name = "Teste",
            Description = "Descrição do produto de teste",
            ImageUrl = "https://example.com/teste.jpg",
            Price = 10,
            CategoryId = 1
        };

        var service = new ProductService(
            productRepositoryMock.Object,
            _productReviewRepositoryMock.Object,
            mapper,
            unitOfWorkMock.Object);

        // ACT
        var result = await service.AddAsync(dto);

        // ASSERT
        Assert.NotNull(result.Data);
        Assert.Equal(Application.Enums.TypeResult.Created, result.TypeResult);
        Assert.Equal(dto.Name, result.Data.Name);

        // Verifica se chamou Add e Commit
        productRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Once);
        unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
    }
}



