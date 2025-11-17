using Domain.Models;

namespace Application.Interfaces;

public interface IProductRepository
{
    IEnumerable<Product> GetProducts();
    Product? GetProduct(int id);
    Product AddProduct(Product product);
}