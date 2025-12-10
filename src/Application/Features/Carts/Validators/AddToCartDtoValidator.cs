using Application.Features.Carts.DTOs;
using Application.Features.Products.Repositories;
using FluentValidation;

namespace Application.Features.Carts.Validators;

public class AddToCartDtoValidator : AbstractValidator<AddToCartDto>
{
    public AddToCartDtoValidator(IProductRepository productRepository)
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0).WithMessage("ProductId deve ser maior que zero")
            .MustAsync(async (id, ct) => await productRepository.GetByIdAsync(id) != null)
            .WithMessage("Produto não encontrado");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantidade deve ser maior que zero")
            .LessThanOrEqualTo(99).WithMessage("Quantidade máxima: 99 unidades");
    }
}
