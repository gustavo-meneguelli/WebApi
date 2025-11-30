using Application.DTOs.Products;
using Application.Validators.Products;

namespace UnitTests;

public class CreateProductDtoValidatorTests
{
    [Fact]
    public async Task Validate_ShouldReturnError_WhenPriceIsNegative()
    {
        // Arrange
        var validator = new CreateProductDtoValidator(); 

        var model = new CreateProductDto 
        { 
            Name = "Produto Teste", 
            Price = -10 
        };
        
        //ACT
        var result = await validator.ValidateAsync(model);
        
        //ASSERT
        Assert.False(result.IsValid);
    }
    
    [Fact]
    public async Task Validate_ShouldReturnSuccess_WhenProductIsValid()
    {
        //ARRANGE
        var validator = new CreateProductDtoValidator();
        
        var model = new CreateProductDto 
        { 
            Name = "Produto Teste", 
            Price = 10 
        };
        
        //ACT
        var result = await validator.ValidateAsync(model);
        
        //ASSERT
        Assert.True(result.IsValid);
    }
}