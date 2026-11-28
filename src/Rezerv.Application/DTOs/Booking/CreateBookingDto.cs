using System.ComponentModel.DataAnnotations;
using Rezerv.Application.DTOs.Common;

namespace Rezerv.Application.DTOs.Booking;

public class CreateBookingDto : IValidatableObject
{
    [NotEmptyGuid]
    public Guid HotelId { get; set; }

    [NotEmptyGuid]
    public Guid RoomId { get; set; }

    public DateTime CheckIn { get; set; }

    public DateTime CheckOut { get; set; }

    [Range(1, 100)]
    public int NumberOfGuests { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (CheckIn == default)
            yield return new ValidationResult(
                "Check-in date is required.",
                new[] { nameof(CheckIn) });

        if (CheckOut == default)
            yield return new ValidationResult(
                "Check-out date is required.",
                new[] { nameof(CheckOut) });

        if (CheckIn != default &&
            CheckOut != default &&
            CheckOut <= CheckIn)
        {
            yield return new ValidationResult(
                "Check-out must be after check-in.",
                new[] { nameof(CheckOut) });
        }

        if (CheckIn != default &&
            CheckIn.Date < DateTime.UtcNow.Date)
        {
            yield return new ValidationResult(
                "Check-in date cannot be in the past.",
                new[] { nameof(CheckIn) });
        }
    }
}