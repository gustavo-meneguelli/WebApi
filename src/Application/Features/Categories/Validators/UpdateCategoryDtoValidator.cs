using Application.Features.Categories.DTOs;
using Application.Features.Categories.Repositories;
using Domain.Constants;
using FluentValidation;

namespace Application.Features.Categories.Validators;

public class UpdateCategoryDtoValidator : AbstractValidator<UpdateCategoryDto>
{
    public UpdateCategoryDtoValidator(ICategoryRepository categoryRepository)
    {
        // Validação de Name: update parcial, só valida se informado
        RuleFor(c => c.Name)
            .Cascade(CascadeMode.Stop)
            .MinimumLength(3).WithMessage(string.Format(ErrorMessages.MinLength, "name", 3))
            .MaximumLength(50).WithMessage(string.Format(ErrorMessages.MaxLength, "name", 50))
            .MustAsync(async (name, _) =>
            {
                bool exists = await categoryRepository.ExistsByNameAsync(name);
                return !exists;
            })
            .WithMessage(string.Format(ErrorMessages.AlreadyExists, "category", "name"))
            .When(c => c.Name != string.Empty);
    }
}

