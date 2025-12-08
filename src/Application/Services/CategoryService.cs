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
    public async Task<Result<PagedResult<CategoryResponseDto>>> GetAllAsync(PaginationParams paginationParams)
    {
        var pagedCategories = await repository.GetAllAsync(paginationParams);
        
        var categoriesDto = mapper.Map<IEnumerable<CategoryResponseDto>>(pagedCategories.Items);
        
        var pagedResult = new PagedResult<CategoryResponseDto>
        {
            Items = categoriesDto,
            CurrentPage = pagedCategories.CurrentPage,
            PageSize = pagedCategories.PageSize,
            TotalCount = pagedCategories.TotalCount,
            TotalPages = pagedCategories.TotalPages
        };
        
        return Result<PagedResult<CategoryResponseDto>>.Success(pagedResult);
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
        var category = mapper.Map<Category>(dto);
        
        await repository.AddAsync(category);

        await unitOfWork.CommitAsync();
        
        var responseDto = mapper.Map<CategoryResponseDto>(category);
        
        return Result<CategoryResponseDto>.Created(responseDto);
    }
}