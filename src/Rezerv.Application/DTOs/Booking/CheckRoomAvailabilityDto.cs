namespace Rezerv.Application.DTOs.Booking;

public class CheckRoomAvailabilityDto
{
    public List<Guid> RoomIds { get; set; } = new();
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
}