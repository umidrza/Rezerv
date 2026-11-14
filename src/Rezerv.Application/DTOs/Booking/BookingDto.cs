namespace Rezerv.Application.DTOs.Booking;

public class BookingDto
{
    public Guid Id { get; set; }

    public Guid HotelId { get; set; }

    public DateTime CheckIn { get; set; }

    public DateTime CheckOut { get; set; }

    public int NumberOfGuests { get; set; }

    public decimal TotalPrice { get; set; }

    public string Status { get; set; } = string.Empty;
}