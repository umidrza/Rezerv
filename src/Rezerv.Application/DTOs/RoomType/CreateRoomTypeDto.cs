namespace Rezerv.Application.DTOs.RoomType;

public class CreateRoomTypeDto
{
    public Guid HotelId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public decimal PricePerNight { get; set; }
}