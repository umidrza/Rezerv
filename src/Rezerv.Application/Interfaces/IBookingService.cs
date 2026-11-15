using Rezerv.Application.DTOs.Booking;

namespace Rezerv.Application.Interfaces;

public interface IBookingService
{
    Task<BookingDto> CreateBookingAsync(CreateBookingDto dto, Guid userId);
    Task<BookingDto?> GetBookingAsync(Guid id, Guid userId);
    Task<bool> CheckRoomAvailabilityAsync(CheckRoomAvailabilityDto dto);
    Task ApproveBookingAsync(Guid bookingId, Guid userId, bool isAdmin);
    Task RejectBookingAsync(Guid bookingId, Guid userId, bool isAdmin);
    Task CancelBookingAsync(Guid bookingId, Guid userId);
    Task<List<BookingDto>> GetHotelBookingsAsync(Guid hotelId, Guid userId, bool isAdmin);
    Task<List<BookingDto>> GetMyBookingsAsync(Guid userId);
}