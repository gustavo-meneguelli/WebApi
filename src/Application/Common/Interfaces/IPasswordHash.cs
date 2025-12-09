namespace Application.Common.Interfaces;

public interface IPasswordHash
{
    public string HashPassword(string password);
    public bool VerifyHashedPassword(string password, string hashedPassword);
}
