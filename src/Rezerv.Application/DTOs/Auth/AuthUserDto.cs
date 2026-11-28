namespace Rezerv.Application.DTOs.Auth;

public sealed record AuthUserDto(
    Guid Id,
    string Email,
    string Name,
    IReadOnlyList<string> Roles);
