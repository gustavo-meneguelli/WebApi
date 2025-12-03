using Application.Common.Models;
using Application.Interfaces.Generics;
using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IProductRepository : IRepository<Product>
{
    Task<PagedResult<Product>> GetAllAsync(PaginationParams paginationParams);
    Task<Product?> GetByNameAsync(string name);
    Task<bool> ExistByNameAsync(string name);
}