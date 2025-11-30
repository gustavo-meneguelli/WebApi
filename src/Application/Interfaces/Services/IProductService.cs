using Application.Common.Models;
using Application.DTOs.Products;
using Domain.Entities;

namespace Application.Interfaces.Services;

public interface IProductService
{
    Task<Result<PagedResult<Product>>> GetAllAsync(PaginationParams paginationParams);
    Task<Result<Product?>> GetByIdAsync(int id);
    Task<Result<Product>> AddAsync(CreateProductDto dto);
    Task<Result<Product?>> UpdateAsync(int id, UpdateProductDto dto);
    Task<Result<Product?>> DeleteAsync(int id);
}