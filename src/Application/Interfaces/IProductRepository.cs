using Domain.Models;

namespace Application.Interfaces;

public interface IProductRepository
{
    IEnumerable<Product> GetProducts();
    Product? GetProduct(int id);
    Product? GetProductByName(string name);
    Product AddProduct(Product product);
    Product UpdateProduct(Product product);
}