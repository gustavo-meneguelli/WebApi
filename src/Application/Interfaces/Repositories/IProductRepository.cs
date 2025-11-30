using Application.Common.Models;
using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IProductRepository
{
    Task<PagedResult<Product>> GetAllAsync(PaginationParams paginationParams);
    Task<Product?> GetByIdAsync(int id);
    Task<Product?> GetByNameAsync(string name);
    Task<Product> AddAsync(Product product);
    Task UpdateAsync(Product product);
    Task<bool> ExistByNameAsync(string name);
    Task DeleteAsync(Product product);
}