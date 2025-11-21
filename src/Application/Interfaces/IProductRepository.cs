using Domain.Models;

namespace Application.Interfaces;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task<Product?> GetByNameAsync(string name);
    Task<Product> AddAsync(Product product);
    Task UpdateAsync(Product product);
    Task<bool> ExistByNameAsync(string name);
}