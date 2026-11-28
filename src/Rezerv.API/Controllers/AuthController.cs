using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Rezerv.API.Authentication;
using Rezerv.Application.DTOs.Auth;
using Rezerv.Application.Interfaces;
using Rezerv.Infrastructure.Settings;

namespace Rezerv.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly JwtSettings _jwtSettings;

    public AuthController(
        IAuthService authService,
        IOptions<JwtSettings> jwtOptions)
    {
        _authService = authService;
        _jwtSettings = jwtOptions.Value;
    }

    [HttpPost("register")]
    [EnableRateLimiting("Auth")]
    public async Task<IActionResult> Register(
        RegisterDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _authService.RegisterAsync(dto, cancellationToken);
            SetAuthCookie(result.Token);
            return Ok(result.User);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return ValidationProblem(ModelState);
        }
    }

    [HttpPost("login")]
    [EnableRateLimiting("Auth")]
    public async Task<IActionResult> Login(
        LoginDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _authService.LoginAsync(dto, cancellationToken);
            SetAuthCookie(result.Token);
            return Ok(result.User);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser(
        CancellationToken cancellationToken)
    {
        var userIdValue = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (!Guid.TryParse(userIdValue, out var userId))
            return Unauthorized();

        var user = await _authService.GetCurrentUserAsync(
            userId,
            cancellationToken);
        return user is null ? Unauthorized() : Ok(user);
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete(AuthCookieDefaults.Name, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Path = AuthCookieDefaults.Path
        });

        return NoContent();
    }

    private void SetAuthCookie(string token)
    {
        Response.Cookies.Append(AuthCookieDefaults.Name, token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Path = AuthCookieDefaults.Path,
            Expires = DateTimeOffset.UtcNow.AddMinutes(
                _jwtSettings.ExpirationInMinutes)
        });
    }
}