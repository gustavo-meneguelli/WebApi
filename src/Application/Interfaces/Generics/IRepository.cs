using System.Linq.Expressions;
using Application.Common.Models;
using Microsoft.EntityFrameworkCore.Query;

namespace Application.Interfaces.Generics;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);

    Task<PagedResult<T>> GetAllAsync(
        PaginationParams paginationParams,
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null);
}