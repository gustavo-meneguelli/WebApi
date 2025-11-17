using Application.Interfaces;
using Domain.Models;

namespace Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    // Lista temporária enquanto não houver banco de dados
    private readonly IEnumerable<Product> _temporaryList = new List<Product> 
    {
        new Product { Id = 1, Name = "Notebook Acer Nitro V15", Price = 7000 },
        new Product { Id = 2, Name = "Notebook Asus Tuf Gaming", Price = 9000 },
        new Product { Id = 3, Name = "Notebook Acer Le Novo Loq", Price = 5000 },
        new Product { Id = 4, Name = "Notebook AlienWare Aurora", Price = 15000 },
    };
    
    public IEnumerable<Product> GetProducts()
    {
        return  _temporaryList;
    }

    public Product? GetProduct(int id)
    {
        return _temporaryList.FirstOrDefault(p => p.Id == id);
    }

    public Product AddProduct(Product product)
    {
        _temporaryList.ToList().Add(product);
        return product;
    }
}