namespace Rezerv.Application.DTOs;

public class CreateBookingDto
{
    public Guid HotelId { get; set; }

    public DateTime CheckIn { get; set; }

    public DateTime CheckOut { get; set; }

    public int NumberOfGuests { get; set; }

    public List<Guid> RoomIds { get; set; } = new();
}