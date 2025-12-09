using Application.Features.Auth.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Generics;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserRepository(AppDbContext context) : Repository<User>(context), IUserRepository
{
    private readonly AppDbContext _context = context;

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
    }
}
