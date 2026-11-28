using Rezerv.Application.DTOs.Booking;

namespace Rezerv.Application.Interfaces;

public interface IBookingService
{
    Task<BookingDto> CreateBookingAsync(
        CreateBookingDto dto,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<BookingDto?> GetBookingAsync(
        Guid bookingId,
        CancellationToken cancellationToken = default);

    Task<bool> CheckRoomAvailabilityAsync(
        Guid roomId,
        DateTime checkIn,
        DateTime checkOut,
        CancellationToken cancellationToken = default);

    Task<bool> ApproveBookingAsync(
        Guid bookingId,
        Guid userId,
        bool isAdmin,
        CancellationToken cancellationToken = default);

    Task<bool> RejectBookingAsync(
        Guid bookingId,
        Guid userId,
        bool isAdmin,
        CancellationToken cancellationToken = default);

    Task<List<BookingDto>> GetHotelBookingsAsync(
        Guid hotelId,
        Guid userId,
        bool isAdmin,
        CancellationToken cancellationToken = default);

    Task<List<BookingDto>> GetMyBookingsAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task CancelBookingAsync(
        Guid bookingId,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<BookingDto?> GetBookingForUserAsync(
        Guid bookingId,
        Guid userId,
        bool isAdmin,
        CancellationToken cancellationToken = default);
}