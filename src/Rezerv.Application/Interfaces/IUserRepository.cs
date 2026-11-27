namespace Rezerv.Application.Interfaces;

public interface IUserRepository
{
    Task<int> CountAsync();
    Task<Dictionary<Guid, string>> GetFullNamesByIdsAsync(
        IEnumerable<Guid> userIds);
}