namespace Rezerv.Application.DTOs.HotelReview;

public class CreateHotelReviewDto
{
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
}