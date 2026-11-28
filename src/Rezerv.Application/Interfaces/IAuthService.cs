using Rezerv.Application.DTOs.Auth;

namespace Rezerv.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(
        RegisterDto dto,
        CancellationToken cancellationToken = default);

    Task<AuthResponseDto> LoginAsync(
        LoginDto dto,
        CancellationToken cancellationToken = default);

    Task<AuthUserDto?> GetCurrentUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}