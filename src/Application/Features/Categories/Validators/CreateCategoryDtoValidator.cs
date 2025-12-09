using Application.Features.Categories.DTOs;
using Application.Features.Categories.Repositories;
using Domain.Constants;
using FluentValidation;

namespace Application.Features.Categories.Validators;

public class CreateCategoryDtoValidator : AbstractValidator<CreateCategoryDto>
{
    public CreateCategoryDtoValidator(ICategoryRepository categoryRepository)
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome é obrigatório.")
            .MinimumLength(3).WithMessage("O nome deve ter no mínimo 3 caracteres.")
            .MaximumLength(50).WithMessage("O nome deve ter no máximo 50 caracteres.");

        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .MustAsync(async (name, _) =>
            {
                bool exists = await categoryRepository.ExistsByNameAsync(name);
                return !exists;
            })
            .WithMessage(string.Format(ErrorMessages.AlreadyExists, "Categoria", "nome"));
    }
}

