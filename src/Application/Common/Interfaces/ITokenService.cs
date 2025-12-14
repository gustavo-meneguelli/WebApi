using Application.Features.Auth.DTOs;
using Domain.Entities;

namespace Application.Common.Interfaces;

public interface ITokenService
{
    AuthResponseDto GenerateAuthResponse(User user);
}
