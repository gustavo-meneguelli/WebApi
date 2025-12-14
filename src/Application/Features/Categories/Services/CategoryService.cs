using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Features.Categories.DTOs;
using Application.Features.Categories.Repositories;
using AutoMapper;
using Domain.Constants;
using Domain.Entities;

namespace Application.Features.Categories.Services;

public class CategoryService(ICategoryRepository categoryRepository, IMapper mapper, IUnitOfWork unitOfWork) : ICategoryService
{
    public async Task<Result<PagedResult<CategoryResponseDto>>> GetAllAsync(PaginationParams paginationParams)
    {
        var pagedEntities = await categoryRepository.GetAllAsync(paginationParams);

        var categories = mapper.Map<IEnumerable<CategoryResponseDto>>(pagedEntities.Items);

        var response = new PagedResult<CategoryResponseDto>
        {
            Items = categories,
            CurrentPage = pagedEntities.CurrentPage,
            PageSize = pagedEntities.PageSize,
            TotalCount = pagedEntities.TotalCount,
            TotalPages = pagedEntities.TotalPages
        };

        return Result<PagedResult<CategoryResponseDto>>.Success(response);
    }

    public async Task<Result<CategoryResponseDto>> GetByIdAsync(int id)
    {
        var category = await categoryRepository.GetByIdAsync(id);

        if (category is null)
        {
            return Result<CategoryResponseDto>.NotFound(string.Format(ErrorMessages.NotFound, "Categoria"));
        }

        var response = mapper.Map<CategoryResponseDto>(category);

        return Result<CategoryResponseDto>.Success(response);
    }

    public async Task<Result<CategoryResponseDto>> AddAsync(CreateCategoryDto dto)
    {
        var category = mapper.Map<Category>(dto);

        await categoryRepository.AddAsync(category);

        await unitOfWork.CommitAsync();

        var response = mapper.Map<CategoryResponseDto>(category);

        return Result<CategoryResponseDto>.Created(response);
    }

    public async Task<Result<CategoryResponseDto?>> UpdateAsync(int id, UpdateCategoryDto dto)
    {
        var category = await categoryRepository.GetByIdAsync(id);

        if (category is null)
        {
            return Result<CategoryResponseDto?>.NotFound(
                string.Format(ErrorMessages.NotFound, "Categoria"));
        }

        // Mapper atualiza apenas campos informados (update parcial)
        mapper.Map(dto, category);
        await categoryRepository.UpdateAsync(category);
        await unitOfWork.CommitAsync();

        var response = mapper.Map<CategoryResponseDto>(category);
        return Result<CategoryResponseDto?>.Success(response);
    }

    public async Task<Result<CategoryResponseDto?>> DeleteAsync(int id)
    {
        var category = await categoryRepository.GetByIdAsync(id);

        if (category is null)
        {
            return Result<CategoryResponseDto?>.NotFound(
                string.Format(ErrorMessages.NotFound, "Categoria"));
        }

        await categoryRepository.DeleteAsync(category);
        await unitOfWork.CommitAsync();

        // DELETE retorna 204 NoContent conforme padrão REST
        return Result<CategoryResponseDto?>.NoContent();
    }
}


