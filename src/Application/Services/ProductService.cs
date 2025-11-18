using Application.DTO;
using Application.Interfaces;
using Application.Utilities;
using Domain.Models;

namespace Application.Services;

public class ProductService(IProductRepository repository) : IProductService
{
    public Result<IEnumerable<Product>> GetAll()
    {
        var products = repository.GetProducts();
        
        return Result<IEnumerable<Product>>.Success(products);
    }

    public Result<Product?> GetById(int id)
    {
        var product = repository.GetProduct(id);

        return product is null
            ? Result<Product?>.NotFound("No product were found with this ID.")
            : Result<Product?>.Success(product);
    }

    public Result<Product> AddProduct(CreateProductDto dto)
    {
        bool productExists = repository.GetProductByName(dto.Name) is not null;

        if (productExists)
        {
            return Result<Product>.Duplicate("Product with that name already exists.");
        }
        
        var product = new Product { Name = dto.Name, Price = dto.Price };
        repository.AddProduct(product);
        return Result<Product>.Created(product);
    }

    public Result<Product?> UpdateProduct(int id, UpdateProductDto dto)
    {
        var product = repository.GetProduct(id);

        if (product is null)
        {
            return Result<Product?>.NotFound("No products were found with this ID.");
        }

        var productNameAlreadyExists = repository.GetProductByName(dto.Name) is not null;

        if (productNameAlreadyExists && dto.Name != product.Name) 
        {
            return Result<Product?>.Duplicate("Product with that name already exists.");
        }
        
        if (dto.Name != product.Name && dto.Name != string.Empty)
        {
            product.Name = dto.Name;
        }
        
        if (dto.Price != product.Price && dto.Price != 0)
        {
            product.Price = dto.Price;
        }

        repository.UpdateProduct(product);
        return Result<Product?>.Success(product);
    }
}