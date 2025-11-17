using Application.Interfaces;
using Domain.Models;

namespace Application.Services;

public class ProductService(IProductRepository repository) : IProductService
{
    public IEnumerable<Product> GetAll()
    {
        return repository.GetProducts();
    }

    public Product? GetById(int id)
    {
        return repository.GetProduct(id);
    }

    public Product AddProduct(string productName, decimal price)
    {
        var product = new Product {  Name = productName, Price = price };
        repository.AddProduct(product);
        return product;
    }
}