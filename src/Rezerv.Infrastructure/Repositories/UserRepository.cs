using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Rezerv.Application.Interfaces;
using Rezerv.Infrastructure.Identity;

namespace Rezerv.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserRepository(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return _userManager.Users.CountAsync(cancellationToken);
    }

    public async Task<Dictionary<Guid, string>> GetFullNamesByIdsAsync(
        IEnumerable<Guid> userIds,
        CancellationToken cancellationToken = default)
    {
        var ids = userIds.Distinct().ToList();

        return await _userManager.Users
            .Where(user => ids.Contains(user.Id))
            .ToDictionaryAsync(
                user => user.Id,
                user => user.FullName,
                cancellationToken);
    }
}