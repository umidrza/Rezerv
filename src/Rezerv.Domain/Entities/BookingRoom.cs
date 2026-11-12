namespace Rezerv.Domain.Entities;

public class BookingRoom
{
    public Guid Id { get; set; }

    public Guid BookingId { get; set; }

    public Guid RoomId { get; set; }

    public decimal PricePerNight { get; set; }

    public Booking Booking { get; set; } = null!;

    public Room Room { get; set; } = null!;
}