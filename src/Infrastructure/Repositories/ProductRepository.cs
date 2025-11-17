using Application.Interfaces;
using Domain.Models;
using Infrastructure.Data;

namespace Infrastructure.Repositories;

public class ProductRepository(AppDbContext context) : IProductRepository
{
    
    public IEnumerable<Product> GetProducts()
    {
        return context.Products.ToList();
    }

    public Product? GetProduct(int id)
    {
        return context.Products.FirstOrDefault(p => p.Id == id);
    }

    public Product AddProduct(Product product)
    {
        context.Products.Add(product);
        context.SaveChanges();
        return product;
    }
}