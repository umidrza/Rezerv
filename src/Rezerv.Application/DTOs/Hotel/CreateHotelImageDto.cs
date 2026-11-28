using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Rezerv.Application.Constants;
using Rezerv.Application.DTOs.Common;

namespace Rezerv.Application.DTOs.Hotel;

public class CreateHotelImageDto : IValidatableObject
{
    private static readonly string[] AllowedExtensions =
    {
        ".jpg",
        ".jpeg",
        ".png",
        ".webp"
    };

    [NotEmptyGuid]
    public Guid HotelId { get; set; }

    [Required]
    public IFormFile File { get; set; } = null!;

    public IEnumerable<ValidationResult> Validate(
        ValidationContext validationContext)
    {
        if (File is null)
            yield break;

        var extension = Path.GetExtension(File.FileName);

        if (!AllowedExtensions.Contains(
                extension,
                StringComparer.OrdinalIgnoreCase))
        {
            yield return new ValidationResult(
                "The File field only accepts files with the following extensions: .jpg, .jpeg, .png, .webp",
                new[] { nameof(File) });
        }

        if (File.Length == 0)
        {
            yield return new ValidationResult(
                "Image file cannot be empty.",
                new[] { nameof(File) });
        }
        else if (File.Length > ImageUploadLimits.MaxFileSizeBytes)
        {
            yield return new ValidationResult(
                "Image size cannot exceed 5 MB.",
                new[] { nameof(File) });
        }
    }
}