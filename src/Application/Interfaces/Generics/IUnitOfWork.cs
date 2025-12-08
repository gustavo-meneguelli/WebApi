namespace Application.Interfaces.Generics;

public interface IUnitOfWork
{
    Task<bool> CommitAsync();
}