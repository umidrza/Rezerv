using Rezerv.Application.DTOs.Booking;

namespace Rezerv.Application.Interfaces;

public interface IBookingService
{
    Task<BookingDto> CreateBookingAsync(
        CreateBookingDto dto,
        Guid userId);

    Task<BookingDto?> GetBookingAsync(Guid bookingId);

    Task<bool> CheckRoomAvailabilityAsync(
        Guid roomId,
        DateTime checkIn,
        DateTime checkOut);

    Task<bool> ApproveBookingAsync(
        Guid bookingId,
        Guid userId,
        bool isAdmin);

    Task<bool> RejectBookingAsync(
        Guid bookingId,
        Guid userId,
        bool isAdmin);

    Task<List<BookingDto>> GetHotelBookingsAsync(
        Guid hotelId,
        Guid userId,
        bool isAdmin);

    Task<List<BookingDto>> GetMyBookingsAsync(Guid userId);

    Task<bool> CancelBookingAsync(
        Guid bookingId,
        Guid userId);
}