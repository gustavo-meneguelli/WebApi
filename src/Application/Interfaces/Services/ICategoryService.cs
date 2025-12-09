using Application.Common.Models;
using Application.DTO.Categories;

namespace Application.Interfaces.Services;

public interface ICategoryService
{
    Task<Result<PagedResult<CategoryResponseDto>>> GetAllAsync(PaginationParams paginationParams);
    
    Task<Result<CategoryResponseDto>> GetByIdAsync(int id);
    
    Task<Result<CategoryResponseDto>> AddAsync(CreateCategoryDto dto);
    
    Task<Result<CategoryResponseDto?>> UpdateAsync(int id, UpdateCategoryDto dto);
    
    Task<Result<CategoryResponseDto?>> DeleteAsync(int id);
}