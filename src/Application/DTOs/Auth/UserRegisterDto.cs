using Domain.Enums;

namespace Application.DTOs.Auth;

public class UserRegisterDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public UserRole UserRole { get; set; } = UserRole.Common;
}