using Application.Interfaces;
using Domain.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await context.Users.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task<User> AddUserAsync(User user)
    {
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
        return user;
    }
}