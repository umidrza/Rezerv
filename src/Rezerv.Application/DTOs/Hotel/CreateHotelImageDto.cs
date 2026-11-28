using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Rezerv.Application.DTOs.Common;

namespace Rezerv.Application.DTOs.Hotel;

public class CreateHotelImageDto : IValidatableObject
{
    [NotEmptyGuid]
    public Guid HotelId { get; set; }

    [Required]
    [FileExtensions(Extensions = "jpg,jpeg,png,webp,gif,bmp,avif,tif,tiff")]
    public IFormFile File { get; set; } = null!;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (File is null)
            yield break;

        if (File.Length == 0)
        {
            yield return new ValidationResult(
                "Image file cannot be empty.",
                new[] { nameof(File) });
        }
        else if (File.Length > 5 * 1024 * 1024)
        {
            yield return new ValidationResult(
                "Image size cannot exceed 5 MB.",
                new[] { nameof(File) });
        }
    }
}