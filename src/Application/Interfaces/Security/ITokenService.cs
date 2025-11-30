using Domain.Entities;

namespace Application.Interfaces.Security;

public interface ITokenService
{
    public string GenerateToken(User user);
}