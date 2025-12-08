using Application.Common.Models;
using Application.DTO.Categories;
using Application.Interfaces.Generics;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Constants;
using Domain.Entities;

namespace Application.Services;

public class CategoryService(ICategoryRepository repository, IMapper mapper, IUnitOfWork unitOfWork) : ICategoryService
{
    public async Task<Result<IEnumerable<CategoryResponseDto>>> GetAllAsync()
    {
        var categories = await repository.GetAllAsync();
        
        var response = mapper.Map<IEnumerable<CategoryResponseDto>>(categories);
        
        return Result<IEnumerable<CategoryResponseDto>>.Success(response);
    }

    public async Task<Result<CategoryResponseDto>> GetByIdAsync(int id)
    {
        var category = await repository.GetByIdAsync(id);

        if (category is null)
        {
            return Result<CategoryResponseDto>.NotFound(string.Format(ErrorMessages.NotFound, "Categoria"));
        }

        var dto = mapper.Map<CategoryResponseDto>(category);
        
        return Result<CategoryResponseDto>.Success(dto);
    }

    public async Task<Result<CategoryResponseDto>> AddAsync(CreateCategoryDto dto)
    {
        
        var categoryExists = await repository.ExistsByNameAsync(dto.Name);

        if (categoryExists)
        {
            return Result<CategoryResponseDto>.Duplicate(string.Format(ErrorMessages.AlreadyExists, "Categoria", "nome"));
        }
        
        var category = mapper.Map<Category>(dto);
        
        await repository.AddAsync(category);

        await unitOfWork.CommitAsync();
        
        var responseDto = mapper.Map<CategoryResponseDto>(category);
        
        return Result<CategoryResponseDto>.Created(responseDto);
    }
}