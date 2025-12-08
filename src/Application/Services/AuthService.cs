using Application.Common.Models;
using Application.DTO.Auth;
using Application.Interfaces.Generics;
using Application.Interfaces.Repositories;
using Application.Interfaces.Security;
using Application.Interfaces.Services;
using Domain.Constants;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services;

public class AuthService(IUserRepository userRepository, IPasswordHash passwordHash, ITokenService tokenService, IUnitOfWork unitOfWork)
    : IAuthService
{
    public async Task<Result<string>> LoginAsync(LoginDto dto)
    {
        var user = await userRepository.GetUserByUsernameAsync(dto.Username);

        if (user is null)
        {
            return Result<string>.Unauthorized(string.Format(ErrorMessages.CredentialsInvalid));
        }

        var passwordIsValid = passwordHash.VerifyHashedPassword(dto.Password, user.PasswordHash);

        if (!passwordIsValid)
        {
            return Result<string>.Unauthorized(string.Format(ErrorMessages.CredentialsInvalid));
        }
        
        var token = tokenService.GenerateToken(user);
        
        return Result<string>.Success(token);
    }

    public async Task<Result<string>> RegisterAsync(UserRegisterDto dto)
    {
        var userExists = await userRepository.GetUserByUsernameAsync(dto.Username);

        if (userExists is not null)
        {
            return Result<string>.Duplicate(string.Format(ErrorMessages.AlreadyExists, "usuário", "username"));
        }
        
        var user = new User
            { Username = dto.Username, PasswordHash = passwordHash.HashPassword(dto.Password), Role = UserRole.Common};
        
        await userRepository.AddAsync(user);
        
        await unitOfWork.CommitAsync();
        
        return Result<string>.Success("User created with success");
    }
}