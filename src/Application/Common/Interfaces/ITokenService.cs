using Domain.Entities;

namespace Application.Common.Interfaces;

public interface ITokenService
{
    public string GenerateToken(User user);
}
