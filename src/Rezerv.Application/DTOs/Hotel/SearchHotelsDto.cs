using System.ComponentModel.DataAnnotations;

namespace Rezerv.Application.DTOs.Hotel;

public class SearchHotelsDto : IValidatableObject
{
    [StringLength(100)]
    public string? City { get; set; }

    [StringLength(100)]
    public string? Country { get; set; }

    [Range(1, 5)]
    public int? StarRating { get; set; }

    [Range(1, 5)]
    public double? MinimumAverageRating { get; set; }

    [StringLength(24)]
    [RegularExpression(
        "(?i)^(Name|StarRating|AverageRating)$",
        ErrorMessage = "SortBy must be Name, StarRating, or AverageRating.")]
    public string? SortBy { get; set; }

    public bool SortDescending { get; set; } = true;

    public DateTime? CheckIn { get; set; }

    public DateTime? CheckOut { get; set; }

    [Range(1, 100)]
    public int? NumberOfGuests { get; set; }

    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    [Range(1, 100)]
    public int PageSize { get; set; } = 12;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (CheckIn.HasValue != CheckOut.HasValue)
        {
            yield return new ValidationResult(
                "Both check-in and check-out dates are required.",
                new[] { nameof(CheckIn), nameof(CheckOut) });
        }
        else if (CheckIn.HasValue && CheckOut.HasValue)
        {
            if (CheckOut.Value <= CheckIn.Value)
            {
                yield return new ValidationResult(
                    "Check-out must be after check-in.",
                    new[] { nameof(CheckOut) });
            }

            if (CheckIn.Value.Date < DateTime.UtcNow.Date)
            {
                yield return new ValidationResult(
                    "Check-in date cannot be in the past.",
                    new[] { nameof(CheckIn) });
            }
        }
    }
}