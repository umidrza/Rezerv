using System.ComponentModel.DataAnnotations;

namespace Rezerv.Application.DTOs.HotelReview;

public class CreateHotelReviewDto
{
    [Range(1, 5)]
    public int Rating { get; set; }

    [Required]
    [StringLength(1000, MinimumLength = 1)]
    public string Comment { get; set; } = string.Empty;
}