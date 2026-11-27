namespace Rezerv.Application.DTOs.HotelReview;

public class HotelReviewDto
{
    public Guid Id { get; set; }
    public Guid HotelId { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}