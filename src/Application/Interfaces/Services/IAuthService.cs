using Application.Common.Models;
using Application.DTOs.Auth;

namespace Application.Interfaces.Services;

public interface IAuthService
{
    public Task<Result<string>> LoginAsync(LoginDto login);
    public Task<Result<string>> RegisterAsync(UserRegisterDto dto);
}