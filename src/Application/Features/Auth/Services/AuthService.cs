using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Features.Auth.DTOs;
using Application.Features.Auth.Repositories;
using Domain.Constants;
using Domain.Entities;
using Domain.Enums;

namespace Application.Features.Auth.Services;

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
        var user = new User
        {
            Username = dto.Username,
            PasswordHash = passwordHash.HashPassword(dto.Password),
            Role = UserRole.Common
        };

        await userRepository.AddAsync(user);

        await unitOfWork.CommitAsync();

        return Result<string>.Success("Usuário criado com sucesso");
    }
}


