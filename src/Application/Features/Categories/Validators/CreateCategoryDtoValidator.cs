using Application.Features.Categories.DTOs;
using Application.Features.Categories.Repositories;
using Domain.Constants;
using FluentValidation;

namespace Application.Features.Categories.Validators;

public class CreateCategoryDtoValidator : AbstractValidator<CreateCategoryDto>
{
    public CreateCategoryDtoValidator(ICategoryRepository categoryRepository)
    {
        // Validação de Name: sync primeiro, async só se passar nas básicas
        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage(string.Format(ErrorMessages.RequiredField, "name"))
            .MinimumLength(3).WithMessage(string.Format(ErrorMessages.MinLength, "name", 3))
            .MaximumLength(50).WithMessage(string.Format(ErrorMessages.MaxLength, "name", 50))
            .MustAsync(async (name, _) =>
            {
                bool exists = await categoryRepository.ExistsByNameAsync(name);
                return !exists;
            })
            .WithMessage(string.Format(ErrorMessages.AlreadyExists, "category", "name"));
    }
}

