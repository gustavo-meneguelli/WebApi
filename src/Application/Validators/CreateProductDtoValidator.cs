using Application.DTO.Products;
using Application.Interfaces.Repositories;
using Domain.Constants;
using FluentValidation;

namespace Application.Validators;

public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductDtoValidator(IProductRepository productRepository, ICategoryRepository categoryRepository)
    {
        RuleFor(p => p.Name)
            .NotEmpty().WithMessage(string.Format(ErrorMessages.RequiredField, "Nome"))
            .MinimumLength(3).WithMessage("O nome deve ter no mínimo 3 caracteres.")
            .MaximumLength(50).WithMessage("O nome deve ter no máximo 50 caracteres.");

        RuleFor(p => p.Price)
            .GreaterThan(0).WithMessage("O preço deve ser maior que zero.");

        RuleFor(p => p.Name)
            .Cascade(CascadeMode.Stop) 
            .MustAsync(async (name, _) =>
            {
                bool exists = await productRepository.ExistByNameAsync(name);
                return !exists;
            })
            .WithMessage(string.Format(ErrorMessages.AlreadyExists, "produto", "nome"));

        RuleFor(p => p.CategoryId)
            .Cascade(CascadeMode.Stop)
            .GreaterThan(0).WithMessage("ID da categoria inválido.")
            .MustAsync(async (id, _) =>
            {
                var category = await categoryRepository.GetByIdAsync(id);
                return category is not null;
            })
            .WithMessage(string.Format(ErrorMessages.NotFound, "Categoria"));
    }
}