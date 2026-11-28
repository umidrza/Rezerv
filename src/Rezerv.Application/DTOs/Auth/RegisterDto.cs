using System.ComponentModel.DataAnnotations;

namespace Rezerv.Application.DTOs.Auth;

public class RegisterDto
{
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(254)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(128, MinimumLength = 8)]
    public string Password { get; set; } = string.Empty;
}