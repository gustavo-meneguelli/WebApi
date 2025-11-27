using Application.DTO;
using Application.Interfaces;
using Application.Utilities;
using Domain.Enums;
using Domain.Models;

namespace Application.Services;

public class AuthService(IUserRepository userRepository, IPasswordHash passwordHash, ITokenService tokenService)
    : IAuthService
{
    public async Task<Result<string>> LoginAsync(LoginDto dto)
    {
        var user = await userRepository.GetUserByUsernameAsync(dto.Username);

        if (user is null)
        {
            return Result<string>.Unauthorized("User or password is invalid");
        }

        var passwordIsValid = passwordHash.VerifyHashedPassword(dto.Password, user.PasswordHash);

        if (!passwordIsValid)
        {
            return Result<string>.Unauthorized("User or password is invalid");
        }
        
        var token = tokenService.GenerateToken(user);
        
        return Result<string>.Success(token);
    }

    public async Task<Result<string>> RegisterAsync(UserRegisterDto dto)
    {
        var userExists = await userRepository.GetUserByUsernameAsync(dto.Username);

        if (userExists is not null)
        {
            return Result<string>.Duplicate("User already exists");
        }
        
        var user = new User
            { Username = dto.Username, PasswordHash = passwordHash.HashPassword(dto.Password), Role = UserRole.Common};
        
        await userRepository.AddUserAsync(user);
        
        return Result<string>.Success("User created with success");
    }
}