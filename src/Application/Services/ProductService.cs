using Application.Common.Models;
using Application.DTO.Products;
using Application.Interfaces.Generics;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Constants;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class ProductService(
    IProductRepository productRepository,
    IMapper mapper,
    IUnitOfWork unitOfWork) : IProductService
{
    public async Task<Result<PagedResult<ProductResponseDto>>> GetAllAsync(PaginationParams pagination)
    {
        var pagedResult = await productRepository.GetAllAsync(
            pagination,
            filter: null,
            include: query => query.Include(p => p.Category)!);

        var listProductResponseDtos = mapper.Map<IEnumerable<ProductResponseDto>>(pagedResult.Items);

        var resultDto = new PagedResult<ProductResponseDto>
        {
            Items = listProductResponseDtos,
            TotalCount = pagedResult.TotalCount,
            PageSize = pagedResult.PageSize,
            CurrentPage = pagedResult.CurrentPage,
            TotalPages = pagedResult.TotalPages
        };

        return Result<PagedResult<ProductResponseDto>>.Success(resultDto);
    }

    public async Task<Result<ProductResponseDto?>> GetByIdAsync(int id)
    {
        var product = await productRepository.GetByIdAsync(id);

        if (product is null)
        {
            return Result<ProductResponseDto?>.NotFound(string.Format(ErrorMessages.NotFound, "Produto"));
        }

        var productResponseDto = mapper.Map<Product, ProductResponseDto>(product);

        return Result<ProductResponseDto?>.Success(productResponseDto);

    }

    public async Task<Result<ProductResponseDto>> AddAsync(CreateProductDto dto)
    {
        var product = mapper.Map<Product>(dto);

        await productRepository.AddAsync(product);
    
        await unitOfWork.CommitAsync();

        var productResponseDto = mapper.Map<Product, ProductResponseDto>(product);

        return Result<ProductResponseDto>.Created(productResponseDto);
    }

    public async Task<Result<ProductResponseDto?>> UpdateAsync(int id, UpdateProductDto dto)
    {
        var product = await productRepository.GetByIdAsync(id);

        if (product is null)
        {
            return Result<ProductResponseDto?>.NotFound(string.Format(ErrorMessages.NotFound, "Produto"));
        }

        mapper.Map(dto, product);
        await productRepository.UpdateAsync(product);
        await unitOfWork.CommitAsync();

        var productResponseDto = mapper.Map<Product, ProductResponseDto>(product);
        return Result<ProductResponseDto?>.Success(productResponseDto);
    }

    public async Task<Result<ProductResponseDto?>> DeleteAsync(int id)
    {
        var product = await productRepository.GetByIdAsync(id);

        if (product is null)
        {
            return Result<ProductResponseDto?>.NotFound(string.Format(ErrorMessages.NotFound, "Produto"));
        }

        await productRepository.DeleteAsync(product);

        var productResponseDto = mapper.Map<Product, ProductResponseDto>(product);

        return Result<ProductResponseDto?>.Success(productResponseDto);
    }
}