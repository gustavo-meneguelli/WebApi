using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

public class User : Entity
{
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Common;

    public override string ToString()
    {
        return $"User {{ Id = {Id}, Username = \"{Username}\", Role = {Role} }}";
    }
}