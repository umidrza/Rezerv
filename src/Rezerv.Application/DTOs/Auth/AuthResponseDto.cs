namespace Rezerv.Application.DTOs.Auth;

public sealed record AuthResponseDto(AuthUserDto User, string Token);