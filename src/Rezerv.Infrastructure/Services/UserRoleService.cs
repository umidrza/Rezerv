using Microsoft.AspNetCore.Identity;
using Rezerv.Application.Interfaces;
using Rezerv.Infrastructure.Identity;

namespace Rezerv.Infrastructure.Services;

public class UserRoleService : IUserRoleService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserRoleService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task AssignRoleAsync(
        Guid userId,
        string role,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var user = await _userManager.FindByIdAsync(userId.ToString());
        cancellationToken.ThrowIfCancellationRequested();

        if (user == null)
            throw new ArgumentException("User not found.");

        var alreadyInRole = await _userManager.IsInRoleAsync(user, role);
        cancellationToken.ThrowIfCancellationRequested();

        if (!alreadyInRole)
        {
            var result = await _userManager.AddToRoleAsync(user, role);
            cancellationToken.ThrowIfCancellationRequested();

            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }
}