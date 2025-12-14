using Application.Common.Interfaces;
using Domain.Entities;

namespace Application.Features.Categories.Repositories;

public interface ICategoryRepository : IRepository<Category>
{
    Task<IEnumerable<Category>> GetAllAsync();
    Task<bool> ExistsByNameAsync(string name);
}
