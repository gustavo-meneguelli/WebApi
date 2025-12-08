using System.Linq.Expressions;
using Application.Common.Models;
using Application.Interfaces.Generics;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Infrastructure.Generics;

public class Repository<T>(AppDbContext context) : IRepository<T> where T : class
{
    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await context.Set<T>().FindAsync(id);
    }

    public async Task<T> AddAsync(T entity)
    {
        await context.Set<T>().AddAsync(entity);
        
        return entity;
    }

    public async Task UpdateAsync(T entity)
    {
        context.Set<T>().Update(entity);
        
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(T entity)
    {
        context.Set<T>().Remove(entity);
        await Task.CompletedTask;
    }

    public async Task<PagedResult<T>> GetAllAsync(
        PaginationParams paginationParams,
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IIncludableQueryable<T,
            object>>? include = null)
    {
        IQueryable<T> query = context.Set<T>().AsNoTracking();

        if (include != null)
        {
            query = include(query);
        }

        if (filter != null)
        {
            query = query.Where(filter);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .ToListAsync();

        var totalPages = (int)Math.Ceiling(totalCount / (double)paginationParams.PageSize);

        return new PagedResult<T>
        {
            Items = items,
            CurrentPage = paginationParams.PageNumber,
            PageSize = paginationParams.PageSize,
            TotalCount = totalCount,
            TotalPages = totalPages
        };
    }
}