
using Domain.Entities;
using Application.Common.Interfaces;


namespace Application.Features.Products.Repositories;

public interface IProductRepository : IRepository<Product>
{
    Task<Product?> GetByNameAsync(string name);
    Task<bool> ExistByNameAsync(string name);
}
