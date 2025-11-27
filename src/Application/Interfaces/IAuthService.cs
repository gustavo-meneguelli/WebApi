using Application.DTO;
using Application.Utilities;

namespace Application.Interfaces;

public interface IAuthService
{
    public Task<Result<string>> LoginAsync(LoginDto login);
    public Task<Result<string>> RegisterAsync(UserRegisterDto dto);
}