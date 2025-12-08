using Application.DTO.Products;
using Application.Interfaces.Generics;
using Moq;
using Application.Services;
using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Entities;

namespace UnitTests;

public class ProductServiceTests
{
    [Fact]
    public async Task GetByIdAsync_ShouldReturnSuccess_WhenProductExists()
    {
        // ARRANGE
        var productRepositoryMock = new Mock<IProductRepository>();
        var categoryRepositoryMock = new Mock<ICategoryRepository>();
        var configuration = new MapperConfiguration(cfg => 
        {
            cfg.AddProfile(new Application.Mappings.MappingProfile());
        });
        var mapper = configuration.CreateMapper();
        var unitToWorkMock = new Mock<IUnitOfWork>();

        var model = new Product 
        { 
            Id = 1, 
            Name = "Teste", 
            Price = 10,
            CategoryId = 1
        };

        productRepositoryMock
            .Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(model);
        
        var service = new ProductService(productRepositoryMock.Object, categoryRepositoryMock.Object, mapper, unitToWorkMock.Object);

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
        var categoryRepositoryMock = new Mock<ICategoryRepository>();
        var mapperMock = new Mock<IMapper>();
        var unitToWorkMock = new Mock<IUnitOfWork>();


        productRepositoryMock
            .Setup(repo => repo.GetByIdAsync(99))
            .ReturnsAsync((Product?)null); 

        var service = new ProductService(productRepositoryMock.Object, categoryRepositoryMock.Object, mapperMock.Object, unitToWorkMock.Object);

        // ACT
        var result = await service.GetByIdAsync(99);

        // ASSERT
        Assert.NotNull(result);
        Assert.Equal(Application.Enums.TypeResult.NotFound, result.TypeResult);
        Assert.Equal("No products were found with this ID.", result.Message);
    }
    
    [Fact]
    public async Task UpdateAsync_ShouldReturnDuplicated_WhenProductExists()
    {
        //ARRANGE
        var productRepositoryMock = new Mock<IProductRepository>();
        var categoryRepositoryMock = new Mock<ICategoryRepository>();
        var mapperMock = new Mock<IMapper>();
        var unitToWorkMock = new Mock<IUnitOfWork>();
        
        var model = new Product 
        { 
            Id = 1, 
            Name = "Teste", 
            Price = 10 ,
            CategoryId = 1
        };
        
        productRepositoryMock
            .Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(model);

        productRepositoryMock
            .Setup(repo => repo.ExistByNameAsync("Outro Teste"))
            .ReturnsAsync(true);
        
        var service = new ProductService(productRepositoryMock.Object, categoryRepositoryMock.Object, mapperMock.Object, unitToWorkMock.Object);

        //ACT
        var fakeDto = new UpdateProductDto { Name = "Outro Teste",  Price = 10 };
        var result = await service.UpdateAsync(1, fakeDto);
        
        //ASSERT
        Assert.NotNull(result);
        Assert.Equal(Application.Enums.TypeResult.Duplicated, result.TypeResult);
        Assert.Equal("Product with that name already exists.", result.Message);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnSuccess_WhenDataIsValid()
    {
        //ARRANGE
        var productRepositoryMock = new Mock<IProductRepository>();
        var categoryRepositoryMock = new Mock<ICategoryRepository>();
        var configuration = new MapperConfiguration(cfg => 
        {
            cfg.AddProfile(new Application.Mappings.MappingProfile());
        });
        var mapper = configuration.CreateMapper();
        var unitToWorkMock = new Mock<IUnitOfWork>();

        
        
        var model = new Product 
        { 
            Id = 1, 
            Name = "Teste", 
            Price = 10,
            CategoryId = 1
        };
        
        productRepositoryMock
            .Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(model);
        productRepositoryMock
            .Setup(repo => repo.ExistByNameAsync("Outro Teste"))
            .ReturnsAsync(false);
        
        var service = new ProductService(productRepositoryMock.Object, categoryRepositoryMock.Object, mapper, unitToWorkMock.Object);
        
        //ACT
        var fakeDto = new UpdateProductDto { Name = "Outro Teste",  Price = 10 };
        var result = await service.UpdateAsync(1, fakeDto);
        
        //ASSERT
        Assert.NotNull(result);
        Assert.Equal(Application.Enums.TypeResult.Success, result.TypeResult);
        Assert.Equal("Outro Teste", result.Data?.Name);
    }

    [Fact]
    public async Task AddAsync_ShouldReturnError_WhenNameIsDuplicate()
    {
        //ARRANGE
        var productRepositoryMock = new Mock<IProductRepository>();
        var categoryRepositoryMock = new Mock<ICategoryRepository>();
        var configuration = new MapperConfiguration(cfg => 
        {
            cfg.AddProfile(new Application.Mappings.MappingProfile());
        });
        var mapper = configuration.CreateMapper();
        var unitToWorkMock = new Mock<IUnitOfWork>();


        var model = new CreateProductDto
        {
            Name = "Teste",
            Price = 10,
            CategoryId = 1
        };
        
        productRepositoryMock
            .Setup(repo => repo.ExistByNameAsync(model.Name))
            .ReturnsAsync(true);
        
        var service = new ProductService(productRepositoryMock.Object, categoryRepositoryMock.Object, mapper, unitToWorkMock.Object);
        
        //ACT
        var result = await service.AddAsync(model);
        
        //ASSERT
        Assert.NotNull(result);
        Assert.Equal(Application.Enums.TypeResult.Duplicated ,result.TypeResult);
    }

    [Fact]
    public async Task AddAsync_ShouldReturnSuccess_WhenDataIsValid()
    {
        //ARRANGE
        var productRepositoryMock = new Mock<IProductRepository>();
        var categoryRepositoryMock = new Mock<ICategoryRepository>();
        var mapperMock = new MapperConfiguration(cfg => 
        {
            cfg.AddProfile(new Application.Mappings.MappingProfile());
        });
        var mapper = mapperMock.CreateMapper();
        var unitToWorkMock = new Mock<IUnitOfWork>();

        var model = new CreateProductDto()
        {
            Name = "Teste",
            Price = 10,
            CategoryId = 1
        };
        
        productRepositoryMock
            .Setup(repo => repo.ExistByNameAsync(model.Name))
            .ReturnsAsync(false);
        
        categoryRepositoryMock
            .Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(new Category { Id = 1, Name = "Categoria Teste" });
        
        var service = new ProductService(productRepositoryMock.Object, categoryRepositoryMock.Object, mapper, unitToWorkMock.Object);
        
        //ACT
        var result = await service.AddAsync(model);
        
        //ASSERT
        Assert.NotNull(result.Data);
        Assert.Equal(Application.Enums.TypeResult.Created, result.TypeResult);
        Assert.Equal(model.Name, result.Data.Name);
        Assert.Equal(model.Price, result.Data.Price);
    }
}