using Application.DTO.Products;
using Application.Interfaces.Repositories;
using Application.Validators;
using Domain.Entities;
using Moq;

namespace UnitTests;

public class UpdateProductDtoValidatorTests
{
    [Fact]
    public async Task Validate_ShouldFail_WhenNameAlreadyExists()
    {
        // ARRANGE
        var productRepositoryMock = new Mock<IProductRepository>();
        var categoryRepositoryMock = new Mock<ICategoryRepository>();

        // Simula que o nome "Produto Duplicado" já existe no banco
        productRepositoryMock
            .Setup(repo => repo.ExistByNameAsync("Produto Duplicado"))
            .ReturnsAsync(true);

        var validator = new UpdateProductDtoValidator(
            productRepositoryMock.Object,
            categoryRepositoryMock.Object);

        var dto = new UpdateProductDto 
        { 
            Name = "Produto Duplicado", 
            Price = 100, 
            CategoryId = 1 
        };

        // ACT
        var result = await validator.ValidateAsync(dto);

        // ASSERT
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name");
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("Já existe"));
    }

    [Fact]
    public async Task Validate_ShouldPass_WhenNameIsUnique()
    {
        // ARRANGE
        var productRepositoryMock = new Mock<IProductRepository>();
        var categoryRepositoryMock = new Mock<ICategoryRepository>();

        // Simula que o nome "Produto Novo" NÃO existe no banco
        productRepositoryMock
            .Setup(repo => repo.ExistByNameAsync("Produto Novo"))
            .ReturnsAsync(false);

        // Simula que a categoria existe
        categoryRepositoryMock
            .Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(new Category { Id = 1, Name = "Categoria Teste" });

        var validator = new UpdateProductDtoValidator(
            productRepositoryMock.Object,
            categoryRepositoryMock.Object);

        var dto = new UpdateProductDto 
        { 
            Name = "Produto Novo", 
            Price = 100, 
            CategoryId = 1 
        };

        // ACT
        var result = await validator.ValidateAsync(dto);

        // ASSERT
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenCategoryDoesNotExist()
    {
        // ARRANGE
        var productRepositoryMock = new Mock<IProductRepository>();
        var categoryRepositoryMock = new Mock<ICategoryRepository>();

        // Simula que o nome é único
        productRepositoryMock
            .Setup(repo => repo.ExistByNameAsync("Produto Teste"))
            .ReturnsAsync(false);

        // Simula que a categoria NÃO existe
        categoryRepositoryMock
            .Setup(repo => repo.GetByIdAsync(999))
            .ReturnsAsync((Category?)null);

        var validator = new UpdateProductDtoValidator(
            productRepositoryMock.Object,
            categoryRepositoryMock.Object);

        var dto = new UpdateProductDto 
        { 
            Name = "Produto Teste", 
            Price = 100, 
            CategoryId = 999 
        };

        // ACT
        var result = await validator.ValidateAsync(dto);

        // ASSERT
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "CategoryId");
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("não foi encontrado"));
    }

    [Fact]
    public async Task Validate_ShouldNotValidateName_WhenNameIsEmpty()
    {
        // ARRANGE - Update parcial: nome vazio não deve ser validado
        var productRepositoryMock = new Mock<IProductRepository>();
        var categoryRepositoryMock = new Mock<ICategoryRepository>();

        var validator = new UpdateProductDtoValidator(
            productRepositoryMock.Object,
            categoryRepositoryMock.Object);

        var dto = new UpdateProductDto 
        { 
            Name = string.Empty,  // Não quer atualizar o nome
            Price = 100, 
            CategoryId = 0  // Não quer atualizar a categoria
        };

        // ACT
        var result = await validator.ValidateAsync(dto);

        // ASSERT
        Assert.True(result.IsValid); // Deve passar pois campos vazios não são validados
        
        // Verifica que ExistByNameAsync NÃO foi chamado (validação condicional .When())
        productRepositoryMock.Verify(
            repo => repo.ExistByNameAsync(It.IsAny<string>()), 
            Times.Never);
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenPriceIsInvalid()
    {
        // ARRANGE
        var productRepositoryMock = new Mock<IProductRepository>();
        var categoryRepositoryMock = new Mock<ICategoryRepository>();

        var validator = new UpdateProductDtoValidator(
            productRepositoryMock.Object,
            categoryRepositoryMock.Object);

        var dto = new UpdateProductDto 
        { 
            Name = string.Empty, 
            Price = -10,  // Preço negativo - inválido
            CategoryId = 0 
        };

        // ACT
        var result = await validator.ValidateAsync(dto);

        // ASSERT
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Price");
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("maior que zero"));
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenPriceExceedsMaximum()
    {
        // ARRANGE
        var productRepositoryMock = new Mock<IProductRepository>();
        var categoryRepositoryMock = new Mock<ICategoryRepository>();

        var validator = new UpdateProductDtoValidator(
            productRepositoryMock.Object,
            categoryRepositoryMock.Object);

        var dto = new UpdateProductDto 
        { 
            Name = string.Empty, 
            Price = 150000,  // Preço acima do limite (100.000)
            CategoryId = 0 
        };

        // ACT
        var result = await validator.ValidateAsync(dto);

        // ASSERT
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Price");
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("100.000"));
    }
}
