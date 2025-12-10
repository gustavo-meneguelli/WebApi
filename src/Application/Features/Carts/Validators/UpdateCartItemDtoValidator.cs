using Application.Features.Carts.DTOs;
using FluentValidation;

namespace Application.Features.Carts.Validators;

public class UpdateCartItemDtoValidator : AbstractValidator<UpdateCartItemDto>
{
    public UpdateCartItemDtoValidator()
    {
        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantidade deve ser maior que zero")
            .LessThanOrEqualTo(99).WithMessage("Quantidade máxima: 99 unidades");
    }
}
