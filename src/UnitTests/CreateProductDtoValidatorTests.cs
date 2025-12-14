using Application.Features.Products.DTOs;
using Application.Features.Products.Repositories;
using Application.Features.Categories.Repositories;
using Application.Common.Interfaces;
using Application.Features.Products.Validators;
using Domain.Entities;
using FluentValidation.TestHelper;
using Moq;

namespace UnitTests;

public class CreateProductDtoValidatorTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly CreateProductDtoValidator _validator;

    public CreateProductDtoValidatorTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();

        _validator = new CreateProductDtoValidator(_productRepositoryMock.Object, _categoryRepositoryMock.Object);
    }

    [Fact]
    public async Task Validate_ShouldReturnError_WhenPriceIsNegative()
    {
        // ARRANGE
        var model = new CreateProductDto { Name = "Valid Name", Price = -10, CategoryId = 1 };

        // ACT
        var result = await _validator.TestValidateAsync(model);

        // ASSERT
        result.ShouldHaveValidationErrorFor(product => product.Price)
              .WithErrorMessage("O preço deve ser maior que zero.");
    }

    [Fact]
    public async Task Validate_ShouldReturnError_WhenNameIsDuplicated()
    {
        _productRepositoryMock
            .Setup(repo => repo.ExistByNameAsync("Teclado"))
            .ReturnsAsync(true);

        var model = new CreateProductDto { Name = "Teclado", Price = 100, CategoryId = 1 };

        // ACT
        var result = await _validator.TestValidateAsync(model);

        // ASSERT
        result.ShouldHaveValidationErrorFor(product => product.Name)
              .WithErrorMessage("Já existe um registro de produto com este nome.");
    }

    [Fact]
    public async Task Validate_ShouldReturnError_WhenCategoryDoesNotExist()
    {
        // ARRANGE
        _productRepositoryMock.Setup(x => x.ExistByNameAsync(It.IsAny<string>())).ReturnsAsync(false);

        _categoryRepositoryMock
            .Setup(repo => repo.GetByIdAsync(99))
            .ReturnsAsync((Category?)null);

        var model = new CreateProductDto { Name = "Mouse", Price = 50, CategoryId = 99 };

        // ACT
        var result = await _validator.TestValidateAsync(model);

        // ASSERT
        result.ShouldHaveValidationErrorFor(product => product.CategoryId)
              .WithErrorMessage("Categoria não foi encontrado(a).");
    }

    [Fact]
    public async Task Validate_ShouldReturnSuccess_WhenDataIsValid()
    {
        // ARRANGE
        _productRepositoryMock
            .Setup(repo => repo.ExistByNameAsync("Novo Produto"))
            .ReturnsAsync(false); // Não existe

        _categoryRepositoryMock
            .Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(new Category { Id = 1, Name = "Tech" }); // Existe

        var model = new CreateProductDto { Name = "Novo Produto", Price = 10, CategoryId = 1 };

        // ACT
        var result = await _validator.TestValidateAsync(model);

        // ASSERT
        result.ShouldNotHaveAnyValidationErrors();
    }
}
