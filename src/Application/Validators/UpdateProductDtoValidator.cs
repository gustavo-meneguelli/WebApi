using Application.DTO.Products;
using Application.Interfaces.Repositories;
using Domain.Constants;
using FluentValidation;

namespace Application.Validators;

public class UpdateProductDtoValidator : AbstractValidator<UpdateProductDto>
{
    public UpdateProductDtoValidator(IProductRepository productRepository, ICategoryRepository categoryRepository)
    {
        // Validação de Nome - APENAS se for informado (update parcial)
        RuleFor(p => p.Name)
            .MinimumLength(3).WithMessage("O nome deve ter no mínimo 3 caracteres.")
            .MaximumLength(50).WithMessage("O nome deve ter no máximo 50 caracteres.")
            .When(p => p.Name != string.Empty);
        
        RuleFor(p => p.Name)
            .Cascade(CascadeMode.Stop)
            .MustAsync(async (name, _) =>
            {
                bool exists = await productRepository.ExistByNameAsync(name);
                return !exists;
            })
            .WithMessage(string.Format(ErrorMessages.AlreadyExists, "produto", "nome"))
            .When(p => p.Name != string.Empty);
        
        // Validação de Preço - APENAS se for informado
        RuleFor(p => p.Price)
            .GreaterThan(0).WithMessage("O preço deve ser maior que zero.")
            .LessThan(100000).WithMessage("O preço deve ser menor que R$ 100.000,00")
            .When(p => p.Price != 0);
        
        // Validação de CategoryId - APENAS se for informado
        RuleFor(p => p.CategoryId)
            .GreaterThan(0).WithMessage("ID da categoria inválido.")
            .When(p => p.CategoryId != 0);
        
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
