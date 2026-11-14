using Rezerv.Application.DTOs.Booking;

namespace Rezerv.Application.Interfaces;

public interface IBookingService
{
    Task<BookingDto> CreateBookingAsync(CreateBookingDto dto, Guid userId);
    Task<BookingDto?> GetBookingAsync(Guid id, Guid userId);
    Task<bool> CheckRoomAvailabilityAsync(CheckRoomAvailabilityDto dto);
}