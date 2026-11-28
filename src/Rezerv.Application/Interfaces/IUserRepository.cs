namespace Rezerv.Application.Interfaces;

public interface IUserRepository
{
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    Task<Dictionary<Guid, string>> GetFullNamesByIdsAsync(
        IEnumerable<Guid> userIds,
        CancellationToken cancellationToken = default);
}