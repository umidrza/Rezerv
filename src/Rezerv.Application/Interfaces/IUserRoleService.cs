namespace Rezerv.Application.Interfaces;

public interface IUserRoleService
{
    Task AssignRoleAsync(
        Guid userId,
        string role,
        CancellationToken cancellationToken = default);
}