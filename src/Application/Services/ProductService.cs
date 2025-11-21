using Application.DTO;
using Application.Interfaces;
using Application.Utilities;
using Domain.Models;

namespace Application.Services;

public class ProductService(IProductRepository repository) : IProductService
{
    public async Task<Result<IEnumerable<Product>>> GetAllAsync()
    {
        var products = await repository.GetAllAsync();
        
        return Result<IEnumerable<Product>>.Success(products);
    }

    public async Task<Result<Product?>> GetByIdAsync(int id)
    {
        var product = await repository.GetByIdAsync(id);

        return product is null
            ? Result<Product?>.NotFound("No product were found with this ID.")
            : Result<Product?>.Success(product);
    }

    public async Task<Result<Product>> AddAsync(CreateProductDto dto)
    {
        bool productExists = await repository.GetByNameAsync(dto.Name) is not null;

        if (productExists)
        {
            return Result<Product>.Duplicate("Product with that name already exists.");
        }
        
        var product = new Product { Name = dto.Name, Price = dto.Price };
        await repository.AddAsync(product);
        return Result<Product>.Created(product);
    }

    public async Task<Result<Product?>> UpdateAsync(int id, UpdateProductDto dto)
    {
        var product = await repository.GetByIdAsync(id);

        if (product is null)
        {
            return Result<Product?>.NotFound("No products were found with this ID.");
        }
        
        bool isNecessaryChangeName = dto.Name != string.Empty && dto.Name != product.Name;

        if (isNecessaryChangeName)
        {
            bool nameExists = await repository.ExistByNameAsync(dto.Name);

            if (nameExists)
            {
                return Result<Product?>.Duplicate("Product with that name already exists.");
            }

            product.Name = dto.Name;
        }
        
        bool isNecessaryChangePrice = dto.Price != product.Price && dto.Price != 0;

        if (isNecessaryChangePrice)
        {
            product.Price = dto.Price;
        }

        await repository.UpdateAsync(product);
        return Result<Product?>.Success(product);
    }
}