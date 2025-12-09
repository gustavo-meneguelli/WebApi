using Application.Features.Categories.DTOs;
using Application.Features.Products.Repositories;
using Application.Features.Categories.Repositories;
using Application.Features.Auth.Repositories;
using Domain.Constants;
using FluentValidation;

namespace Application.Features.Categories.Validators;

public class UpdateCategoryDtoValidator : AbstractValidator<UpdateCategoryDto>
{
    public UpdateCategoryDtoValidator(ICategoryRepository categoryRepository)
    {
        // Update parcial: valida nome APENAS se informado
        RuleFor(c => c.Name)
            .MinimumLength(3).WithMessage("O nome deve ter no mínimo 3 caracteres.")
            .MaximumLength(50).WithMessage("O nome deve ter no máximo 50 caracteres.")
            .When(c => c.Name != string.Empty);

        // Verifica unicidade apenas se nome informado
        RuleFor(c => c.Name)
            .Cascade(CascadeMode.Stop)
            .MustAsync(async (name, _) =>
            {
                bool exists = await categoryRepository.ExistsByNameAsync(name);
                return !exists;
            })
            .WithMessage(string.Format(ErrorMessages.AlreadyExists, "categoria", "nome"))
            .When(c => c.Name != string.Empty);
    }
}

