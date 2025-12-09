using Application.Common.Interfaces;
using Infrastructure.Data;

namespace Infrastructure.Generics;

public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    public async Task<bool> CommitAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }
}
