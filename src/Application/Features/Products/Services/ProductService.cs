using Application.Common.Models;
using Application.Features.Products.DTOs;
using Application.Features.Products.Repositories;
using Application.Features.ProductReviews.Repositories;
using Application.Common.Interfaces;
using AutoMapper;
using Domain.Constants;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Products.Services;

public class ProductService(
    IProductRepository productRepository,
    IProductReviewRepository productReviewRepository,
    IMapper mapper,
    IUnitOfWork unitOfWork) : IProductService
{
    public async Task<Result<PagedResult<ProductResponseDto>>> GetAllAsync(PaginationParams pagination)
    {
        // Eager loading: carrega Category junto para evitar N+1 queries
        var pagedEntities = await productRepository.GetAllAsync(
            pagination,
            filter: null,
            include: query => query.Include(p => p.Category)!);

        var products = mapper.Map<IEnumerable<ProductResponseDto>>(pagedEntities.Items).ToList();

        // Busca ratings de todos os produtos em UMA query (evita N+1)
        var productIds = products.Select(p => p.Id).ToList();
        var ratingSummaries = await productReviewRepository.GetRatingSummaryBatchAsync(productIds);

        // Associa ratings aos produtos usando o dicionário
        foreach (var product in products)
        {
            if (ratingSummaries.TryGetValue(product.Id, out var summary))
            {
                product.AverageRating = summary.AverageRating;
                product.TotalReviews = summary.TotalReviews;
            }
            else
            {
                product.AverageRating = 0;
                product.TotalReviews = 0;
            }
        }

        var response = new PagedResult<ProductResponseDto>
        {
            Items = products,
            TotalCount = pagedEntities.TotalCount,
            PageSize = pagedEntities.PageSize,
            CurrentPage = pagedEntities.CurrentPage,
            TotalPages = pagedEntities.TotalPages
        };

        return Result<PagedResult<ProductResponseDto>>.Success(response);
    }

    public async Task<Result<ProductResponseDto?>> GetByIdAsync(int id)
    {
        var product = await productRepository.GetByIdAsync(id);

        if (product is null)
        {
            return Result<ProductResponseDto?>.NotFound(string.Format(ErrorMessages.NotFound, "Produto"));
        }

        var response = mapper.Map<Product, ProductResponseDto>(product);

        // Calcular média de avaliações
        var (averageRating, totalReviews) = await productReviewRepository.GetProductRatingSummaryAsync(id);
        response.AverageRating = averageRating;
        response.TotalReviews = totalReviews;

        return Result<ProductResponseDto?>.Success(response);

    }

    public async Task<Result<ProductResponseDto>> AddAsync(CreateProductDto dto)
    {
        var product = mapper.Map<Product>(dto);

        await productRepository.AddAsync(product);

        await unitOfWork.CommitAsync();

        var response = mapper.Map<Product, ProductResponseDto>(product);

        return Result<ProductResponseDto>.Created(response);
    }

    public async Task<Result<ProductResponseDto?>> UpdateAsync(int id, UpdateProductDto dto)
    {
        var product = await productRepository.GetByIdAsync(id);

        if (product is null)
        {
            return Result<ProductResponseDto?>.NotFound(string.Format(ErrorMessages.NotFound, "Produto"));
        }

        // Mapper atualiza apenas campos informados (update parcial)
        mapper.Map(dto, product);
        await productRepository.UpdateAsync(product);
        await unitOfWork.CommitAsync();

        var response = mapper.Map<Product, ProductResponseDto>(product);
        return Result<ProductResponseDto?>.Success(response);
    }

    public async Task<Result<ProductResponseDto?>> DeleteAsync(int id)
    {
        var product = await productRepository.GetByIdAsync(id);

        if (product is null)
        {
            return Result<ProductResponseDto?>.NotFound(string.Format(ErrorMessages.NotFound, "Produto"));
        }

        await productRepository.DeleteAsync(product);
        await unitOfWork.CommitAsync();

        // DELETE retorna 204 NoContent conforme padrão REST
        return Result<ProductResponseDto?>.NoContent();
    }
}

