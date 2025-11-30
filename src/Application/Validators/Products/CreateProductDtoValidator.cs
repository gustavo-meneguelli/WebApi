using Application.DTOs.Products;
using FluentValidation;

namespace Application.Validators.Products;

public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductDtoValidator()
    {
        RuleFor(product => product.Name)
            .MaximumLength(50).WithMessage("Name must not exceed 50 characters")
            .MinimumLength(3).WithMessage("The name must contain at least 3 characters.");
        
        RuleFor(product => product.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero")
            .LessThan(100000).WithMessage("Price must be less than R$100.000,00");
    }
}