using System.ComponentModel.DataAnnotations;

namespace Rezerv.Application.DTOs.Hotel;

public class CreateHotelDto : IValidatableObject
{
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    [StringLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [StringLength(300, MinimumLength = 1)]
    public string Address { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string City { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Country { get; set; } = string.Empty;

    [Range(1, 5)]
    public int StarRating { get; set; }

    [Url]
    [StringLength(2048)]
    public string? PrimaryImageUrl { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(PrimaryImageUrl))
            yield break;

        if (!Uri.TryCreate(PrimaryImageUrl, UriKind.Absolute, out var imageUri) ||
            (imageUri.Scheme != Uri.UriSchemeHttp &&
             imageUri.Scheme != Uri.UriSchemeHttps))
        {
            yield return new ValidationResult(
                "Primary image URL must use HTTP or HTTPS.",
                new[] { nameof(PrimaryImageUrl) });
        }
    }
}