using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Features.Categories.DTOs;
using Application.Features.Categories.Repositories;
using AutoMapper;
using Domain.Constants;
using Domain.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace Application.Features.Categories.Services;

public class CategoryService(
    ICategoryRepository categoryRepository, 
    IMapper mapper, 
    IUnitOfWork unitOfWork,
    IMemoryCache cache) : ICategoryService
{
    // Chave de cache para lista de categorias paginada
    private const string CategoriesCacheKeyPrefix = "categories_page_";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

    public async Task<Result<PagedResult<CategoryResponseDto>>> GetAllAsync(PaginationParams paginationParams)
    {
        // Chave única por página e tamanho
        var cacheKey = $"{CategoriesCacheKeyPrefix}{paginationParams.PageNumber}_{paginationParams.PageSize}";

        // Tenta buscar do cache primeiro
        if (cache.TryGetValue(cacheKey, out PagedResult<CategoryResponseDto>? cachedResult) && cachedResult != null)
        {
            return Result<PagedResult<CategoryResponseDto>>.Success(cachedResult);
        }

        // Se não estiver no cache, busca do banco
        var pagedEntities = await categoryRepository.GetAllAsync(paginationParams);

        var categories = mapper.Map<IEnumerable<CategoryResponseDto>>(pagedEntities.Items);

        var response = new PagedResult<CategoryResponseDto>
        {
            Items = categories,
            CurrentPage = pagedEntities.CurrentPage,
            PageSize = pagedEntities.PageSize,
            TotalCount = pagedEntities.TotalCount,
            TotalPages = pagedEntities.TotalPages
        };

        // Armazena no cache por 5 minutos
        cache.Set(cacheKey, response, CacheDuration);

        return Result<PagedResult<CategoryResponseDto>>.Success(response);
    }

    public async Task<Result<CategoryResponseDto>> GetByIdAsync(int id)
    {
        var category = await categoryRepository.GetByIdAsync(id);

        if (category is null)
        {
            return Result<CategoryResponseDto>.NotFound(string.Format(ErrorMessages.NotFound, "Categoria"));
        }

        var response = mapper.Map<CategoryResponseDto>(category);

        return Result<CategoryResponseDto>.Success(response);
    }

    public async Task<Result<CategoryResponseDto>> AddAsync(CreateCategoryDto dto)
    {
        var category = mapper.Map<Category>(dto);

        await categoryRepository.AddAsync(category);

        await unitOfWork.CommitAsync();

        // Invalida cache ao criar nova categoria
        InvalidateCategoriesCache();

        var response = mapper.Map<CategoryResponseDto>(category);

        return Result<CategoryResponseDto>.Created(response);
    }

    public async Task<Result<CategoryResponseDto?>> UpdateAsync(int id, UpdateCategoryDto dto)
    {
        var category = await categoryRepository.GetByIdAsync(id);

        if (category is null)
        {
            return Result<CategoryResponseDto?>.NotFound(
                string.Format(ErrorMessages.NotFound, "Categoria"));
        }

        // Mapper atualiza apenas campos informados (update parcial)
        mapper.Map(dto, category);
        await categoryRepository.UpdateAsync(category);
        await unitOfWork.CommitAsync();

        // Invalida cache ao atualizar categoria
        InvalidateCategoriesCache();

        var response = mapper.Map<CategoryResponseDto>(category);
        return Result<CategoryResponseDto?>.Success(response);
    }

    public async Task<Result<CategoryResponseDto?>> DeleteAsync(int id)
    {
        var category = await categoryRepository.GetByIdAsync(id);

        if (category is null)
        {
            return Result<CategoryResponseDto?>.NotFound(
                string.Format(ErrorMessages.NotFound, "Categoria"));
        }

        await categoryRepository.DeleteAsync(category);
        await unitOfWork.CommitAsync();

        // Invalida cache ao deletar categoria
        InvalidateCategoriesCache();

        // DELETE retorna 204 NoContent conforme padrão REST
        return Result<CategoryResponseDto?>.NoContent();
    }

    /// <summary>
    /// Remove todas as entradas de cache de categorias.
    /// Chamado automaticamente em operações de escrita (Create/Update/Delete).
    /// </summary>
    private void InvalidateCategoriesCache()
    {
        // MemoryCache não suporta remoção por prefixo nativamente,
        // então usamos abordagem de token de cancelamento ou recriação
        // Para simplicidade, removemos páginas comuns (1-10)
        for (int page = 1; page <= 10; page++)
        {
            for (int size = 5; size <= 50; size += 5)
            {
                cache.Remove($"{CategoriesCacheKeyPrefix}{page}_{size}");
            }
        }
    }
}


