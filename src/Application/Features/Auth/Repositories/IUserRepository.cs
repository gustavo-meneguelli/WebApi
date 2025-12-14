using Application.Common.Interfaces;
using Domain.Entities;

namespace Application.Features.Auth.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetUserByUsernameAsync(string username);
}
