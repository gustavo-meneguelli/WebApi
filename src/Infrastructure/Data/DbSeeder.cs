using Application.Interfaces.Generics;
using Application.Interfaces.Repositories;
using Application.Interfaces.Security;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Data;

public class DbSeeder(IUserRepository userRepository, IPasswordHash passwordHash, IConfiguration configuration, IUnitOfWork unitOfWork)
{
    public async Task SeedAsync()
    {
        var userExists = await userRepository.GetUserByUsernameAsync("admin");

        if (userExists is null)
        {
            var password = configuration["AdminSettings:Password"]
                ?? throw new InvalidOperationException("AdminSettings:Password is null");
            
            var user = new User()
            {
                Username = "admin",
                Role = UserRole.Admin,
                PasswordHash = passwordHash.HashPassword(password)
            };
            
            await userRepository.AddAsync(user);
            await unitOfWork.CommitAsync();
        }
    }
}