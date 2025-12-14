using Application.Common.Models;
using Application.Features.Products.DTOs;

namespace Application.Features.Products.Services;

public interface IProductService
{
    Task<Result<PagedResult<ProductResponseDto>>> GetAllAsync(PaginationParams paginationParams);
    Task<Result<ProductResponseDto?>> GetByIdAsync(int id);
    Task<Result<ProductResponseDto>> AddAsync(CreateProductDto dto);
    Task<Result<ProductResponseDto?>> UpdateAsync(int id, UpdateProductDto dto);
    Task<Result<ProductResponseDto?>> DeleteAsync(int id);
}

