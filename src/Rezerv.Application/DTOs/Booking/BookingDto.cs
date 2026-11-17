using Rezerv.Domain.Enums;

namespace Rezerv.Application.DTOs.Booking;

public class BookingDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public Guid HotelId { get; set; }
    public string HotelName { get; set; } = string.Empty;

    public Guid RoomId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public string RoomTypeName { get; set; } = string.Empty;

    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }

    public int NumberOfGuests { get; set; }

    public decimal PricePerNight { get; set; }
    public decimal TotalPrice { get; set; }

    public BookingStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}