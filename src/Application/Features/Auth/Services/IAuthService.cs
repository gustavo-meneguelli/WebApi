using Application.Common.Models;
using Application.Features.Auth.DTOs;

namespace Application.Features.Auth.Services;

public interface IAuthService
{
    public Task<Result<string>> LoginAsync(LoginDto login);
    public Task<Result<string>> RegisterAsync(UserRegisterDto dto);
}


