namespace Rezerv.Application.DTOs.Hotel;

public class CreateHotelImageDto
{
    public Guid HotelId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
}