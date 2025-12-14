using Application.Common.Models;
using Application.Common.Interfaces;
using Domain.Entities;

namespace Application.Features.Products.Repositories;

public interface IProductRepository : IRepository<Product>
{
    Task<Product?> GetByNameAsync(string name);
    Task<bool> ExistByNameAsync(string name);
}
