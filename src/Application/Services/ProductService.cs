using Application.Common.Models;
using Application.DTOs.Products;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Entities;

namespace Application.Services;

public class ProductService(IProductRepository repository, IMapper mapper) : IProductService
{
    public async Task<Result<PagedResult<Product>>> GetAllAsync(PaginationParams pagination)
    {
        var pagedResult = await repository.GetAllAsync(pagination);
        
        return Result<PagedResult<Product>>.Success(pagedResult);
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
        bool productExists = await repository.ExistByNameAsync(dto.Name);

        if (productExists)
        {
            return Result<Product>.Duplicate("Product with that name already exists.");
        }
        
        var product = mapper.Map<Product>(dto);
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
        }

        mapper.Map(dto, product);
        
        await repository.UpdateAsync(product);
        return Result<Product?>.Success(product);
    }

    public async Task<Result<Product?>> DeleteAsync(int id)
    {
        var product = await repository.GetByIdAsync(id);

        if (product is null)
        {
            return Result<Product?>.NotFound("No product were found with this ID.");
        }
        
        await repository.DeleteAsync(product);
        
        return Result<Product?>.Success(product);
    }
}