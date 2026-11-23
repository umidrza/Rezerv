namespace Rezerv.Application.Interfaces;

public interface IUserRepository
{
    Task<int> CountAsync();
}