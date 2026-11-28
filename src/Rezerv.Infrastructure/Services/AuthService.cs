using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Rezerv.Application.DTOs.Auth;
using Rezerv.Application.Interfaces;
using Rezerv.Infrastructure.Identity;
using Rezerv.Infrastructure.Settings;

namespace Rezerv.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly JwtSettings _jwtSettings;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IOptions<JwtSettings> jwtOptions)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtSettings = jwtOptions.Value;
    }

    public async Task<AuthResponseDto> RegisterAsync(
        RegisterDto dto,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FullName = dto.FullName
        };

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        await _userManager.AddToRoleAsync(user, "Customer");
        cancellationToken.ThrowIfCancellationRequested();

        return await GenerateTokenAsync(user, cancellationToken);
    }

    public async Task<AuthResponseDto> LoginAsync(
        LoginDto dto,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var user = await _userManager.FindByEmailAsync(dto.Email);
        cancellationToken.ThrowIfCancellationRequested();

        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        var result = await _signInManager.CheckPasswordSignInAsync(
            user,
            dto.Password,
            lockoutOnFailure: true);
        cancellationToken.ThrowIfCancellationRequested();

        if (!result.Succeeded)
            throw new UnauthorizedAccessException("Invalid email or password.");

        return await GenerateTokenAsync(user, cancellationToken);
    }

    public async Task<AuthUserDto?> GetCurrentUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var user = await _userManager.FindByIdAsync(userId.ToString());
        cancellationToken.ThrowIfCancellationRequested();
        if (user is null)
            return null;

        var roles = await _userManager.GetRolesAsync(user);
        cancellationToken.ThrowIfCancellationRequested();
        return new AuthUserDto(
            user.Id,
            user.Email ?? string.Empty,
            user.FullName,
            roles.ToArray());
    }

    private async Task<AuthResponseDto> GenerateTokenAsync(
        ApplicationUser user,
        CancellationToken cancellationToken)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.Name, user.FullName)
        };

        var roles = await _userManager.GetRolesAsync(user);
        cancellationToken.ThrowIfCancellationRequested();

        foreach (var role in roles)
        {
            claims.Add(new Claim("role", role));
        }

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtSettings.Secret));

        var credentials = new SigningCredentials(
            key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                _jwtSettings.ExpirationInMinutes),
            signingCredentials: credentials);

        var userDto = new AuthUserDto(
            user.Id,
            user.Email!,
            user.FullName,
            roles.ToArray());

        return new AuthResponseDto(
            userDto,
            new JwtSecurityTokenHandler().WriteToken(token));
    }
}