using Rezerv.Application.DTOs.Dashboard;
using Rezerv.Domain.Entities;
using Rezerv.Domain.Enums;

namespace Rezerv.Application.Interfaces;

public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);
    Task AddIfAvailableAsync(
        Booking booking,
        CancellationToken cancellationToken = default);
    Task UpdateAsync(
        Booking booking,
        CancellationToken cancellationToken = default);
    Task<List<Booking>> GetByHotelIdAsync(
        Guid hotelId,
        CancellationToken cancellationToken = default);
    Task<List<Booking>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
    Task<bool> ApproveAsync(
        Guid bookingId,
        CancellationToken cancellationToken = default);
    Task<bool> IsRoomBookedAsync(
        Guid roomId,
        DateTime checkIn,
        DateTime checkOut,
        CancellationToken cancellationToken = default);
    Task<int> CountAsync(
        BookingStatus? status = null,
        CancellationToken cancellationToken = default);

    Task<int> CountByUserIdAsync(
        Guid userId,
        BookingStatus? status = null,
        CancellationToken cancellationToken = default);

    Task<int> CountByManagerIdAsync(
        Guid managerId,
        BookingStatus? status = null,
        CancellationToken cancellationToken = default);

    Task<List<Booking>> GetRecentAsync(
        int count,
        CancellationToken cancellationToken = default);

    Task<List<Booking>> GetRecentByUserIdAsync(
        Guid userId,
        int count,
        CancellationToken cancellationToken = default);

    Task<List<Booking>> GetRecentByManagerIdAsync(
        Guid managerId,
        int count,
        CancellationToken cancellationToken = default);

    Task<BookingDashboardCounts> GetDashboardCountsAsync(
        CancellationToken cancellationToken = default);

    Task<BookingDashboardCounts> GetDashboardCountsByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<BookingDashboardCounts> GetDashboardCountsByManagerIdAsync(
        Guid managerId,
        CancellationToken cancellationToken = default);
}