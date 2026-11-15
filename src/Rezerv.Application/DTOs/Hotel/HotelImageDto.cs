namespace Rezerv.Application.DTOs.Hotel;

public class HotelImageDto
{
    public Guid Id { get; set; }
    public Guid HotelId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
}