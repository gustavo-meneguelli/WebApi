using Application.Features.Products.DTOs;
using Application.Features.Products.Repositories;
using Application.Features.Categories.Repositories;
using Domain.Constants;
using FluentValidation;

namespace Application.Features.Products.Validators;

public class UpdateProductDtoValidator : AbstractValidator<UpdateProductDto>
{
    public UpdateProductDtoValidator(IProductRepository productRepository, ICategoryRepository categoryRepository)
    {
        // Update parcial: nome vazio = não atualizar este campo
        RuleFor(p => p.Name)
            .MinimumLength(3).WithMessage("O nome deve ter no mínimo 3 caracteres.")
            .MaximumLength(50).WithMessage("O nome deve ter no máximo 50 caracteres.")
            .When(p => p.Name != string.Empty);

        // Verifica unicidade apenas se informado um nome novo
        RuleFor(p => p.Name)
            .Cascade(CascadeMode.Stop)
            .MustAsync(async (name, _) =>
            {
                bool exists = await productRepository.ExistByNameAsync(name);
                return !exists;
            })
            .WithMessage(string.Format(ErrorMessages.AlreadyExists, "produto", "nome"))
            .When(p => p.Name != string.Empty);

        // Update parcial: preço zero = não atualizar este campo
        RuleFor(p => p.Price)
            .GreaterThan(0).WithMessage("O preço deve ser maior que zero.")
            .LessThan(100000).WithMessage("O preço deve ser menor que R$ 100.000,00")
            .When(p => p.Price != 0);

        // Update parcial: categoryId zero = não atualizar este campo
        RuleFor(p => p.CategoryId)
            .GreaterThan(0).WithMessage("ID da categoria inválido.")
            .When(p => p.CategoryId != 0);

        // Verifica se categoria existe apenas quando informada
        RuleFor(p => p.CategoryId)
            .Cascade(CascadeMode.Stop)
            .MustAsync(async (id, _) =>
            {
                var category = await categoryRepository.GetByIdAsync(id);
                return category is not null;
            })
            .WithMessage(string.Format(ErrorMessages.NotFound, "Categoria"))
            .When(p => p.CategoryId != 0);
    }
}

